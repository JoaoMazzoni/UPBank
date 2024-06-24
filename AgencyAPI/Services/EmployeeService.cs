using Models;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public class EmployeeService
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly AddressService _addressService;

        public EmployeeService(AddressService address)
        {
            _addressService = address;
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/employees");

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
    }
}
