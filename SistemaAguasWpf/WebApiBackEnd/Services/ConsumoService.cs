using System;
using System.Linq;
using System.Net;
using WebApiBackEnd.Models;

namespace WebApiBackEnd.Services
{
    public class ConsumoService
    {

        private readonly DataClassesDataContext _dc;

        public ConsumoService(DataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Gets the previous meter reading.
        /// </summary>
        /// <param name="idContador"></param>
        /// <param name="data"></param>
        /// <param name="idConsumoIgnorar"></param>
        /// <returns></returns>
        public Consumo ObterLeituraAnterior(int idContador, DateTime data, int? idConsumoIgnorar = null)
        {
            var query = _dc.Consumos.Where(c => c.IdContador == idContador && c.Data < data);

            if (idConsumoIgnorar.HasValue)
            {
                query = query.Where(c => c.IdConsumo != idConsumoIgnorar.Value);
            }

            return query.OrderByDescending(c => c.Data).FirstOrDefault();
        }


        /// <summary>
        /// Gets the next meter reading.
        /// </summary>
        /// <param name="idContador"></param>
        /// <param name="data"></param>
        /// <param name="idConsumoIgnorar"></param>
        /// <returns></returns>
        public Consumo ObterLeituraSeguinte(int idContador, DateTime data, int idConsumoIgnorar)
        {
            return _dc.Consumos.Where(c => c.IdContador == idContador && c.Data > data && c.IdConsumo != idConsumoIgnorar)
                .OrderBy(c => c.Data).FirstOrDefault();
        }

        /// <summary>
        /// Calculates consumption based on the previous reading.
        /// </summary>
        /// <param name="leituraAtual"></param>
        /// <param name="consumoAnterior"></param>
        /// <returns></returns>
        public decimal CalcularConsumo(decimal leituraAtual, Consumo consumoAnterior)
        {
            if (consumoAnterior == null)
            {
                return leituraAtual;
            }

            return leituraAtual - consumoAnterior.LeituraAtual;
        }


        /// <summary>
        /// Creats a new consumption record.
        /// </summary>
        /// <param name="novoConsumo"></param>
        /// <returns></returns>
        public ServiceResult CriarConsumo(Consumo novoConsumo)
        {
            Contadore contador = _dc.Contadores.SingleOrDefault(c => c.IdContador == novoConsumo.IdContador);

            if (contador == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum contador com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (contador.Ativo == false)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não é possível registar consumos num contador inativo.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Cliente cliente = _dc.Clientes.SingleOrDefault(c => c.IdCLiente == contador.IdCliente);

            if (cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente associado ao contador não existe.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (cliente.Ativo == false)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não é possível registar consumos para um cliente inativo.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            if (novoConsumo.Data > DateTime.Today)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A data da leitura não pode ser futura.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            bool existeMesmaData = _dc.Consumos.Any(c => c.IdContador == novoConsumo.IdContador && c.Data == novoConsumo.Data);

            if (existeMesmaData)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Já existe uma leitura registada para esse contador nessa data.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Consumo consumoAnterior = ObterLeituraAnterior(novoConsumo.IdContador, novoConsumo.Data);

            if (consumoAnterior != null && novoConsumo.LeituraAtual < consumoAnterior.LeituraAtual)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A leitura atual não pode ser inferior à leitura anterior.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Consumo seguinte = _dc.Consumos.Where(c => c.IdContador == novoConsumo.IdContador && c.Data > novoConsumo.Data)
                .OrderBy(c => c.Data)
                .FirstOrDefault();

            if (seguinte != null && novoConsumo.LeituraAtual > seguinte.LeituraAtual)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A leitura atual não pode ser superior à leitura seguinte.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            novoConsumo.ConsumoCalculado = CalcularConsumo(novoConsumo.LeituraAtual, consumoAnterior);

            if (seguinte != null)
            {
                seguinte.ConsumoCalculado = seguinte.LeituraAtual - novoConsumo.LeituraAtual;
            }

            novoConsumo.IdFatura = null;

            _dc.Consumos.InsertOnSubmit(novoConsumo);

            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao registar o consumo.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Consumo registado com sucesso.",
                StatusCode = HttpStatusCode.Created
            };
        }


        /// <summary>
        /// Updates an existing consumption record.
        /// </summary>
        /// <param name="consumoAlterado"></param>
        /// <returns></returns>
        public ServiceResult AlterarConsumo(Consumo consumoAlterado)
        {
            Consumo consumo = _dc.Consumos.SingleOrDefault(c => c.IdConsumo == consumoAlterado.IdConsumo);

            if (consumo == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum consumo com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (consumo.IdFatura != null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Este consumo já foi faturado.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Contadore contador = _dc.Contadores.SingleOrDefault(c => c.IdContador == consumo.IdContador);

            if (contador == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O contador associado ao consumo não existe.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (contador.Ativo == false)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O contador encontra-se inativo.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Cliente cliente = _dc.Clientes.SingleOrDefault(c => c.IdCLiente == contador.IdCliente);

            if (cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente associado ao contador não existe.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (cliente.Ativo == false)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente encontra-se inativo.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            if (consumoAlterado.Data > DateTime.Today)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A data da leitura não pode ser futura.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            bool existeMesmaData = _dc.Consumos.Any(c => c.IdContador == consumo.IdContador && c.Data == consumoAlterado.Data &&
                c.IdConsumo != consumo.IdConsumo);

            if (existeMesmaData)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Já existe uma leitura para esse contador nessa data.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Consumo anterior = ObterLeituraAnterior(consumo.IdContador, consumoAlterado.Data, consumo.IdConsumo);

            if (anterior != null && consumoAlterado.LeituraAtual < anterior.LeituraAtual)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A leitura atual não pode ser inferior à leitura anterior.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            Consumo seguinte = ObterLeituraSeguinte(consumo.IdContador, consumoAlterado.Data, consumo.IdConsumo);

            if (seguinte != null && consumoAlterado.LeituraAtual > seguinte.LeituraAtual)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A leitura atual não pode ser superior à leitura seguinte.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            consumo.Data = consumoAlterado.Data;
            consumo.LeituraAtual = consumoAlterado.LeituraAtual;

            consumo.ConsumoCalculado = CalcularConsumo(consumoAlterado.LeituraAtual, anterior);

            if (seguinte != null)
            {
                seguinte.ConsumoCalculado = seguinte.LeituraAtual - consumoAlterado.LeituraAtual;
            }

            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao alterar o consumo.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Consumo alterado com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }



        /// <summary>
        /// Deletes a consumption record.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult EliminarConsumo(int id)
        {
            Consumo consumo = _dc.Consumos.SingleOrDefault(  c => c.IdConsumo == id);

            if (consumo == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum consumo com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (consumo.IdFatura != null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não é possível eliminar um consumo que já foi faturado.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            _dc.Consumos.DeleteOnSubmit(consumo);

            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao eliminar o consumo.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Consumo eliminado com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }

    }
}