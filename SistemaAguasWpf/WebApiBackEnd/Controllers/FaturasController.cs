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
        public List<Fatura> Get()
        {
            var lista = from Fatura in dc.Faturas orderby Fatura.IdFatura select Fatura;

            return lista.ToList();
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

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] GerarFaturaDto dados)
        {


            if (dados == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados para gerar a fatura são obrigatórios."));
            }
            
            Consumo consumo = dc.Consumos.SingleOrDefault(c => c.IdConsumo == dados.IdConsumo);

            if(consumo == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum consumo com esse Id."));
            }
           
            if(consumo.IdFatura != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Este consumo já possui uma fatura associada."));
            }

            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == consumo.IdContador);

            if(contador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "O contador associado ao consumo não existe."));
            }

            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == contador.IdCliente);

            if(cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                "O cliente associado ao contador não existe."));
            }

            EstadoFatura estado = dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == 0);
            if(estado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,
                "O estado inicial da fatura não está configurado."));
            }


            TarifaService tarifaService = new TarifaService();
            decimal valorTotal = tarifaService.CalcularValorFatura(consumo.ConsumoCalculado);

            Fatura novaFatura = new Fatura();

            novaFatura.IdCliente = cliente.IdCLiente;
            novaFatura.IdContador = contador.IdContador;
            novaFatura.DataFatura = DateTime.Now;
            novaFatura.Consumo = consumo.ConsumoCalculado;
            novaFatura.ValorTotal = valorTotal;

            novaFatura.IdEstadoFatura = 0;
            dc.Faturas.InsertOnSubmit(novaFatura);

            try
            {
                dc.SubmitChanges();
                consumo.IdFatura = novaFatura.IdFatura;
                dc.SubmitChanges();
            }
            catch(Exception e) 
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, novaFatura));
        }

        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Fatura faturaAlterada)
        {
            if (faturaAlterada == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados da fatura são obrigatórios."));
            }

            Fatura fatura = dc.Faturas.SingleOrDefault(f => f.IdFatura == faturaAlterada.IdFatura);

            if (fatura == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhuma fatura com esse Id."));
            }

            EstadoFatura estado = dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == faturaAlterada.IdEstadoFatura);

            if (estado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "O estado indicado para a fatura não existe."));
            }

            if (fatura.IdEstadoFatura == 2)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possível alterar o estado de uma fatura anulada."));
            }

            if (faturaAlterada.IdEstadoFatura == 2)
            {
                return ResponseMessage(Request.CreateResponse( HttpStatusCode.BadRequest,
                    "Para anular uma fatura deve utilizar a operação de anulação."));
            }

            fatura.IdEstadoFatura = faturaAlterada.IdEstadoFatura;

            try
            {
                dc.SubmitChanges();
                
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Fatura alterada com sucesso."));
        }

        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            Fatura fatura = dc.Faturas.SingleOrDefault(f => f.IdFatura == id);

            if (fatura == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhuma fatura com esse Id."));
            }

            if (fatura.IdEstadoFatura == 2)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "A fatura já se encontra anulada."));

            }

            Consumo consumo = dc.Consumos.SingleOrDefault(c => c.IdFatura == id);

            if (consumo != null)
            {
                consumo.IdFatura = null;
            }


            EstadoFatura estadoAnulada = dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == 2);

            if (estadoAnulada == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,
                    "O estado Anulada não está configurado."));
            }

            fatura.IdEstadoFatura = 2;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(
                    HttpStatusCode.InternalServerError,
                    e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Fatura anulada com sucesso."));
        }
    }
}