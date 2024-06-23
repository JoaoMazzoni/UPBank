using Models;
using Newtonsoft.Json;
using System.Text;
using static Models.Account;

namespace AgencyAPI.Services
{
    public class AccountService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Account>> GetRestrictedAccounts()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/accounts");

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
            var response = await _client.GetAsync("https://localhost:5001/api/accounts");

            if (response.IsSuccessStatusCode)
            {
                List<Account> AccountsPerProfile = new();

                var accounts = await response.Content.ReadFromJsonAsync<List<Account>>();
                if (Enum.TryParse(profile, out Profile profileEnum))
                {
                    foreach (var account in accounts)
                    {

                    }
                }
            }
            else
                return null;
        }

        public async Task<Account> PostAccount(Account account)
        {

            try
            {

                var content = new StringContent(JsonConvert.SerializeObject(account), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("https://localhost:5001/api/accounts", content);

                response.EnsureSuccessStatusCode();
                string accountResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Account>(accountResponse);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
