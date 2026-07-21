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
    public class ConsumosController : ApiController
    {
        DataClassesDataContext dc = new DataClassesDataContext
            (ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);


        /// <summary>
        /// Get all consumos.
        /// </summary>
        /// <returns>List of consumos</returns>
        // GET api/<controller>
        public List<Consumo> Get()
        {
            var lista = from Consumo in dc.Consumos orderby Consumo.IdConsumo select Consumo;
            return lista.ToList();
        }


        /// <summary>
        /// Get a consumo by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The consumo if found.</returns>
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            var consumo = dc.Consumos.SingleOrDefault(c => c.IdConsumo == id);

            if (consumo != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, consumo));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Consumo não encontrado"));
        }

        /// <summary>
        /// Creates a new Consumo.
        /// </summary>
        /// <param name="novoConsumo">The consumo to create.</param>
        /// <returns>The created consumo.</returns>
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Consumo novoConsumo)
        {
            if (novoConsumo == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do consumo são Obrigatórios."));
            }
            ConsumoService consumoService = new ConsumoService(dc);

            ServiceResult resultado = consumoService.CriarConsumo(novoConsumo);

            if (!resultado.Sucesso)
            {
                return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
            }

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, novoConsumo));
        }

        /// <summary>
        /// Updates as existing consumo
        /// </summary>
        /// <param name="consumoAlterado">The consumo data to update.</param>
        /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Consumo consumoAlterado)
        {
            if (consumoAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do consumo são Obrigatórios."));
            }

            ConsumoService consumoService = new ConsumoService(dc);

            ServiceResult resultado = consumoService.AlterarConsumo(consumoAlterado);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
        }

        /// <summary>
        /// Deletes a consumo by Id.
        /// </summary>
        /// <param name="id">The Id of the consumo to delete.</param>
        /// <returns>The delete result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            ConsumoService consumoService = new ConsumoService(dc);

            ServiceResult resultado = consumoService.EliminarConsumo(id);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
        }
    }
}