using System;
using System.Collections.Generic;
using System.Configuration;
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

            if (novoCliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Os dados do cliente são obrigatórios-"));
            }

            Cliente clienteMesmoNif = dc.Clientes.FirstOrDefault(c => c.Nif == novoCliente.Nif);

            if (clienteMesmoNif != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um Cliente registado com esse Nif"));
            }

            if (!string.IsNullOrWhiteSpace(novoCliente.Email))
            {

                Cliente clienteMesmoEmail = dc.Clientes.FirstOrDefault(c => c.Email == novoCliente.Email);

                if (clienteMesmoEmail != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "Já existe um Cliente  com esse Email"));
                }

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

            if (clienteAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do cliente são obrigatórios"));
            }

            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.IdCLiente == clienteAlterado.IdCLiente);

            if (cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum Cliente com esse Id para poder alterar"));
            }

            Cliente clienteMesmoNif = dc.Clientes.FirstOrDefault(
                c => c.Nif == clienteAlterado.Nif && c.IdCLiente != clienteAlterado.IdCLiente);

            if (clienteMesmoNif != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe um Cliente registado com esse Nif"));
            }

            if (!string.IsNullOrWhiteSpace(clienteAlterado.Email))
            {

                Cliente clienteMesmoEmail = dc.Clientes.FirstOrDefault(
                    c => c.Email == clienteAlterado.Email && c.IdCLiente != clienteAlterado.IdCLiente);

                if (clienteMesmoEmail != null)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "Já existe um Cliente  com esse Email"));
                }

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

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Cliente alterado com sucesso"));

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            Cliente cliente = dc.Clientes.FirstOrDefault(c => c.IdCLiente == id);

            if (cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum Cliente com esse Id para poder eliminar"));
            }

            var temFaturas = dc.Faturas.Any(f => f.IdCliente == id);

            if (temFaturas)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "O cliente não pode ser eliminado porque possui faturas associadas"));
            }

            var temConsumos = dc.Consumos.Any(consumo => dc.Contadores.Any
            (contador => contador.IdContador == consumo.IdContador && contador.IdCliente == id));

            if (temConsumos)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "O cliente não pode ser eliminado porque possui consumos associados"));
            }

            dc.Clientes.DeleteOnSubmit(cliente);
            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                   "Cliente eliminado com sucesso"));
        }
    }
}