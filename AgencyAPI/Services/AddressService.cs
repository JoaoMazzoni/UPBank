using Models;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public class AddressService
    {
        private static readonly HttpClient _client = new HttpClient();

        public async Task<List<Address>> GetAddresses()
        {
            var response = await _client.GetAsync("https://localhost:5001/api/addresses");

            if (response.IsSuccessStatusCode)
            {
                var addresses = await response.Content.ReadFromJsonAsync<List<Address>>();
                return addresses;
            }
            else
                return null;
        }

        public async Task<Address> PostAddress(Address address)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(address), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync("https://localhost:5001/api/addresses", content);

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
