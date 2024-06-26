using System.Net;
using Models;
using Models.DTO;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace AccountAPI.Services;

public class AccountService
{
    private HttpClient _http = new();
    private readonly string _customerBaseUri = "https://localhost:7045/api/Customers";
    private readonly string _employeeBaseUri = "https://localhost:7040/api/Employees";

    public async Task<Account> PopulateAccountData(AccountDTO dto)
    {
        return new Account
        {
            Number = dto.Number,
            AgencyNumber = dto.AgencyNumber,
            SavingsAccountNumber = dto.SavingsAccountNumber,
            MainCustomerId = dto.MainCustomerId,
            SecondaryCustomerId = dto.SecondaryCustomerId,
            Restriction = dto.Restriction,
            SpecialLimit = dto.SpecialLimit,
            Date = dto.Date,
            Balance = dto.Balance
        };
    }

    public DisabledAccount DisableAccountFeatures(Account account)
    {
        account.Restriction = true;
        if (account.CreditCard != null)
        {
            account.CreditCard.Active = false;
        }

        return new DisabledAccount
        {
            Number = account.Number,
            AgencyNumber = account.AgencyNumber,
            SavingsAccountNumber = account.SavingsAccountNumber,
            MainCustomerId = account.MainCustomerId,
            SecondaryCustomerId = account.SecondaryCustomerId,
            CreditCard = account.CreditCard,
            Restriction = account.Restriction,
            SpecialLimit = account.SpecialLimit,
            Date = account.Date,
            Balance = account.Balance,
            Profile = account.Profile
        };
    }

    public Account EnableAccountFeatures(DisabledAccount disabledAccount)
    {
        disabledAccount.Restriction = false;
        if (disabledAccount.CreditCard != null)
        {
            disabledAccount.CreditCard.Active = false;
        }

        return new Account
        {
            Number = disabledAccount.Number,
            AgencyNumber = disabledAccount.AgencyNumber,
            SavingsAccountNumber = disabledAccount.SavingsAccountNumber,
            MainCustomerId = disabledAccount.MainCustomerId,
            SecondaryCustomerId = disabledAccount.SecondaryCustomerId,
            CreditCard = disabledAccount.CreditCard,
            Restriction = disabledAccount.Restriction,
            SpecialLimit = disabledAccount.SpecialLimit,
            Date = disabledAccount.Date,
            Balance = disabledAccount.Balance,
            Profile = disabledAccount.Profile
        };
    }

    public async Task<bool> ValidateManagerRequest(string managerIdStr)
    {
        try
        {
            var id = int.Parse(managerIdStr);
            var employeeResponse = await _http.GetAsync($"{_employeeBaseUri}/Get/Managers/");
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
            if (foundEmployee == null || !foundEmployee.Manager)
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
            > 5000 => AccountProfile.Premium
        };
        return profile;
    }

    public double GetSpecialLimitBySalary(double salary)
    {
        double specialLimit = salary switch
        {
            <= 2000 => 500,
            <= 5000 => 2000,
            > 5000 => 4000
        };
        return specialLimit;
    }

    public async Task<Customer?> GetCustomerData(string customerCpf)
    {
        Customer? customer;
        try
        {
            var customerResponse = await _http.GetAsync($"{_customerBaseUri}/{customerCpf}");
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
}