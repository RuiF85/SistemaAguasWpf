using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WpfFrontEnd.Models;

namespace WpfFrontEnd.Services
{

    public class FaturaApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string BaseUrl;

        public FaturaApiService()
        {
            _httpClient = new HttpClient();
            BaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "faturas";
        }

        public async Task<List<Fatura>> ObterFaturas()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(BaseUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<List<Fatura>>(json);
            }
            return new List<Fatura>();
        }

        public async Task<bool> GerarFatura(int idConsumo)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/gerar/{idConsumo}", null);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AlterarEsatdo(Fatura fatura)
        {
            string json = JsonConvert.SerializeObject(fatura);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }
        public async Task<bool> AnularFatura(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
