using System.Net;

namespace WebApiBackEnd.Models
{
    public class ServiceResult
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }

        public HttpStatusCode StatusCode { get; set; }

    }
}