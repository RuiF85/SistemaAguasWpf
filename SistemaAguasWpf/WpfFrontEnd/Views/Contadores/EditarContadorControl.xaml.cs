using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Contadores
{
    /// <summary>
    /// Interaction logic for EditarClienteControl.xaml
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

        private async void CarregarDados()
        {
            await CarregarClientes();
            await CarregarContadores();
        }


        private async Task CarregarClientes()
        {
            cmbClientes.ItemsSource = await clienteService.ObterClientes();
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
                return;
            }

            cmbClientes.SelectedValue = contadorSelecionado.IdCliente;
            txtNumeroContador.Text = contadorSelecionado.NumeroContador;
            dpDataInstalacao.SelectedDate = contadorSelecionado.DataInstalacao;
            chkAtivo.IsChecked = contadorSelecionado.Ativo;
        }

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


        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

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
