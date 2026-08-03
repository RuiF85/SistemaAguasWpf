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

        /// <summary>
        /// Retrieves all counters from the API.
        /// </summary>
        /// <returns>A list of counters. Returns an empty list if the request fails.</returns>
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

        /// <summary>
        /// Creates a new counter by sending a POST request to the API.
        /// </summary>
        /// <param name="contador">The counter to be created.</param>
        /// <returns>True if the counter is created successfully; otherwise, false.</returns>
        public async Task<bool> CriarContador(Contador contador)
        {
            string json = JsonConvert.SerializeObject(contador);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates an existing counter by sending a PUT request to the API.
        /// </summary>
        /// <param name="contador">The counter to be updated.</param>
        /// <returns>True if the counter is updated successfully; otherwise, false.</returns>
        public async Task<bool> AlterarContador(Contador contador)
        {
            string json = JsonConvert.SerializeObject(contador);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        /// <summary>
        /// Deletes an existing counter from the API.
        /// </summary>
        /// <param name="id">The identifier of the counter to be deleted.</param>
        /// <returns>True if the counter is deleted successfully; otherwise, false.</returns>
        public async Task<bool> EliminarContador(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}