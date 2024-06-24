using Models;
using Models.DTO;
using Newtonsoft.Json;
using System.Runtime.ConstrainedExecution;
using System.Text;

namespace AgencyAPI.Services
{
    public class AddressService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Address>> GetAddresses()
        {
            var response = await _client.GetAsync("https://localhost:7238");

            if (response.IsSuccessStatusCode)
            {
                var addresses = await response.Content.ReadFromJsonAsync<List<Address>>();
                return addresses;
            }
            else
                return null;
        }

        public async Task<Address> GetAddress(string zipCode)
        {
            var response = await _client.GetAsync($"https://localhost:7238/api/Addresses/{zipCode}");

            if (response.IsSuccessStatusCode)
            {
                var address = await response.Content.ReadFromJsonAsync<Address>();
                return address;
            }
            else
                return null;
        }

        public async Task<Address> PostAddress(AddressDTO address)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(address), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("https://localhost:7238/api/Addresses", content);

                response.EnsureSuccessStatusCode();
                string addressResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Address>(addressResponse);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
