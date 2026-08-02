using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfFrontEnd.Models;

namespace WpfFrontEnd.Services
{
    public class ContadorApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string BaseUrl;

        public ContadorApiService()
        {
            _httpClient = new HttpClient();
            BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "contadores";
        }

        public async Task<List<Contador>> ObterContadores()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Contador>>(json);
            }
            return new List<Contador>();
        }

        public async Task<bool> CriarContador(Contador contador)
        {
            string json = JsonConvert.SerializeObject(contador);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AlterarContador(Contador contador)
        {
            string json = JsonConvert.SerializeObject(contador);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }
        public async Task<bool> EliminarContador(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}