using System.Net;
using Models;
using Models.DTO;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace AccountAPI.Services;

public class AccountService
{
    private readonly HttpClient _http = new();
    private const string CustomerBaseUri = "https://localhost:7045/api/Customers";
    private const string EmployeeBaseUri = "https://localhost:7040/api/Employees";
    private const string AgencyBaseUri = "https://localhost:7196/api/Agencies";

    public DisabledAccount DisableAccountFeatures(Account account)
    {
        account.Restriction = true;
        if (account.CreditCard != null)
        {
            account.CreditCard.Active = false;
        }

        return new DisabledAccount(account);
    }

    public Account EnableAccountFeatures(DisabledAccount disabledAccount)
    {
        disabledAccount.Restriction = false;
        if (disabledAccount.CreditCard != null)
        {
            disabledAccount.CreditCard.Active = false;
        }

        return new Account(disabledAccount);
    }

    public async Task<bool> ValidateManagerRequest(string managerIdStr)
    {
        try
        {
            var id = int.Parse(managerIdStr);
            var employeeResponse = await _http.GetAsync($"{EmployeeBaseUri}/Get/Managers/");
            if (employeeResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var employees =
                JsonConvert.DeserializeObject<List<Employee>>(await employeeResponse.Content.ReadAsStringAsync());
            if (employees == null)
            {
                return false;
            }

            var foundEmployee = employees.Find(e => e.Register == id);
            if (foundEmployee is not { Manager: true } or null)
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return true;
    }

    public AccountProfile GetProfileBySalary(double salary)
    {
        var profile = salary switch
        {
            <= 2000 => AccountProfile.Academic,
            <= 5000 => AccountProfile.Normal,
            > 5000 => AccountProfile.Premium,
            _ => throw new ArgumentOutOfRangeException(nameof(salary), salary, "Invalid customer salary.")
        };
        return profile;
    }

    public double GetSpecialLimitBySalary(double salary)
    {
        double specialLimit = salary switch
        {
            <= 2000 => 500,
            <= 5000 => 2000,
            > 5000 => 4000,
            _ => throw new ArgumentOutOfRangeException(nameof(salary), salary, "Invalid customer salary.")
        };
        return specialLimit;
    }

    public async Task<Customer?> GetCustomerData(string customerCpf)
    {
        Customer? customer;
        try
        {
            var customerResponse = await _http.GetAsync($"{CustomerBaseUri}/{customerCpf}");
            if (customerResponse.StatusCode != HttpStatusCode.OK)
            {
                return null;
            }

            customer = JsonConvert.DeserializeObject<Customer>(await customerResponse.Content.ReadAsStringAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

        return customer;
    }

    public CreditCard? GenerateCreditCard(AccountProfile customerProfile, string ownerName)
    {
        var creditCard = new CreditCard();
        creditCard.Owner = ownerName;
        creditCard.SecurityCode = $"{new Random().Next(1, 999)}".PadLeft(3, '0');

        switch (customerProfile)
        {
            case AccountProfile.Academic:
                creditCard.Number = new Random().NextInt64(3000000000000000, 9999999999999999);
                creditCard.Limit = 800;
                creditCard.Date = new DateTime(new DateOnly().Year + 5, new DateOnly().Month, 1);
                creditCard.Flag = CreditCardFlags.Elo.ToString();
                break;
            case AccountProfile.Normal:
                creditCard.Number = new Random().NextInt64(4000000000000000, 9999999999999999);
                creditCard.Limit = 8000;
                creditCard.Date = new DateTime(new DateOnly().Year + 8, new DateOnly().Month, 1);
                creditCard.Flag = CreditCardFlags.MasterCard.ToString();
                break;
            case AccountProfile.Premium:
                creditCard.Number = new Random().NextInt64(5000000000000000, 9999999999999999);
                creditCard.Limit = 20000;
                creditCard.Date = new DateTime(new DateOnly().Year + 12, new DateOnly().Month, 1);
                creditCard.Flag = CreditCardFlags.Visa.ToString();
                break;
            default:
                return null;
        }

        return creditCard;
    }

    public async Task<bool> CheckAgencyStatus(string agencyNumber)
    {
        try
        {
            var agencyResponse = await _http.GetAsync($"{AgencyBaseUri}/{agencyNumber}");
            if (agencyResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var agency = JsonConvert.DeserializeObject<Agency>(await agencyResponse.Content.ReadAsStringAsync());
            if (agency is null or { Restriction: true })
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return true;
    }
}