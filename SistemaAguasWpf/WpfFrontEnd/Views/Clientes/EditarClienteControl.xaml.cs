using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Clientes
{
    /// <summary>
    /// Interaction logic for EditarClienteControl.xaml
    /// </summary>
    public partial class EditarClienteControl : UserControl
    {

        private readonly ClienteApiService clienteApiService = new ClienteApiService();
        private Cliente clienteSelecionado;

        public EditarClienteControl()
        {
            InitializeComponent();

            CarregarClientes();
        }

        /// <summary>
        /// Loads the list of clients into the data grid.
        /// </summary>
        private async Task CarregarClientes()
        {
            dgClientes.ItemsSource = await clienteApiService.ObterClientes();
        }

        /// <summary>
        /// Handles the DataGrid selection change event.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clienteSelecionado = dgClientes.SelectedItem as Cliente;

            if (clienteSelecionado == null)
            {
                return;

            }
            txtNome.Text = clienteSelecionado.Nome;
            txtApelido.Text = clienteSelecionado.Apelido;
            txtNif.Text = clienteSelecionado.Nif;
            txtContacto.Text = clienteSelecionado.Contacto;
            txtEmail.Text = clienteSelecionado.Email;
            txtMorada.Text = clienteSelecionado.Morada;
            txtCodigoPostal.Text = clienteSelecionado.CodigoPostal;
            txtLocalidade.Text = clienteSelecionado.Localidade;
            chkAtivo.IsChecked = clienteSelecionado.Ativo;
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            clienteSelecionado = null;
            LimparCampos();
        }

        /// <summary>
        /// Deletes the selected client.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        /// <param name="e"></param>
        private async void BtnGuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            if (clienteSelecionado == null)
            {
                MessageBox.Show("Selecione primeiro um cliente.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            clienteSelecionado.Nome = txtNome.Text;
            clienteSelecionado.Apelido = txtApelido.Text;
            clienteSelecionado.Nif = txtNif.Text;
            clienteSelecionado.Contacto = txtContacto.Text;
            clienteSelecionado.Email = txtEmail.Text;
            clienteSelecionado.Morada = txtMorada.Text;
            clienteSelecionado.CodigoPostal = txtCodigoPostal.Text;
            clienteSelecionado.Localidade = txtLocalidade.Text;
            clienteSelecionado.Ativo = chkAtivo.IsChecked == true;

            bool sucesso = await clienteApiService.AlterarCliente(clienteSelecionado);

            if (sucesso)
            {
                MessageBox.Show("Cliente atualizado com sucesso.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await CarregarClientes();
                      LimparCampos();
                clienteSelecionado = null;

            }
            else
            {
                MessageBox.Show("Não foi possível atualizar o cliente.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the input fields.
        /// </summary>
        private void LimparCampos()
        {
            txtNome.Clear();
            txtApelido.Clear();
            txtNif.Clear();
            txtContacto.Clear();
            txtEmail.Clear();
            txtMorada.Clear();
            txtCodigoPostal.Clear();
            txtLocalidade.Clear();

            chkAtivo.IsChecked = false;
            dgClientes.SelectedItem = null;
        }
    }
}
