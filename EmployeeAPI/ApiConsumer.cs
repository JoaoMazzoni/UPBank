using Microsoft.AspNetCore.Mvc;
using Models.Utils;
using Newtonsoft.Json;
using System.Text;

namespace EmployeeAPI
{
    public class ApiConsumer<T>
    {
        private HttpClient _httpClient;
        private string _baseUrl;

        public ApiConsumer(string baseUrl)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
        }

        public async Task<T> Get(string endpoint, bool ignoreProperty)
        {
            var response = await _httpClient.GetAsync(_baseUrl + endpoint);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            T objectReturn;

            if (ignoreProperty)
                objectReturn = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings { ContractResolver = new IgnoreJsonPropertyContractResolver() });
            else
                objectReturn = JsonConvert.DeserializeObject<T>(json);

            return objectReturn;
        }

        public async Task<T> Post(string endpoint, T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsJsonAsync(_baseUrl + endpoint, content).Result;
            response.EnsureSuccessStatusCode();

            string strResponse = response.Content.ReadAsStringAsync().Result;

            T objReturn = JsonConvert.DeserializeObject<T>(strResponse);
            return objReturn;
        }
        public async Task<ActionResult<T>?> Patch(string endpoint)
        {
            try
            {
                var response = await _httpClient.PatchAsync(_baseUrl + endpoint, null);

                var strResponse = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                ActionResult<T>? objReturn = JsonConvert.DeserializeObject<ActionResult<T>>(strResponse);

                return objReturn;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
