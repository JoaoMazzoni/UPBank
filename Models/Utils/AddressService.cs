using Models;
using Models.DTO;
using Models.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text;

namespace Models.Utils
{
    public class AddressService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<Address> GetAddressByAPI(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://localhost:7238/api/Addresses/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string addressReturn = await response.Content.ReadAsStringAsync();
                    JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new IgnoreJsonPropertyContractResolver() };
                    var address = JsonConvert.DeserializeObject<Address>(addressReturn, settings);
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

            var content = new StringContent(JsonConvert.SerializeObject(add), Encoding.UTF8, "application/json");
            HttpResponseMessage respose = await AddressService._httpClient.PostAsync("https://localhost:7238/api/Addresses", content);
            if (respose.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new ArgumentException("CEP Inválido. Erro ao obter endereço do serviço ViaCEP");
                
            }
            string addressReturn = await respose.Content.ReadAsStringAsync();
            JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new IgnoreJsonPropertyContractResolver() };
            var address = JsonConvert.DeserializeObject<Address>(addressReturn, settings);

            return address;
        }


    }
}
