using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Consumos
{
    public partial class NovoConsumoControl : UserControl
    {
        private readonly ContadorApiService contadorService;
        private readonly ConsumoApiService consumoService;

        private List<Consumo> todosConsumos;

        public NovoConsumoControl()
        {
            InitializeComponent();

            contadorService = new ContadorApiService();
            consumoService = new ConsumoApiService();

            todosConsumos = new List<Consumo>();

            dpData.SelectedDate = DateTime.Today;

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
        /// Loads the consumption history for the selected meter into the DataGrid.
        /// </summary>
        private async void CarregarConsumos()
        {
            try
            {
                todosConsumos = await consumoService.ObterConsumos();

                AtualizarHistorico();
            }
            catch
            {
                todosConsumos = new List<Consumo>();

                dgConsumos.ItemsSource = null;
                txtTotalConsumos.Text = "0 registos";

                MessageBox.Show("Não foi possível carregar o histórico de consumos.", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates the consumption history when a different meter is selected in the ComboBox.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void CmbContadores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarHistorico();
        }

        /// <summary>
        /// Updates the DataGrid and information labels based on the selected meter and its consumption history.
        /// </summary>
        private void AtualizarHistorico()
        {
            if (cmbContadores.SelectedValue == null)
            {
                dgConsumos.ItemsSource = null;
                txtTotalConsumos.Text = "0 registos";

                txtInformacaoLeitura.Text = "Selecione um contador para consultar a última leitura.";

                return;
            }

            int idContador = (int)cmbContadores.SelectedValue;

            List<Consumo> historico = todosConsumos
                .Where(c => c.IdContador == idContador)
                .OrderByDescending(c => c.Data)
                .ToList();

            dgConsumos.ItemsSource = historico;

            if (historico.Count == 1)
            {
                txtTotalConsumos.Text = "1 registo";
            }
            else
            {
                txtTotalConsumos.Text = historico.Count + " registos";
            }

            Consumo ultimaLeitura = historico.FirstOrDefault();

            if (ultimaLeitura == null)
            {
                txtInformacaoLeitura.Text = "Este contador ainda não possui leituras registadas.";
            }
            else
            {
                txtInformacaoLeitura.Text = "Última leitura: " +
                    ultimaLeitura.LeituraAtual.ToString("N3") +
                    " em " +
                    ultimaLeitura.Data.ToString("dd/MM/yyyy") +
                    ".";
            }
        }

        /// <summary>
        /// Handles the click event of the "Guardar" button to save a new consumption record.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbContadores.SelectedValue == null)
            {
                MessageBox.Show("Selecione um contador.", "Contador obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                cmbContadores.Focus();
                return;
            }

            if (dpData.SelectedDate == null)
            {
                MessageBox.Show("Selecione a data da leitura.", "Data obrigatória",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                dpData.Focus();
                return;
            }

            if (dpData.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show("A data da leitura não pode ser futura.", "Data inválida",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                dpData.Focus();
                return;
            }

            string textoLeitura = txtLeituraAtual.Text.Trim();

            if (string.IsNullOrWhiteSpace(textoLeitura))
            {
                MessageBox.Show("Introduza a leitura atual.", "Leitura obrigatória",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtLeituraAtual.Focus();
                return;
            }

            decimal leituraAtual;

            if (!decimal.TryParse(textoLeitura, out leituraAtual))
            {
                MessageBox.Show("Introduza uma leitura válida.", "Leitura inválida",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtLeituraAtual.Focus();
                return;
            }

            if (leituraAtual < 0)
            {
                MessageBox.Show("A leitura atual não pode ser negativa.", "Leitura inválida",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtLeituraAtual.Focus();
                return;
            }

            int idContador = (int)cmbContadores.SelectedValue;

            DateTime dataLeitura = dpData.SelectedDate.Value.Date;

            bool existeMesmaData = todosConsumos.Any(c => c.IdContador == idContador &&
                     c.Data.Date == dataLeitura);

            if (existeMesmaData)
            {
                MessageBox.Show("Já existe uma leitura para esse contador nessa data.", "Leitura duplicada",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                dpData.Focus();
                return;
            }

            Consumo consumo = new Consumo
            {
                IdContador = idContador,
                Data = dataLeitura,
                LeituraAtual = leituraAtual
            };

            try
            {
                bool sucesso = await consumoService.CriarConsumo(consumo);

                if (sucesso)
                {
                    MessageBox.Show("Consumo registado com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    txtLeituraAtual.Clear();
                    dpData.SelectedDate = DateTime.Today;

                    CarregarConsumos();

                    txtLeituraAtual.Focus();
                }
                else
                {
                    MessageBox.Show("Não foi possível registar o consumo.\n\n" +
                        "Verifique se o contador e o cliente estão ativos, " +
                        "se já existe uma leitura nessa data e se o valor " +
                        "respeita as outras leituras.",
                        "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch
            {
                MessageBox.Show("Não foi possível comunicar com a API.", "Erro de ligação",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the "Limpar" button to clear all input fields and reset the form.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        /// <summary>
        /// Clears all input fields and resets the form to its initial state.
        /// </summary>
        private void LimparCampos()
        {
            cmbContadores.SelectedIndex = -1;
            dpData.SelectedDate = DateTime.Today;
            txtLeituraAtual.Clear();

            dgConsumos.ItemsSource = null;
            txtTotalConsumos.Text = "0 registos";

            txtInformacaoLeitura.Text = "Selecione um contador para consultar a última leitura.";

            cmbContadores.Focus();
        }
    }
}