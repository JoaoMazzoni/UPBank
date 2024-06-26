using Microsoft.AspNetCore.Mvc;
using Models;

namespace AgencyAPI.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetRestrictedAccounts();

        Task<List<Account>> GetAccountsPerProfile(string profile);   
    }
}
