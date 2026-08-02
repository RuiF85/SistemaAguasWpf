using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using WpfFrontEnd.Models;

namespace WpfFrontEnd.Services
{
    public class EstadoFaturaApiService
    {

        private readonly HttpClient httpClient = new HttpClient();
        private readonly string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"] + "estadofaturas";

        public async Task<List<EstadoFatura>> ObterEstados()
        {
            HttpResponseMessage response = await httpClient.GetAsync(baseUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<EstadoFatura>>(json);
            }

            return new List<EstadoFatura>();
        }
    }
}
