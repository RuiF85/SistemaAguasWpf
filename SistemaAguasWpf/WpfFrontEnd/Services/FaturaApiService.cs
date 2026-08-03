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

        /// <summary>
        /// Retrieves all invoices from the API.
        /// </summary>
        /// <returns>A list of invoices. Returns an empty list if the request fails.</returns>
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

        /// <summary>
        /// Generates a new invoice based on the specified consumption ID.
        /// </summary>
        /// <param name="idConsumo">The ID of the consumption for which to generate an invoice.</param>
        /// <returns>True if the invoice is generated successfully; otherwise, false.</returns>
        public async Task<bool> GerarFatura(int idConsumo)
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"{BaseUrl}/gerar/{idConsumo}", null);

            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Updates the status of an existing invoice.
        /// </summary>
        /// <param name="fatura">The invoice with the updated status.</param>
        /// <returns>True if the invoice status is updated successfully; otherwise, false.</returns>
        public async Task<bool> AlterarEstado(Fatura fatura)
        {
            string json = JsonConvert.SerializeObject(fatura);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(BaseUrl, content);

            return response.IsSuccessStatusCode;

        }

        /// <summary>
        /// Annulls an existing invoice.
        /// </summary>
        /// <param name="id">The ID of the invoice to be annulled.</param>
        /// <returns>True if the invoice is annulled successfully; otherwise, false.</returns>
        public async Task<bool> AnularFatura(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");

            return response.IsSuccessStatusCode;
        }
    }
}
