using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using Models.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CustomerAPI.Services
{
    public class AddressService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public Address ValidationAddress(string id)
        {
            //consume the company api
            try
            {
                var response = _httpClient.GetAsync($"https://localhost:7238/api/Addresses/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var address = JsonConvert.DeserializeObject<Address>(response.Content.ReadAsStringAsync().Result);
                    if (address != null)
                    {
                        return address;
                    }

                }
            }
            catch (HttpRequestException ex)
            {
                throw new ArgumentException("Erro ao enviar requisições para a API " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("API não encontrada " + ex.Message);
            }
            return null;
        }


        public async Task<Address> PostAddress(AddressDTO add)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(add), Encoding.UTF8, "application/json");
                HttpResponseMessage respose = await AddressService._httpClient.PostAsync("https://localhost:7238/api/Addresses", content);
                respose.EnsureSuccessStatusCode();
                string addressReturn = await respose.Content.ReadAsStringAsync();
                JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new IgnoreJsonPropertyContractResolver() };
                var address = JsonConvert.DeserializeObject<Address>(addressReturn, settings);
                return address;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
