using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using WebApiBackEnd.Models;

namespace WebApiBackEnd.Services
{
    public class ContadorService
    {
        private readonly DataClassesDataContext _dc;

        public ContadorService(DataClassesDataContext dc)
        {
            _dc = dc;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="novoContador"></param>
        /// <returns></returns>
        public ServiceResult CriarContador(Contadore novoContador)
        {
            Cliente cliente = _dc.Clientes.SingleOrDefault(c => c.IdCLiente == novoContador.IdCliente);
            
            if(cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum Cliente com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool numeroDuplicado = _dc.Contadores.Any(c => c.NumeroContador == novoContador.NumeroContador);

            if (numeroDuplicado)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem ="Já existe um contador registado com esse número",
                    StatusCode = HttpStatusCode.Conflict
                };
            }
            _dc.Contadores.InsertOnSubmit(novoContador);

            try
            {
                _dc.SubmitChanges();
            }
            catch (Exception e)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao criar o contador",
                    StatusCode = HttpStatusCode.InternalServerError
                };

            }
            return new ServiceResult
            {
                Sucesso =true,
                Mensagem = "Contador criado com sucesso.",
                StatusCode = HttpStatusCode.Created
            };

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="contadorAlterado"></param>
        /// <returns></returns>
        public ServiceResult AlterarContador(Contadore contadorAlterado)
        {
            Contadore contador = _dc.Contadores.SingleOrDefault(c=> c.IdContador == contadorAlterado.IdContador);

            if(contador == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum contador com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }
          
            Cliente cliente = _dc.Clientes.SingleOrDefault(c => c.IdCLiente == contadorAlterado.IdCliente);

            if(cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum cliente com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool numeroDuplicado = _dc.Contadores.Any(c => c.NumeroContador == contadorAlterado.NumeroContador
            && c.IdContador != contadorAlterado.IdContador);

            if (numeroDuplicado)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Já existe outro contador registado com esse número",
                    StatusCode= HttpStatusCode.Conflict
                };
            }

            contador.IdCliente = contadorAlterado.IdCliente;
            contador.NumeroContador = contadorAlterado.NumeroContador;
            contador.DataInstalacao = contadorAlterado.DataInstalacao;
            contador.Ativo = contadorAlterado.Ativo;



            try
            {
                _dc.SubmitChanges();
            }
            catch 
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao alterar o contador.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Contador alterado com sucesso.",
                StatusCode = HttpStatusCode.OK
            };

        }

        public ServiceResult ApagarContador(int id)
        {
            Contadore contador = _dc.Contadores.SingleOrDefault(c => c.IdContador == id) ;

            if (contador == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum contador com esse Id",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool temConsumos = _dc.Consumos.Any(c => c.IdContador == id);

            if (temConsumos)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O contador não pode ser eliminado porque possui consumos associados.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            bool temFaturas = _dc.Faturas.Any(c => c.IdContador  == id);

            if (temFaturas)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O contador não pode ser eliminado porque possui faturas associadas.",
                    StatusCode = HttpStatusCode.Conflict
                };

            }

            _dc.Contadores.DeleteOnSubmit(contador);

            try
            {
                _dc.SubmitChanges();
            }
            catch 
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao eliminar o contador.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Contador eliminado com sucesso",
                StatusCode = HttpStatusCode.OK
            };

        }
    }
}