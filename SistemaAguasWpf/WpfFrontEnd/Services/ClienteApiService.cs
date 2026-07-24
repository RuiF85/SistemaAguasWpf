using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfFrontEnd.Models;

namespace WpfFrontEnd.Services
{
    public class ClienteApiService
    {
        private readonly HttpClient _httpClient;

        private readonly string BaseUrl;

        public ClienteApiService()
        {
            _httpClient = new HttpClient();
            BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "clientes";
        }

        public async Task<List<Cliente>> ObterClientes()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Cliente>>(json);
            }
            return new List<Cliente>();

        }

        public async Task<bool> CriarCliente(Cliente cliente)
        {
            string json = JsonConvert.SerializeObject(cliente);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AlterarCliente(Cliente cliente)
        {
            string json = JsonConvert.SerializeObject(cliente);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        public async Task<bool> EliminarCliente(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}

