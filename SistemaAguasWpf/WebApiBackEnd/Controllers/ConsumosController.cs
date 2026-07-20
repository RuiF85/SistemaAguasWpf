using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


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

            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == novoConsumo.IdContador);

            if (contador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum contador com esse Id."));
            }

            if(contador.Ativo == false)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possivel registar consumos num contador inativo."));
            }

            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == contador.IdCliente);

            if(cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "O cliente associado ao contador não existe."));
            }

            if (cliente.Ativo == false)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possivel registar consumos para um cliente inativo."));
            }

            Consumo consumoMesmaData = dc.Consumos.FirstOrDefault(
                c => c.IdContador == novoConsumo.IdContador && c.Data == novoConsumo.Data);

            if(consumoMesmaData != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe uma leitura registada para esse contador nessa data."));
            }

            if (novoConsumo.Data > DateTime.Today)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "A data da leitura não pode ser futura."));
            }

            Consumo ultimoConsumo = dc.Consumos.Where(c => c.IdContador == novoConsumo.IdContador
            && c.Data < novoConsumo.Data).OrderByDescending(c=>c.Data).FirstOrDefault();

            if(ultimoConsumo != null)
            {
                if(novoConsumo.LeituraAtual < ultimoConsumo.LeituraAtual)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                     "A leitura atual não pode ser inferior à leitura anterior."));
                }
                novoConsumo.ConsumoCalculado = novoConsumo.LeituraAtual - ultimoConsumo.LeituraAtual;
            }
            else
            {
                novoConsumo.ConsumoCalculado = novoConsumo.LeituraAtual;
            }


            novoConsumo.IdFatura = null;
            dc.Consumos.InsertOnSubmit(novoConsumo);

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(
                    HttpStatusCode.InternalServerError, e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Created, novoConsumo));
        }

        /// <summary>
        /// Updates as existing consumo
        /// </summary>
        /// <param name="consumoAlterado">The consumo data to update.</param>
        /// <returns>The update result.</returns>
        // PUT api/<controller>/5
        public IHttpActionResult Put([FromBody] Consumo consumoAlterado)
        {
            if(consumoAlterado == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Os dados do consumo são obrigatórios."));
            }

            Consumo consumo = dc.Consumos.SingleOrDefault(c => c.IdConsumo == consumoAlterado.IdConsumo);

            if(consumo == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum consumo com esse Id para poder alterar."));
            }

            if(consumo.IdFatura != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Este consumo já foi faturado. Deve anular a fatura antes de alterar a leitura"));
            }

            Contadore contador = dc.Contadores.SingleOrDefault(c => c.IdContador == consumoAlterado.IdContador);

            if( contador == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum contador com esse Id."));
            }

            if(contador.Ativo == false)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possivel alterar consumos para um contador inativo."));
            }

            Cliente cliente = dc.Clientes.SingleOrDefault(c => c.IdCLiente == contador.IdCliente);

            if(cliente == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound,
                "O cliente associado ao contador não existe."));
            }

            if ( cliente.Ativo == false)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possivel alterar consumos para um cliente inativo."));
            }

            if(consumoAlterado.Data > DateTime.Today)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "A data da leitura não pode ser futura."));
            }

            Consumo consumoMesmaData = dc.Consumos.FirstOrDefault(c => c.IdContador == consumoAlterado.IdContador
            && c.Data == consumoAlterado.Data
            && c.IdConsumo != consumoAlterado.IdConsumo);

            if ( consumoMesmaData !=  null )
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Já existe uma leitura registada para esse contador nessa data."));
            }


            Consumo ultimoConsumo = dc.Consumos.Where(c => c.IdContador == consumoAlterado.IdContador
            && c.Data < consumoAlterado.Data
            && c.IdConsumo != consumoAlterado.IdConsumo)
                .OrderByDescending(c => c.Data) .FirstOrDefault();

            if (ultimoConsumo != null)
            {
                if (consumoAlterado.LeituraAtual < ultimoConsumo.LeituraAtual)
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                        "A leitura atual não pode ser inferior à leitura anterior."));
                }

                consumo.ConsumoCalculado = consumoAlterado.LeituraAtual - ultimoConsumo.LeituraAtual;
            }
            else
            {
                consumo.ConsumoCalculado = consumoAlterado.LeituraAtual;
            }

            consumo.IdContador = consumoAlterado.IdContador;
            consumo.Data = consumoAlterado.Data;
            consumo.LeituraAtual = consumoAlterado.LeituraAtual;

            try
            {
                dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse( HttpStatusCode.InternalServerError,
                    e.Message));
            }

            return ResponseMessage(Request.CreateResponse( HttpStatusCode.OK,
                    "Consumo alterado com sucesso."));

        }


        /// <summary>
        /// Deletes a consumo by Id.
        /// </summary>
        /// <param name="id">The Id of the consumo to delete.</param>
        /// <returns>The delete result.</returns>
        // DELETE api/<controller>/5
        public IHttpActionResult Delete(int id)
        {
            Consumo consumo = dc.Consumos.SingleOrDefault(c => c.IdConsumo == id);

            if(consumo == null)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Não existe nenhum consumo com esse Id."));
            }
            if(consumo.IdFatura != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "Não é possivel eliminar um consumo que já foi faturado."));
            }

            dc.Consumos.DeleteOnSubmit(consumo);

            try
            {
                dc.SubmitChanges();
            }
            catch(Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.InternalServerError,e.Message));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK,
                "Consumo eliminado com sucesso."));
        }
    }
}