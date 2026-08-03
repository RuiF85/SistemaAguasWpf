using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Contadores
{
    /// <summary>
    /// Interaction logic for ApagarControl.xaml
    /// </summary>
    public partial class ApagarContadorControl : UserControl
    {
        private readonly ContadorApiService contadorService = new ContadorApiService();
        private readonly ClienteApiService clienteService = new ClienteApiService();
        private Contador contadorSelecionado;

        public ApagarContadorControl()
        {
            InitializeComponent();

            CarregarContadores();
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
        /// Updates the selected meter details.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void DgContadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contadorSelecionado = dgContadores.SelectedItem as Contador;

            if (contadorSelecionado == null)
            {
                LimparDetalhes();
                return;
            }

            txtCliente.Text = contadorSelecionado.IdCliente.ToString();
            txtNumeroContador.Text = contadorSelecionado.NumeroContador;
            txtDataInstalacao.Text = contadorSelecionado.DataInstalacao.ToString("dd/MM/yyyy");
            txtEstado.Text = contadorSelecionado.Ativo ? "Ativo" : "Inativo";
        }

        /// <summary>
        /// Deletes the selected meter after user confirmation.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void BtnApagar_Click(object sender, RoutedEventArgs e)
        {
            if (contadorSelecionado == null)
            {
                MessageBox.Show("Selecione um Contador.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult resposta = MessageBox.Show( $"Tem a certeza de que pretende apagar o contador {contadorSelecionado.NumeroContador}?",
             "Confirmar eliminação",
             MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resposta != MessageBoxResult.Yes)
            {
                return;
            }
            bool sucesso = await contadorService.EliminarContador(contadorSelecionado.IdContador);

            if (sucesso)
            {
                MessageBox.Show("Contador apagado com sucesso.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                await AtualizarLista();
                         LimparSelecao();
            }
            else
            {
                MessageBox.Show("Não foi possível apagar o contador.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        private void LimparSelecao()
        {
            contadorSelecionado = null;
            dgContadores.SelectedItem = null;
            LimparDetalhes();
        }

        /// <summary>
        /// Refreshes the list of meters.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AtualizarLista()
        {
            await CarregarContadores();
        }

        /// <summary>
        /// Clears the details of the selected meter.
        /// </summary>
        private void LimparDetalhes()
        {
            txtCliente.Text = string.Empty;
            txtNumeroContador.Text = string.Empty;
            txtDataInstalacao.Text = string.Empty;
            txtEstado.Text = string.Empty;
        }
    }
}
