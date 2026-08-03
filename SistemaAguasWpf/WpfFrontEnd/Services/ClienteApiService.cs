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

        /// <summary>
        /// Retrieves all clients from the API.
        /// </summary>
        /// <returns>A list of clients. Returns an empty list if the request fails.</returns>
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

        /// <summary>
        /// Creates a new client in the API.
        /// </summary>
        /// <param name="cliente">The client object to be created.</param>
        /// <returns>True if the client is created successfully; otherwise, false.</returns>
        public async Task<bool> CriarCliente(Cliente cliente)
        {
            string json = JsonConvert.SerializeObject(cliente);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        ///  Updates an existing client in the API.
        /// </summary>
        /// <param name="cliente">The client object to be updated.</param>
        /// <returns>True if the client is updated successfully; otherwise, false.</returns>
        public async Task<bool> AlterarCliente(Cliente cliente)
        {
            string json = JsonConvert.SerializeObject(cliente);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        /// <summary>
        /// Excluir um cliente existente na API
        /// </summary>
        /// <param name="id">The client identifier.</param>
        /// <returns>True if the client is deleted successfully; otherwise, false.</returns>
        public async Task<bool> EliminarCliente(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}

