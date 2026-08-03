using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Consumos
{
    /// <summary>
    /// UserControl for deleting consumptions.
    /// </summary>
    public partial class ApagarConsumoControl : UserControl
    {
        private readonly ConsumoApiService consumoService;

        private List<Consumo> consumos;

        public ApagarConsumoControl()
        {
            InitializeComponent();

            consumoService = new ConsumoApiService();

            CarregarConsumos();

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
                MessageBox.Show("Não foi possível carregar os cosumos.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Deletes the selected consumption after user confirmation.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgConsumos.SelectedItem == null)
            {
                MessageBox.Show("Selecione um consumo.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Consumo consumo = (Consumo)dgConsumos.SelectedItem;

            MessageBoxResult resultado = MessageBox.Show("Tem a certeza que pretende eliminar este consumo?", "Confirmar eliminação",
                                                             MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resultado != MessageBoxResult.Yes)
            {
                return;
            }
            try
            {
                bool sucesso = await consumoService.EliminarConsumo(consumo.IdConsumo);
                if (sucesso)
                {
                    MessageBox.Show("Consumo eliminado com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    CarregarConsumos();
                    LimparSelecao();
                }
                else
                {
                    MessageBox.Show("Não foi possível eliminar o consumo, Já tem uma fatura associada.", "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Não foi possível eliminar o consumo.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// Clears the current selection.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }

        /// <summary>
        /// Clears the current selection.
        /// </summary>
        private void LimparSelecao()
        {
            dgConsumos.SelectedItem = null;
        }
    }
}
