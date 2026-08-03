using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Consumos
{
    /// <summary>
    /// Interaction logic for EditarConsumoControl.xaml
    /// </summary>
    public partial class EditarConsumoControl : UserControl
    {
        private readonly ConsumoApiService consumoService;
        private readonly ContadorApiService contadorService;

        private List<Consumo> consumos;

        public EditarConsumoControl()
        {
            InitializeComponent();

            consumoService = new ConsumoApiService();
            contadorService = new ContadorApiService();

            CarregarContadores();
            CarregarConsumos();
        }

        /// <summary>
        /// Loads the available meters into the ComboBox.
        /// </summary>
        private async void CarregarContadores()
        {
            try
            {
                cmbContadores.ItemsSource = await contadorService.ObterContadores();
            }
            catch
            {
                MessageBox.Show("Não foi possível carregar os contadores.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads the consumptions from the API and populates the DataGrid.
        /// </summary>
        private async void CarregarConsumos()
        {
            try
            {
                consumos = await consumoService.ObterConsumos();
                dgConsumos.ItemsSource = consumos;
            }
            catch
            {
                MessageBox.Show("Não foi possível carregar os consumos.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the form with the selected consumption data.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void DgConsumos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgConsumos.SelectedItem == null)
            {
                return;
            }

            Consumo consumo = (Consumo)dgConsumos.SelectedItem;

            cmbContadores.SelectedValue = consumo.IdContador;
            dpData.SelectedDate = consumo.Data;
            txtLeituraAtual.Text = consumo.LeituraAtual.ToString("0.###");
            txtConsumoCalculado.Text = consumo.ConsumoCalculado.ToString("0.###");

            txtIdFatura.Text = consumo.IdFatura.HasValue ? consumo.IdFatura.Value.ToString() : "";
        }

        /// <summary>
        /// Validates the entered data and updates the selected consumption.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (dgConsumos.SelectedItem == null)
            {
                MessageBox.Show("Selecione um consumo.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpData.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data.", "Data obrigatória",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpData.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show("A data não pode ser futura.", "Data inválida",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal leitura;

            if (!decimal.TryParse(txtLeituraAtual.Text.Trim(), out leitura))
            {
                MessageBox.Show("Introduza uma leitura válida.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (leitura < 0)
            {
                MessageBox.Show("A leitura não pode ser negativa.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Consumo consumo = (Consumo)dgConsumos.SelectedItem;

            consumo.Data = dpData.SelectedDate.Value.Date;
            consumo.LeituraAtual = leitura;

            bool sucesso = await consumoService.AlterarConsumo(consumo);

            if (sucesso)
            {
                MessageBox.Show("Consumo alterado com sucesso.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                CarregarConsumos();
                LimparCampos();
            }
            else
            {
                MessageBox.Show("Não foi possível alterar o consumo.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Clears all form fields.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        /// <summary>
        /// Clears the form fields and resets the selection.
        /// </summary>
        private void LimparCampos()
        {
            dgConsumos.SelectedItem = null;
            cmbContadores.SelectedIndex = -1;
            dpData.SelectedDate = null;

            txtLeituraAtual.Clear();
            txtConsumoCalculado.Clear();
            txtIdFatura.Clear();
        }
    }
}