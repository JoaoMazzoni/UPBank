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
                else
                    return null;
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
                    {
                        return accountsPerProfile;
                    }
                    else
                    {
                        Console.WriteLine($"Nenhuma conta do perfil: {profile}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Erro ao recuperar contas da API");
            }
            return null;
        }

        public async Task<ActionResult<IEnumerable<Account>>> GetActiveLoan()
        {
            var response = await _client.GetAsync("https://localhost:7285/api/accounts");

            if (response.IsSuccessStatusCode)
            {
                List<Account> activeLoans = new();

                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();

                foreach (var account in accounts)
                {
                    if (account.OperationAccounts.Equals("Loan"))
                        activeLoans.Add(account);
                }

                if (activeLoans != null)
                    return activeLoans;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
