using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiBackEnd.Models;
using WebApiBackEnd.Services;

namespace WebApiBackEnd.Controllers
{
    public class FaturasController : ApiController
    {

        DataClassesDataContext dc = new DataClassesDataContext
            (ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);


        /// <summary>
        /// Gets all invoices.
        /// </summary>
        /// <returns>list of invoices</returns>
        // GET api/<controller>
        // GET api/<controller>
        public IHttpActionResult Get()
        {
            var lista = from f in dc.Faturas
                        join c in dc.Clientes
                            on f.IdCliente equals c.IdCLiente
                        join ct in dc.Contadores
                            on f.IdContador equals ct.IdContador
                        join e in dc.EstadoFaturas
                            on f.IdEstadoFatura equals e.IdEstadoFatura
                        orderby f.IdFatura
                        select new
                        {
                            f.IdFatura,
                            f.IdCliente,
                            f.IdContador,
                            f.DataFatura,
                            f.Consumo,
                            f.ValorTotal,
                            f.IdEstadoFatura,

                            NomeCliente = c.Nome + " " + c.Apelido,
                            NumeroContador = ct.NumeroContador,
                            Estado = e.Descricao
                        };

            return Ok(lista.ToList());
        }


        /// <summary>
        /// Gets an invoice by Id.
        /// </summary>
        /// <param name="id">The Id of the invoice.</param>
        /// <returns>The invoice if found.</returns>
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            Fatura fatura = dc.Faturas.SingleOrDefault(f => f.IdFatura == id);

            if(fatura != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, fatura));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                   "Fatura não encontrada."));
        }

        /// <summary>
        /// Generates an invoice.
        /// </summary>
        /// <param name="idConsumos">Consumption Id.</param>
        /// <returns>The result.</returns>
        // POST api/<controller>
        [HttpPost]
        [Route("api/faturas/gerar/{idConsumo}")]
        public IHttpActionResult Post(int idConsumo)
        {
            TarifaService tarifaService = new TarifaService();
            FaturaService faturaService = new FaturaService(dc, tarifaService);

            ServiceResult resultado = faturaService.GerarFatura(idConsumo);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
         
        }

        /// <summary>
        /// Updates an invoice status
        /// </summary>
        /// <param name="faturaAlterada">Invoice to update</param>
        /// <returns>The resolt.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Fatura faturaAlterada)
        {
            if (faturaAlterada == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados da fatura são obrigatórios."));
            }

            TarifaService tarifaService = new TarifaService();
            FaturaService faturaService = new FaturaService(dc, tarifaService);

            ServiceResult resultado = faturaService.AlterarEstado(faturaAlterada);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
        }

        /// <summary>
        /// Cancels an invoice.
        /// </summary>
        /// <param name="id">Invoice Id.</param>
        /// <returns>The result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            TarifaService tarifaService = new TarifaService();
            FaturaService faturaService = new FaturaService(dc, tarifaService);

            ServiceResult resultado = faturaService.AnularFatura(id);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
        }
    }
}