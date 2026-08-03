using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfFrontEnd.Models;

namespace WpfFrontEnd.Services
{
    public class ConsumoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string BaseUrl;

        public ConsumoApiService()
        {
            _httpClient = new HttpClient();
            BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "consumos";
        }

        /// <summary>
        /// Retrieves all consumption records from the API.
        /// </summary>
        /// <returns>A list of consumption records. Returns an empty list if the request fails.</returns>
        public async Task<List<Consumo>> ObterConsumos()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Consumo>>(json);
            }
            return new List<Consumo>();
        }

        /// <summary>
        /// Creates a new consumption record in the API.
        /// </summary>
        /// <param name="consumo">The consumption record to be created.</param>
        /// <returns>True if the consumption record is created successfully; otherwise, false.</returns>
        public async Task<bool> CriarConsumo(Consumo consumo)
        {
            string json = JsonConvert.SerializeObject(consumo);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates an existing consumption record in the API.
        /// </summary>
        /// <param name="consumo">The consumption record to be updated.</param>
        /// <returns>True if the consumption record is updated successfully; otherwise, false.</returns>
        public async Task<bool> AlterarConsumo(Consumo consumo)
        {
            string json = JsonConvert.SerializeObject(consumo);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        /// <summary>
        /// Deletes an existing consumption record from the API.
        /// </summary>
        /// <param name="id">The identifier of the consumption record to be deleted.</param>
        /// <returns>True if the consumption record is deleted successfully; otherwise, false.</returns>
        public async Task<bool> EliminarConsumo(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
