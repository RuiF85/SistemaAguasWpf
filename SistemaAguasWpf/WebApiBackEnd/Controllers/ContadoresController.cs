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
            if (novoContador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Tem que introduzir os dados do contador."));
            }

            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == novoContador.IdCliente);

            if (cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum cliente com esse Id."));
            }

            Contadore contadorMesmoNumero = dc.Contadores.FirstOrDefault(c => c.NumeroContador == novoContador.NumeroContador);

            if ( contadorMesmoNumero != null )
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um contador registado com esse número."));
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
            if(contadorAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do contador são obrigatórios"));
            }


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
                    "Não existe nenhum cliente com esse Id "));
            }

            Contadore contadorMesmoNumero = dc.Contadores.FirstOrDefault(
                c => c.NumeroContador == contadorAlterado.NumeroContador && c.IdContador != contadorAlterado.IdContador);

            if(contadorMesmoNumero != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe outro contador registado com esse número"));
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

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Contador alterado com sucesso"));

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

            if (contador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum contador com esse Id para poder eliminar"));
            }

            var temConsumos = dc.Consumos.Any(c => c.IdContador == id);

            if (temConsumos)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "O contador não pode ser eliminado porque possui consumos associados"));
            }

            var temFaturas = dc.Faturas.Any( f => f.IdContador == id);
            if (temFaturas)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "" +
                    "O contador não pode ser eliminado porque possui Faturas associadas"));
            }

            dc.Contadores.DeleteOnSubmit(contador);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message));
            }


            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Contador eliminado com sucesso"));
        }
    }
}