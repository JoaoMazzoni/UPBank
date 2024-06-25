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

    public async Task<bool> ValidateManagerRequest(string managerId)
    {
        Employee? employee;
        try
        {
            var employeeResponse = await _http.GetAsync($"{_employeeBaseUri}/{managerId}");
            if (employeeResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            employee = JsonConvert.DeserializeObject<Employee>(await employeeResponse.Content.ReadAsStringAsync());

            if (employee is not { Manager: true })
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

    public async Task<AccountProfile> SetProfile(string clientCpf)
    {
        Customer client = new();
        AccountProfile profile;
        try
        {
            var clientResponse = await _http.GetAsync($"{_customerBaseUri}/{clientCpf}");
            client = JsonConvert.DeserializeObject<Customer>(clientResponse.Content.ToJson());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        profile = client.Salary switch
        {
            <= 2000 => AccountProfile.Academic,
            > 2000 and <= 5000 => AccountProfile.Normal,
            > 5000 => AccountProfile.Premium
        };
        return profile;
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

    public CreditCard? GenerateCreditCard(AccountProfile customerProfile, Customer customer)
    {
        long cardNumber;
        double cardLimit;
        DateTime expirationDate;
        CreditCardFlags cardFlag;

        var ownerName = customer.Name;
        var cardSecurityCode = $"{new Random().Next(1, 999)}".PadLeft(3, '0');
        switch (customerProfile)
        {
            case AccountProfile.Academic:
                cardNumber = new Random().NextInt64(3000000000000000, 9999999999999999);
                cardLimit = 800;
                expirationDate = new DateTime(new DateOnly().Year + 5, new DateOnly().Month, 1);
                cardFlag = CreditCardFlags.Elo;
                break;
            case AccountProfile.Normal:
                cardNumber = new Random().NextInt64(4000000000000000, 9999999999999999);
                cardLimit = 8000;
                expirationDate = new DateTime(new DateOnly().Year + 8, new DateOnly().Month, 1);
                cardFlag = CreditCardFlags.MasterCard;
                break;
            case AccountProfile.Premium:
                cardNumber = new Random().NextInt64(5000000000000000, 9999999999999999);
                cardLimit = 20000;
                expirationDate = new DateTime(new DateOnly().Year + 12, new DateOnly().Month, 1);
                cardFlag = CreditCardFlags.Visa;
                break;
            default:
                return null;
        }

        var creditCard = new CreditCard()
        {
            Number = cardNumber,
            Flag = cardFlag.ToString(),
            Date = expirationDate,
            Owner = ownerName,
            SecurityCode = cardSecurityCode,
            Limit = cardLimit,
            Active = false
        };
        return creditCard;
    }
}