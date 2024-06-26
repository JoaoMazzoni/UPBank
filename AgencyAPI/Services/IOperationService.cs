using Models;
using System.Security.Principal;

namespace AgencyAPI.Services
{
    public interface IOperationService
    {
        Task<List<Operation>> GetOperationsByType(string account);
    }

}
