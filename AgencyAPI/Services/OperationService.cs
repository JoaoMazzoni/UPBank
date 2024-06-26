using Models;
using System.Reflection.Emit;

namespace AgencyAPI.Services
{
    public class OperationService : IOperationService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Operation>> GetOperationsByTypeLoan()
        {
            var response = await _client.GetAsync("https://localhost:7285/api/Operations/type/" + 2);

            if (response.IsSuccessStatusCode)
            {
                List<Operation> operations = await response.Content.ReadFromJsonAsync<List<Operation>>();

                if (operations != null)
                    return operations;
                else
                    return null;
            }
            else
                return null;
        }
    }
}
