using Models;
using Models.DTO;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace AccountAPI.Services;

public class AccountService
{
    private HttpClient _http = new();
    private string _agencyBaseUri = "https://localhost:7196/api";
    private string _clientBaseUri = "https://localhost:7045/api";

    public async Task<Account> PopulateAccountData(AccountDTO dto)
    {
        return new Account
        {
            Number = dto.Number,
            AgencyNumber = dto.AgencyNumber,
            SavingsAccountNumber = dto.SavingsAccountNumber,
            MainClientId = dto.MainClientId,
            SecondaryClientId = dto.SecondaryClientId,
            Restriction = dto.Restriction,
            //CreditCard = await GenerateCreditCard(await SetProfile(dto.MainClientId), dto.MainClientId),
            SpecialLimit = dto.SpecialLimit,
            Date = dto.Date,
            Balance = dto.Balance,
            //Profile = await SetProfile(dto.MainClientId),
        };
    }
    public async Task<Models.AccountProfile> SetProfile(string clientCpf)
    {
        Client client = new();
        AccountProfile profile;
        try
        {
            var clientResponse = await _http.GetAsync($"{_clientBaseUri}/{clientCpf}");
            client = JsonConvert.DeserializeObject<Client>(clientResponse.Content.ToJson());
       
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
    public async Task<CreditCard?> GenerateCreditCard(AccountProfile clientProfile, string clientCpf)
    {
        Client? client = new();
        long cardNumber;
        double cardLimit;
        DateTime expirationDate;
        CreditCardFlags cardFlag;
        try
        {
            var clientResponse = await _http.GetAsync($"{_clientBaseUri}/{clientCpf}");
            client = JsonConvert.DeserializeObject<Client>(clientResponse.Content.ToJson());
            if (client == null)
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var ownerName = client.Name;
        var cardSecurityCode = $"{new Random().Next(1, 999)}".PadLeft(3, '0');
        switch (clientProfile)
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