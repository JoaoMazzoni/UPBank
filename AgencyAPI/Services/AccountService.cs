using Microsoft.AspNetCore.Mvc;
using Models;

namespace AgencyAPI.Services
{
    public class AccountService : IAccountService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Account>> GetRestrictedAccounts()
        {
            var response = await _client.GetAsync("https://localhost:7285/api/accounts");

            if (response.IsSuccessStatusCode)
            {
                List<Account> restrictedAccounts = new();

                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();

                foreach (var account in accounts)
                {
                    if (account.Restriction)
                        restrictedAccounts.Add(account);
                }

                if (restrictedAccounts != null)
                    return restrictedAccounts;
            }
            else
                return null;
        }

        public async Task<List<Account>> GetAccountsPerProfile(string profile)
        {
            var response = await _client.GetAsync("https://localhost:7285/api/accounts");
            if (response.IsSuccessStatusCode)
            {
                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
                if (accounts != null)
                {
                    var accountsPerProfile = accounts.Where(a => a.Profile.ToString().Equals(profile)).ToList();
                    //(profile)).ToList();
                    if (accountsPerProfile.Count != 0)
                        return accountsPerProfile; 
                }
            }
            
            return null;
        }
    }
}
