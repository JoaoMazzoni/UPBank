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

        public T Get(string endpoint)
        {
            var response = _httpClient.GetAsync(endpoint).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            T objectReturn = JsonConvert.DeserializeObject<T>(json);
            return objectReturn;
        }

        public T Post(string endpoint, T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsJsonAsync(endpoint, content).Result;

            string strResponse = response.Content.ReadAsStringAsync().Result;

            T objReturn = JsonConvert.DeserializeObject<T>(strResponse);
            return objReturn;
        }
    }
}
