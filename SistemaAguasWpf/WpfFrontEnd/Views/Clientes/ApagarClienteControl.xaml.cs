using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Clientes
{
    /// <summary>
    /// Interaction logic for ApagarClienteControl.xaml
    /// </summary>
    public partial class ApagarClienteControl : UserControl
    {

        private readonly ClienteApiService clienteApiService = new ClienteApiService();
        private Cliente clienteSelecionado;

        public ApagarClienteControl()
        {
            InitializeComponent();

            CarregarClientes();
        }

        private async Task CarregarClientes()
        {
            dgClientes.ItemsSource = await clienteApiService.ObterClientes();
        }

        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clienteSelecionado = dgClientes.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                return;

            }

            lblNome.Text = clienteSelecionado.Nome + " " + clienteSelecionado.Apelido;

            lblNif.Text = clienteSelecionado.Nif;

            lblContacto.Text = clienteSelecionado.Contacto;

            lblEmail.Text = clienteSelecionado.Email;
        }

        private void BtnCancelar_Click( object sender,  RoutedEventArgs e)
        {
            dgClientes.SelectedItem = null;

            lblNome.Text = "";
            lblNif.Text = "";
            lblContacto.Text = "";
            lblEmail.Text = "";

            clienteSelecionado = null;
        }

        private async void BtnApagarCliente_Click( object sender, RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                MessageBox.Show( "Selecione um cliente.","Aviso",
                    MessageBoxButton.OK,  MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult resposta =
                MessageBox.Show(
                    $"Tem a certeza que pretende apagar o cliente {clienteSelecionado.Nome} {clienteSelecionado.Apelido}?",
                    "Confirmar eliminação",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resposta == MessageBoxResult.No)
            {
                return;
            }

            bool sucesso = await clienteApiService.EliminarCliente(clienteSelecionado.IdCliente);

            if (sucesso)
            {
                MessageBox.Show(
                    "Cliente eliminado com sucesso.","Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await CarregarClientes();

                BtnCancelar_Click(null, null);
            }
            else
            {
                MessageBox.Show("Não foi possível eliminar o cliente.", "Erro",
                    MessageBoxButton.OK,  MessageBoxImage.Error);
            }
        }
    }
}