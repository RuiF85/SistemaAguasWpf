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
    public class ClientesController : ApiController
    {

        DataClassesDataContext dc = new DataClassesDataContext
            (ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);


        /// <summary>
        /// Gets all clients.
        /// </summary>
        /// <returns>List of all Clients.</returns>
        // GET api/<controller>
        public List<Cliente> Get()
        {
            var lista = from Cliente in dc.Clientes orderby Cliente.IdCLiente select Cliente;
            return lista.ToList();
        }


        /// <summary>
        /// Gets a client by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The Id of the client.</returns>
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            var cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == id);

            if (cliente != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, cliente));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Cliente não encontrado."));

        }


        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="novoCliente">The client data to create.</param>
        /// <returns>The creation result.</returns>
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Cliente novoCliente)
        {

            if (novoCliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do cliente são obrigatórios."));
            }
            ClienteService ClienteService = new ClienteService(dc);

            ServiceResult resultado = ClienteService.CriarCliente(novoCliente);

            if (!resultado.Sucesso)
            {
                return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, novoCliente));

        }


        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <param name="clienteAlterado">The client data to update.</param>
        /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Cliente clienteAlterado)
        {

            if (clienteAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do cliente são obrigatórios."));
            }

            ClienteService clienteService = new ClienteService(dc);

            ServiceResult resultado = clienteService.AlterarCliente(clienteAlterado);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));

        }

        /// <summary>
        /// Deletes a client by Id.
        /// </summary>
        /// <param name="id">The Id of the client to delete.</param>
        /// <returns>The delete result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            ClienteService clienteService = new ClienteService(dc);

            ServiceResult resultado = clienteService.EliminarCliente(id);

            return ResponseMessage(Request.CreateResponse(resultado.StatusCode, resultado.Mensagem));

        }
    }
}