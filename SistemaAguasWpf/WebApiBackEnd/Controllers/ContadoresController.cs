using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


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
            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == novoContador.IdContador);

            if (contador != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um contador resgistado com esse Id"));
            }
            dc.Contadores.InsertOnSubmit(novoContador);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(
                    HttpStatusCode.InternalServerError, e.Message));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, novoContador));
        }

        /// <summary>
        /// Updates an existing counter.
        /// </summary>
        /// <param name="contadorAlterado"></param>
        /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Contadore contadorAlterado)
        {
            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == contadorAlterado.IdContador);

            if (contador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum contador com esse Id para poder alterar"));
            }


            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == contadorAlterado.IdCliente);

            if (cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum cliente com esse Id para poder alterar"));
            }
            contador.IdCliente = contadorAlterado.IdCliente;
            contador.NumeroContador = contadorAlterado.NumeroContador;
            contador.DataInstalacao = contadorAlterado.DataInstalacao;
            contador.Ativo = contadorAlterado.Ativo;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(
                    HttpStatusCode.ServiceUnavailable, e));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));

        }
        /// <summary>
        /// Deletes a counter by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The delete result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == id);

            if (contador != null)
            {
                dc.Contadores.DeleteOnSubmit(contador);
                try
                {
                    dc.SubmitChanges();
                }
                catch (Exception e)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
                }

                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                "Não existe nenhum contador com esse Id para poder eliminar"));
        }
    }
}