using Models;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployees();

        Task<Employee> GetEmployee(string cpf);

        Task<Employee> IfExistGetEmployeeOnAgency(string cpf);

        Task<RemovedAgencyEmployee> GetEmployeeAgencyNumber(string cpf);

        Task<Employee> PostEmployee(Employee employee);

        Task<RemovedAgencyEmployee> PostEmployeeHistory(RemovedAgencyEmployee employee);

        Task<Employee> DeleteEmployee(string document);

    }
}
