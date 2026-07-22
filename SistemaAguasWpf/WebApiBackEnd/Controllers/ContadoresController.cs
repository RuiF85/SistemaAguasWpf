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
    public class ContadoresController : ApiController
    {

        DataClassesDataContext dc = new DataClassesDataContext(
            ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);


        /// <summary>
        /// Get all Contador
        /// </summary>
        /// <returns>List of counters.</returns>
        // GET api/<controller>

        public List<Contadore> Get()
        {
            var lista = from Contador in dc.Contadores orderby Contador.IdContador  select Contador;
            return lista.ToList();
        }


        /// <summary>
        /// Gets a counter by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The counter if found..</returns>
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            var contador = dc.Contadores.SingleOrDefault(c => c.IdContador == id);

            if (contador != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, contador));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Contador não encontrado")); ;
        }



        /// <summary>
        /// Creates a new counter.
        /// </summary>
        /// <param name="novoContador"></param>
        /// <returns>The created counter.</returns>
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Contadore novoContador)
        {
            if (novoContador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Tem que introduzir os dados do contador."));
            }

            ContadorService contadorService = new ContadorService(dc);

            ServiceResult resultado = contadorService.CriarContador(novoContador);

            if (!resultado.Sucesso)
            {
                return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
            }
            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, novoContador));
        }


        /// <summary>
        /// Updates an existing counter.
        /// </summary>
        /// <param name="contadorAlterado"></param>
        /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Contadore contadorAlterado)
        {
            if(contadorAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do contador são obrigatórios"));
            }

            ContadorService contadorService = new ContadorService(dc);
            
            ServiceResult resultado = contadorService.AlterarContador(contadorAlterado);

            if (!resultado.Sucesso)
            {
                return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
            }
            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, contadorAlterado));

        }


        /// <summary>
        /// Deletes a counter by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The delete result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
           ContadorService contadorService = new ContadorService(dc);

            ServiceResult resultado = contadorService.ApagarContador(id);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
        }
    }
}