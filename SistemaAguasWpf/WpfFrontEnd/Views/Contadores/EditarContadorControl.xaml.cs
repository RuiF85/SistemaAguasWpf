using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Contadores
{
    /// <summary>
    /// UserControl for editing meters.
    /// </summary>
    public partial class EditarContadorControl : UserControl
    {
        private readonly ContadorApiService contadorService = new ContadorApiService();
        private readonly ClienteApiService clienteService = new ClienteApiService();

        private Contador contadorSelecionado;

        public EditarContadorControl()
        {
            InitializeComponent();
            CarregarDados();
        }

        /// <summary>
        /// Loads the clients and meters into the form.
        /// </summary>
        private async void CarregarDados()
        {
            await CarregarClientes();
            await CarregarContadores();
        }

        /// <summary>
        /// Loads the clients into the ComboBox.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CarregarClientes()
        {
            cmbClientes.ItemsSource = await clienteService.ObterClientes();
        }

        /// <summary>
        /// Loads the meters from the API and populates the DataGrid.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task CarregarContadores()
        {
            var contadores = await contadorService.ObterContadores();
            var clientes = await clienteService.ObterClientes();

            foreach (var c in contadores)
            {
                c.NomeCliente = clientes.FirstOrDefault(cl => cl.IdCliente == c.IdCliente)?.NomeCompleto;
            }
            dgContadores.ItemsSource = contadores;
        }

        /// <summary>
        /// Displays the selected meter information in the form.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void DgContadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contadorSelecionado = dgContadores.SelectedItem as Contador;

            if (contadorSelecionado == null)
            {
                return;
            }

            cmbClientes.SelectedValue = contadorSelecionado.IdCliente;
            txtNumeroContador.Text = contadorSelecionado.NumeroContador;
            dpDataInstalacao.SelectedDate = contadorSelecionado.DataInstalacao;
            chkAtivo.IsChecked = contadorSelecionado.Ativo;
        }

        /// <summary>
        /// Validates the entered data and updates the selected meter.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void BtnGuardarAlteracoes_Click(object sender, RoutedEventArgs e)
        {
            if (contadorSelecionado == null)
            {
                MessageBox.Show("Selecione um contador.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbClientes.SelectedValue == null)
            {
                MessageBox.Show("Selecione um cliente.", "Cliente obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbClientes.Focus();
                return;
            }
            string numeroContador = txtNumeroContador.Text.Trim();

            if (string.IsNullOrWhiteSpace(numeroContador))
            {
                MessageBox.Show("Introduza o número do contador.", "Campo obrigatorio.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtNumeroContador.Focus();
                return;
            }

            if (dpDataInstalacao.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data de instalação.", "Data obrigatória.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                dpDataInstalacao.Focus();
                return;
            }

            contadorSelecionado.IdCliente = (int)cmbClientes.SelectedValue;
            contadorSelecionado.NumeroContador = numeroContador;
            contadorSelecionado.DataInstalacao = dpDataInstalacao.SelectedDate.Value;
            contadorSelecionado.Ativo = chkAtivo.IsChecked == true;

            var sucesso = await contadorService.AlterarContador(contadorSelecionado);

            if (sucesso)
            {
                MessageBox.Show("Contador alterado com sucesso.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await CarregarContadores();

                LimparCampos();
            }
            else
            {
                MessageBox.Show("Não foi possivel alterar o contador.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the form fields and resets the selection.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        /// <summary>
        /// Clears the form fields and resets the selection.
        /// </summary>
        private void LimparCampos()
        {
            contadorSelecionado = null;

            dgContadores.SelectedItem = null;
            cmbClientes.SelectedIndex = -1;
            txtNumeroContador.Clear();
            dpDataInstalacao.SelectedDate = null;
            chkAtivo.IsChecked = false;
        }
    }
}
