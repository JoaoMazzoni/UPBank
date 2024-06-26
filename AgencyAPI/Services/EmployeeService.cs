using Models;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public class EmployeeService : IEmployeeService
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly IAddressService _addressService;

        public EmployeeService(IAddressService address)
        {
            _addressService = address;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var response = await _client.GetAsync("https://localhost:7196/api/employees");

            if (response.IsSuccessStatusCode)
            {
                var employees = await response.Content.ReadFromJsonAsync<List<Employee>>();
                foreach (var employee in employees)
                {
                    employee.Address = await _addressService.GetAddress(employee.AddressId);
                }
                return employees;
            }
            else
                return null;
        }

        public async Task<Employee> GetEmployee(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");
            cpf = cpf.Insert(3, ".").Insert(7, ".").Insert(11, "-");
            var response = await _client.GetAsync($"https://localhost:7040/api/Employees/{cpf}");

            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<Employee>();
                employee.Address = await _addressService.GetAddress(employee.AddressId);  
                return employee;
            }
            else
                return null;
        }

        public async Task<Employee> IfExistGetEmployeeOnAgency(string cpf)
        {
            var response = await _client.GetAsync($"https://localhost:7196/api/Employees/{cpf}");

            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<Employee>();
                employee.Address = await _addressService.GetAddress(employee.AddressId);
                return employee;
            }
            else
                return null;
        }

        public async Task<RemovedAgencyEmployee> GetEmployeeAgencyNumber(string cpf)
        {
            var response = await _client.GetAsync($"https://localhost:5001/api/employees/{cpf}");

            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<RemovedAgencyEmployee>();
                employee.Address = await _addressService.GetAddress(employee.AddressId);
                return employee;
            }
            else
                return null;
        }

        public async Task<Employee> PostEmployee(Employee employee)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("https://localhost:5001/api/employees", content);

                response.EnsureSuccessStatusCode();
                string employeeResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Employee>(employeeResponse);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<RemovedAgencyEmployee> PostEmployeeHistory(RemovedAgencyEmployee employee)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("https://localhost:5001/api/employees", content);

                response.EnsureSuccessStatusCode();
                string employeeResponse = await response.Content.ReadAsStringAsync();
                var emp = JsonConvert.DeserializeObject<RemovedAgencyEmployee>(employeeResponse);
                emp.Address = await _addressService.GetAddress(emp.AddressId);
                return emp;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Employee> DeleteEmployee(string document)
        {
            var response = await _client.DeleteAsync($"https://localhost:5001/api/employees/{document}");

            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<Employee>();
                employee.Address = await _addressService.GetAddress(employee.AddressId);
                return employee;
            }
            else
                return null;
        }

    }
}
