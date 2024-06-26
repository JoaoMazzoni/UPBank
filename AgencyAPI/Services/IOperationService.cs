using Models;
using System.Security.Principal;

namespace AgencyAPI.Services
{
    public interface IOperationService
    {
        Task<List<Operation>> GetOperationsByTypeLoan(string account);
    }

}
