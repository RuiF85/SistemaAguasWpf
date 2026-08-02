using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Faturas
{
    /// <summary>
    /// Interaction logic for GerirFaturasControl.xaml
    /// </summary>
    public partial class GerirFaturasControl : UserControl
    {

        private readonly FaturaApiService faturaService;
        private readonly EstadoFaturaApiService estadoService;

        private List<Fatura> faturas;

        public GerirFaturasControl()
        {
            InitializeComponent();

            faturaService = new FaturaApiService();
            estadoService = new EstadoFaturaApiService();

            CarregarFaturas();
            CarregarEstado();
        }

        private async void CarregarEstado()
        {
            cmbEstados.ItemsSource = await estadoService.ObterEstados();
        }

        private async void CarregarFaturas()
        {
            try
            {
                faturas = await faturaService.ObterFaturas();
               
                dgFaturas.ItemsSource = faturas;

                LimparSelecao();

            }
            catch (Exception)
            {
                MessageBox.Show("Não foi possível carregar as faturas.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarFaturas(); 
        }

        private void DgFaturas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgFaturas.SelectedItem == null)
            {
                return;
            }
            Fatura fatura = (Fatura)dgFaturas.SelectedItem;

            txtFaturaSelecionada.Text = fatura.IdFatura.ToString();

            cmbEstados.SelectedValue = fatura.IdEstadoFatura;

            cmbEstados.IsEnabled = true;
            btnAlterarEstado.IsEnabled = true;
            btnAnularFatura.IsEnabled = true;
        }

        private async void BtnAlterarEstado_Click(object sender, RoutedEventArgs e)
        {
            if (dgFaturas.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma fatura.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Fatura fatura = (Fatura)dgFaturas.SelectedItem;

            fatura.IdEstadoFatura = (int)cmbEstados.SelectedValue;

            try
            {
                bool sucesso = await faturaService.AlterarEsatdo(fatura);
                if (sucesso)
                {
                    MessageBox.Show("Esatdo da fatura atualizado com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CarregarFaturas();
                }
                else
                {
                    MessageBox.Show("Não foi possível alterar fatura.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {

                MessageBox.Show("Não foi possível alterar fatura.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void BtnAnularFatura_Click(object sender, RoutedEventArgs e)
        {
            if (dgFaturas.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma fatura.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;

            }

            Fatura fatura = (Fatura)dgFaturas.SelectedItem;

            MessageBoxResult resultado = MessageBox.Show("Pretende anular esta fatura?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
            {
                return;

            }
            try
            {
                bool sucesso = await faturaService.AnularFatura(fatura.IdFatura);

                if (sucesso)
                {
                    MessageBox.Show("Fatura anulada com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CarregarFaturas();
                }
                else
                {
                    MessageBox.Show("Não foi possível anular a fatura.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Não foi possível anular a fatura.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }



        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }


        private void LimparSelecao()
        {
            dgFaturas.SelectedItem = null;
            txtFaturaSelecionada.Text = $"Nenhuma fatura selecionada";

            cmbEstados.SelectedItem = null;
            cmbEstados.IsEnabled = false;
            btnAnularFatura.IsEnabled = false;
           btnAlterarEstado.IsEnabled = false;
        }
    }
}
