using System.Linq;
using System.Net;
using WebApiBackEnd.Models;

namespace WebApiBackEnd.Services
{
    public class ClienteService
    {
        private readonly DataClassesDataContext _dc;

        public ClienteService(DataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Creates a new client after validationg the Nif and email.
        /// </summary>
        /// <param name="novoCliente">The client to crate.</param>
        /// <returns>The result of the create operation.</returns>
        public ServiceResult CriarCliente(Cliente novoCliente)
        {
            Cliente clienteMesmoNif = _dc.Clientes.FirstOrDefault(c => c.Nif == novoCliente.Nif);

            if (clienteMesmoNif != null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Já existe um cliente registado com esse NIF.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            if (!string.IsNullOrWhiteSpace(novoCliente.Email))
            {
                Cliente clienteMesmoEmail = _dc.Clientes.FirstOrDefault(c => c.Email == novoCliente.Email);

                if (clienteMesmoEmail != null)
                {
                    return new ServiceResult
                    {
                        Sucesso = false,
                        Mensagem = "Já existe um cliente com esse email.",
                        StatusCode = HttpStatusCode.Conflict
                    };
                }
            }

            _dc.Clientes.InsertOnSubmit(novoCliente);

            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao criar o cliente.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Cliente criado com sucesso.",
                StatusCode = HttpStatusCode.Created
            };
        }

        /// <summary>
        /// Updates an existing client after validating the client data.
        /// </summary>
        /// <param name="clienteAlterado">The client data to update.</param>
        /// <returns>The result of the update operation.</returns>
        public ServiceResult AlterarCliente(Cliente clienteAlterado)
        {
            Cliente cliente = _dc.Clientes.FirstOrDefault(c => c.IdCLiente == clienteAlterado.IdCLiente);

            if (cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum cliente com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool nifDuplicado = _dc.Clientes.Any(c => c.Nif == clienteAlterado.Nif && c.IdCLiente != clienteAlterado.IdCLiente);

            if (nifDuplicado)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Já existe outro cliente com esse NIF.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            if (!string.IsNullOrWhiteSpace(clienteAlterado.Email))
            {
                bool emailDuplicado = _dc.Clientes.Any(c => c.Email == clienteAlterado.Email && c.IdCLiente != clienteAlterado.IdCLiente);

                if (emailDuplicado)
                {
                    return new ServiceResult
                    {
                        Sucesso = false,
                        Mensagem = "Já existe outro cliente com esse email.",
                        StatusCode = HttpStatusCode.Conflict
                    };
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
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao alterar o cliente.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Cliente alterado com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }

        /// <summary>
        /// Deletes a client if there are no associated invoices, consumptions or meters.
        /// </summary>
        /// <param name="id">The Id of the client to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        public ServiceResult EliminarCliente(int id)
        {
            Cliente cliente = _dc.Clientes.FirstOrDefault(c => c.IdCLiente == id);

            if (cliente == null)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Não existe nenhum cliente com esse Id.",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            bool temFaturas = _dc.Faturas.Any(f => f.IdCliente == id);

            if (temFaturas)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente não pode ser eliminado porque possui faturas associadas.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            bool temConsumos = _dc.Consumos.Any(consumo => _dc.Contadores.Any
            (contador => contador.IdContador == consumo.IdContador && contador.IdCliente == id));

            if (temConsumos)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente não pode ser eliminado porque possui consumos associados.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }

            bool temContadores = _dc.Contadores.Any(c => c.IdCliente == id);

            if (temContadores)
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "O cliente não pode ser eliminado porque possui contadores associados.",
                    StatusCode = HttpStatusCode.Conflict
                };
            }
            _dc.Clientes.DeleteOnSubmit(cliente);

            try
            {
                _dc.SubmitChanges();
            }
            catch
            {
                return new ServiceResult
                {
                    Sucesso = false,
                    Mensagem = "Ocorreu um erro ao eliminar o cliente.",
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new ServiceResult
            {
                Sucesso = true,
                Mensagem = "Cliente eliminado com sucesso.",
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}