using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace WebApiBackEnd.Models
{
    public class ServiceResult
    {
        public bool Sucesso { get; set; }

        public string Mensagem { get; set; }    

        public HttpStatusCode StatusCode { get; set; }

    }
}