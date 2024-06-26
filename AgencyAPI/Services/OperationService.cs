using Models;
using System.Reflection.Emit;

namespace AgencyAPI.Services
{
    public class OperationService : IOperationService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Operation>> GetOperationsByTypeLoan(string account)
        {
            var response = await _client.GetAsync("https://localhost:7285/api/Operations/" + account + "/3");

            if (response.IsSuccessStatusCode)
            {
                List<Operation> operations = new();

                var operationsList = await response.Content.ReadFromJsonAsync<List<Operation>>();

                foreach (var operation in operationsList)
                {
                    if (operation.Type.Equals("Transfer"))
                        operations.Add(operation);
                }

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
