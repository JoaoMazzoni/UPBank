using Models;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployees();

        Task<Employee> GetEmployee(string cpf);

        Task<Employee> GetEmployeeOnAgency(string cpf);

        Task<Employee> PostEmployee(Employee employee);

        Task<RemovedAgencyEmployee> PostEmployeeHistory(RemovedAgencyEmployee employee);

        Task<Employee> DeleteEmployee(string document);

    }
}
