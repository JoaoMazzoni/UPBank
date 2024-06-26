using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Newtonsoft.Json;
using System.Text;

namespace AgencyAPI.Services
{
    public class AddressService : IAddressService
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

                await _client.PostAsync("https://localhost:7238/api/Addresses", content);

                string id = address.ZipCode + address.Number;
                var add = GetAddressById(id);
                
                return add.Result;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Address> GetAddressById(string id)
        {
            var response = await _client.GetAsync($"https://localhost:7238/api/Addresses/{id}");

            if (response.IsSuccessStatusCode)
            {
                var address = await response.Content.ReadFromJsonAsync<Address>();
                return address;
            }
            else
                return null;
        }



        public async Task<Address> PutAddress(string zipCode, AddressDTO address)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(address), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync($"https://localhost:7238/api/Addresses/{zipCode}", content);

                response.EnsureSuccessStatusCode();
                string addressResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Address>(addressResponse);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
