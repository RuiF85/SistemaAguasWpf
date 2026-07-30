using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        private void DgConsumos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dgConsumos.SelectedItem == null)
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

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (dgConsumos.SelectedItem == null)
            {
                MessageBox.Show( "Selecione um consumo.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpData.SelectedDate == null)
            {
                MessageBox.Show( "Selecione a data.", "Data obrigatória",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                 return;
            }

            if (dpData.SelectedDate.Value.Date > DateTime.Today)
            {
                MessageBox.Show( "A data não pode ser futura.", "Data inválida",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal leitura;

            if (!decimal.TryParse(txtLeituraAtual.Text.Trim(), out leitura))
            {
                MessageBox.Show( "Introduza uma leitura válida.", "Erro",
                    MessageBoxButton.OK,MessageBoxImage.Warning);
                return;
            }

            if (leitura < 0)
            {
                MessageBox.Show( "A leitura não pode ser negativa.",  "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Consumo consumo = (Consumo)dgConsumos.SelectedItem;

            consumo.Data = dpData.SelectedDate.Value.Date;
            consumo.LeituraAtual = leitura;

            bool sucesso = await consumoService.AlterarConsumo(consumo);

            if (sucesso)
            {
                MessageBox.Show( "Consumo alterado com sucesso.", "Sucesso",
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

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

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