using System;
using System.Linq;
using System.Net;
using WebApiBackEnd.Models;

namespace WebApiBackEnd.Services
{
    public class FaturaService
    {
        private readonly DataClassesDataContext _dc;
        private readonly TarifaService _tarifaService;

        public FaturaService(DataClassesDataContext dc, TarifaService tarifaService)
        {
            _dc = dc;
            _tarifaService = tarifaService;
        }

        public ServiceResult GerarFatura(int idConsumo)
        {
            Consumo consumo = _dc.Consumos.SingleOrDefault(c => c.IdConsumo == idConsumo);

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
                    Mensagem = "Este consumo já possui uma fatura associada.",
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

            EstadoFatura estadoPendente = _dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == 0);

            if (estadoPendente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O estado inicial da fatura não está configurado.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            decimal valorTotal = _tarifaService.CalcularValorFatura(consumo.ConsumoCalculado);

            Fatura novaFatura = new Fatura
            {
                IdCliente = cliente.IdCLiente,
                IdContador = contador.IdContador,
                DataFatura = DateTime.Now,
                Consumo = consumo.ConsumoCalculado,
                ValorTotal = valorTotal,
                IdEstadoFatura = 0
            };

            _dc.Faturas.InsertOnSubmit(novaFatura);

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Fatura gerada com sucesso.",
                StatusCode = HttpStatusCode.Created
            };
        }

        public ServiceResult AlterarEstado(Fatura faturaAlterada)
        {
            Fatura fatura = _dc.Faturas.SingleOrDefault(f => f.IdFatura == faturaAlterada.IdFatura);

            if (fatura == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhuma fatura com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            EstadoFatura estado = _dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == faturaAlterada.IdEstadoFatura);

            if (estado == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O estado indicado para a fatura não existe.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (fatura.IdEstadoFatura == 2)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não é possível alterar o estado de uma fatura anulada.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            if (faturaAlterada.IdEstadoFatura == 2)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Para anular uma fatura deve utilizar a operação de anulação.",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            fatura.IdEstadoFatura = faturaAlterada.IdEstadoFatura;

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Fatura alterada com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }

        public ServiceResult AnularFatura(int id)
        {
            Fatura fatura = _dc.Faturas.SingleOrDefault(f => f.IdFatura == id);

            if (fatura == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhuma fatura com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            if (fatura.IdEstadoFatura == 2)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "A fatura já se encontra anulada.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            EstadoFatura estadoAnulada = _dc.EstadoFaturas.SingleOrDefault(e => e.IdEstadoFatura == 2);

            if (estadoAnulada == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O estado Anulada não está configurado.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            Consumo consumo = _dc.Consumos.SingleOrDefault(c => c.IdFatura == id);

            if (consumo != null)
            {
                consumo.IdFatura = null;
            }

            fatura.IdEstadoFatura = 2;

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Fatura anulada com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}