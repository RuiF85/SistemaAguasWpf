using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Contadores
{
    /// <summary>
    /// Interaction logic for NovoContadorControl.xaml
    /// </summary>
    public partial class NovoContadorControl : UserControl
    {
        private readonly ClienteApiService clienteService = new ClienteApiService();
        private readonly ContadorApiService contadorService = new ContadorApiService();

        public NovoContadorControl()
        {
            InitializeComponent();

            CarregarClientes();

            dpDataInstalacao.SelectedDate = DateTime.Today;
        }

        private async Task CarregarClientes()
        {
            try
            {
                cmbClientes.ItemsSource = await clienteService.ObterClientes();
            }
            catch
            {
                MessageBox.Show("Não foi possível carregar os clientes.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string numeroContador = txtNumeroContador.Text.Trim();

            if (cmbClientes.SelectedValue == null)
            {
                MessageBox.Show("Selecione um cliente.", "Cliente obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                cmbClientes.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(numeroContador))
            {
                MessageBox.Show("Introduza o número do contador.", "Campo obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtNumeroContador.Focus();
                return;
            }

            if (dpDataInstalacao.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data de instalação.", "Data obrigatória",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                dpDataInstalacao.Focus();
                return;
            }

            if (dpDataInstalacao.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show( "A data de instalação não pode ser uma data futura.",
                    "Data inválida",
                    MessageBoxButton.OK,MessageBoxImage.Warning);

                dpDataInstalacao.Focus();
                return;
            }

            Contador contador = new Contador
            {
                IdCliente = (int)cmbClientes.SelectedValue,
                NumeroContador = numeroContador,
                DataInstalacao = dpDataInstalacao.SelectedDate.Value,
                Ativo = chkAtivo.IsChecked == true
            };


            try
            {
                bool sucesso = await contadorService.CriarContador(contador);

                if (sucesso)
                {
                    MessageBox.Show("Contador criado com sucess.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LimparCampos();
                }
                else
                {
                    MessageBox.Show("Não foi possível criar o contador.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Não foi possível comunicar com a API.", "Erro de ligação",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLimpar_Click(object sender, EventArgs e)
        {
            LimparCampos();
        }



        private void LimparCampos()
        {
            cmbClientes.SelectedIndex = -1;
            txtNumeroContador.Clear();
            dpDataInstalacao.SelectedDate = DateTime.Today;
            chkAtivo.IsChecked = true;

            cmbClientes.Focus();
        }
    }
}
