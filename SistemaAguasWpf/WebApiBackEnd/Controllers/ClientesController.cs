using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiBackEnd.Controllers
{
    public class ClientesController : ApiController
    {

        DataClassesDataContext dc = new DataClassesDataContext
            (ConfigurationManager.ConnectionStrings["SistemaAguasConnectionString"].ConnectionString);


        /// <summary>
        /// Gets all clients.
        /// </summary>
        /// <returns>List  of Clients.</returns>
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
        /// <returns>The client if found.</returns>
        // GET api/<controller>/5
        public IHttpActionResult Get(int id)
        {
            var cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == id);

            if (cliente != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, cliente));
            }
            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Cliente não encontrado"));

        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="novoCliente"></param>
        /// <returns>The created client.</returns>
        // POST api/<controller>
        public IHttpActionResult Post([FromBody] Cliente novoCliente)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.IdCLiente == novoCliente.IdCLiente);

            if (cliente != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, 
                    "Já existe um Cliente registado com esse Id"));
            }

            dc.Clientes.InsertOnSubmit(novoCliente);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(
                    HttpStatusCode.InternalServerError, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, novoCliente));
        }
       /// <summary>
       /// Updates an existing client.
       /// </summary>
       /// <param name="clienteAlterado"></param>
       /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Cliente clienteAlterado)
        {
          Cliente cliente = dc.Clientes.FirstOrDefault(c => c.IdCLiente == clienteAlterado.IdCLiente);
        
             if(cliente == null)
             {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum Cliente com esse Id para poder alterar"));
             }
            
            cliente.Nome = clienteAlterado.Nome;
            cliente.Apelido = clienteAlterado.Apelido;
            cliente.Morada = clienteAlterado.Morada;
            cliente.Nif = clienteAlterado.Nif;
            cliente.Contacto = clienteAlterado.Contacto;
            cliente.Email = clienteAlterado.Email;
            cliente.CodigoPostal = clienteAlterado.CodigoPostal;
            cliente.Localidade = clienteAlterado.Localidade;
            cliente.Ativo = clienteAlterado.Ativo;

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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<controller>/5
      
        public IHttpActionResult Delete(int id)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault( c => c.IdCLiente == id);

            if (cliente != null)
            {
                dc.Clientes.DeleteOnSubmit(cliente);
                try
                {
                    dc.SubmitChanges();
                }
                catch(Exception e)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
                }
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                   "Não existe nenhum Cliente com esse Id para poder eliminar"));
        }
    }
}