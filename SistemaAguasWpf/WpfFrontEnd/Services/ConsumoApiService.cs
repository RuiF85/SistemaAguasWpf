using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public async Task<bool> CriarConsumo(Consumo consumo)
        {
            string json = JsonConvert.SerializeObject(consumo);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AlterarConsumo(Consumo consumo)
        {
            string json = JsonConvert.SerializeObject(consumo);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        public async Task<bool> EliminarConsumo(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }

    }
}
