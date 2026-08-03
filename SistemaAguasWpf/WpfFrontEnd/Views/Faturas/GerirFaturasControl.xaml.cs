using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Faturas
{
    /// <summary>
    /// UserControl for managing invoices.
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

        /// <summary>
        /// Loads the list of states into the combo box.
        /// </summary>
        private async void CarregarEstado()
        {
            cmbEstados.ItemsSource = await estadoService.ObterEstados();
        }

        /// <summary>
        /// Loads the invoices from the API and populates the DataGrid.
        /// </summary>
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

        /// <summary>
        /// Refreshes the list of invoices.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            CarregarFaturas();
        }

        /// <summary>
        /// Displays the selected invoice information.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Updates the status of the selected invoice.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
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
                bool sucesso = await faturaService.AlterarEstado(fatura);
                if (sucesso)
                {
                    MessageBox.Show("Estado da fatura atualizado com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CarregarFaturas();
                }
                else
                {
                    MessageBox.Show("Não foi possível alterar o estado da fatura.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Não foi possível alterar o estado da fatura.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cancels the selected invoice after user confirmation.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Clears the selected invoice and resets the controls.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LimparSelecao()
        {
            dgFaturas.SelectedItem = null;
            txtFaturaSelecionada.Text = "Nenhuma fatura selecionada";

            cmbEstados.SelectedItem = null;
            cmbEstados.IsEnabled = false;
            btnAnularFatura.IsEnabled = false;
            btnAlterarEstado.IsEnabled = false;
        }
    }
}
