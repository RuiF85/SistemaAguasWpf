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
        private Contador contadorSelecionado;

        public ApagarContadorControl()
        {
            InitializeComponent();

            CarregarContadores();
        }

        private async Task CarregarContadores()
        {
            dgContadores.ItemsSource = await contadorService.ObterContadores();
        }

        private void DgContadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            contadorSelecionado = dgContadores.SelectedItem as Contador;

            if (contadorSelecionado == null)
            {
                LimparDetalhes();
                return;
            }

            txtCliente.Text = contadorSelecionado.IdCliente.ToString();
            txtNumeroContador.Text = contadorSelecionado.DataInstalacao.ToString("dd/MM/yyyy");
            txtEstado.Text = contadorSelecionado.Ativo ? "Ativo" : "Inativo";
        }

        private async void BtnApagar_Click(object sender, RoutedEventArgs e)
        {
            if (contadorSelecionado == null)
            {
                MessageBox.Show("Selecione um Contador.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBoxResult resposta = MessageBox.Show(
             $"Tem a certeza de que pretende apagar o contador{contadorSelecionado.NumeroContador}?",
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

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }

        private void LimparSelecao()
        {
            contadorSelecionado = null;
            dgContadores.SelectedItem = null;
            LimparDetalhes();
        }

        private async Task AtualizarLista()
        {
            dgContadores.ItemsSource = await contadorService.ObterContadores();
        }

        private void LimparDetalhes()
        {
            txtCliente.Text = string.Empty;
            txtNumeroContador.Text = string.Empty;
            txtDataInstalacao.Text = string.Empty;
            txtEstado.Text = string.Empty;
        }
    }
}
