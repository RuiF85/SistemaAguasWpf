using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Faturas
{
    /// <summary>
    /// Interaction logic for GerarFaturaControl.xaml
    /// </summary>
    public partial class GerarFaturaControl : UserControl
    {

        private readonly ConsumoApiService _consumoApiService;
        private readonly FaturaApiService _faturaApiService;
        private readonly ContadorApiService _contadorApiService;
        private readonly ClienteApiService _clienteApiService;

        private List<Consumo> _consumos;
        private List<Contador> _contadores;
        private List<Cliente> _clientes;


        public GerarFaturaControl()
        {
            InitializeComponent();

            _consumoApiService = new ConsumoApiService();
            _faturaApiService = new FaturaApiService();
            _contadorApiService = new ContadorApiService();
            _clienteApiService = new ClienteApiService();

            _consumos = new List<Consumo>();
            _contadores = new List<Contador>();
            _clientes = new List<Cliente>();

            Loaded += GerarFaturaControl_Loaded;
        }

        private async void GerarFaturaControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarDados();
        }

        private async Task CarregarDados()
        {
            try
            {
                btnGerarFatura.IsEnabled = false;
                cmbConsumos.IsEnabled = false;

                _consumos = await _consumoApiService.ObterConsumos();

                _contadores = await _contadorApiService.ObterContadores();

                _clientes = await _clienteApiService.ObterClientes();

                List<Consumo> consumosDisponiveis = _consumos
                    .Where(c => c.IdFatura == null)
                    .OrderBy(c => c.Data)
                    .ToList();

                cmbConsumos.ItemsSource = consumosDisponiveis;

                bool existemConsumos = consumosDisponiveis.Any();

                cmbConsumos.IsEnabled = existemConsumos;

                txtSemConsumos.Visibility = existemConsumos
                    ? Visibility.Collapsed
                    : Visibility.Visible;

                if (!existemConsumos)
                {
                    LimparResumo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível carregar os dados necessários.\n\n" +
                    ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);

                cmbConsumos.ItemsSource = null;
                cmbConsumos.IsEnabled = false;
                btnGerarFatura.IsEnabled = false;
                txtSemConsumos.Visibility = Visibility.Visible;

                LimparResumo();
            }
        }


        private void CmbConsumos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Consumo consumoSelecionado = cmbConsumos.SelectedItem as Consumo;

            if (consumoSelecionado == null)
            {
                LimparResumo();
                btnGerarFatura.IsEnabled = false;
                return;
            }

            PreencherDados(consumoSelecionado);

            btnGerarFatura.IsEnabled = true;
        }

        private void PreencherDados(Consumo consumo)
        {
            Contador contador = _contadores.SingleOrDefault(c => c.IdContador == consumo.IdContador);

            Cliente cliente = null;

            if (contador != null)
            {
                cliente = _clientes.SingleOrDefault(c => c.IdCliente == contador.IdCliente);
            }

            txtNumeroFatura.Text = "Automático";

            txtIdConsumo.Text = consumo.IdConsumo.ToString();

            txtDataConsumo.Text = consumo.Data.ToString("dd/MM/yyyy");

            txtDataFatura.Text = DateTime.Today.ToString("dd/MM/yyyy");

            txtDataLimitePagamento.Text = DateTime.Today.AddDays(30).ToString("dd/MM/yyyy");

            txtConsumoCalculado.Text = consumo.ConsumoCalculado.ToString("N2") + " m³";

            if (contador != null)
            {
                txtNumeroContrato.Text = contador.NumeroContador;
            }
            else
            {
                txtNumeroContrato.Text = "—";
            }

            if (cliente != null)
            {
                txtTitularContrato.Text = cliente.NomeCompleto;

                txtMoradaContrato.Text = cliente.Morada;
            }
            else
            {
                txtTitularContrato.Text = "—";
                txtMoradaContrato.Text = "—";
            }

            CalcularEscaloes(consumo.ConsumoCalculado);
        }

        private void CalcularEscaloes(decimal consumoCalculado)
        {
            decimal consumoRestante = Math.Max(consumoCalculado, 0);

            decimal consumoEscalao1 = Math.Min(consumoRestante, 5);

            consumoRestante -= consumoEscalao1;

            decimal consumoEscalao2 = Math.Min(consumoRestante, 10);

            consumoRestante -= consumoEscalao2;

            decimal consumoEscalao3 = Math.Min(consumoRestante, 10);

            consumoRestante -= consumoEscalao3;

            decimal consumoEscalao4 = Math.Max(consumoRestante, 0);

            List<EscalaoFaturaView> escaloes = new List<EscalaoFaturaView>
                {
                    new EscalaoFaturaView
                    {
                        Escalao = "1.º escalão",
                        Intervalo = "Até 5 m³",
                        ValorUnitario = 0.30m,
                        ConsumoAplicado = consumoEscalao1
                    },

                    new EscalaoFaturaView
                    {
                        Escalao = "2.º escalão",
                        Intervalo = "De 5 a 15 m³",
                        ValorUnitario = 0.80m,
                        ConsumoAplicado = consumoEscalao2
                    },

                    new EscalaoFaturaView
                    {
                        Escalao = "3.º escalão",
                        Intervalo = "De 15 a 25 m³",
                        ValorUnitario = 1.20m,
                        ConsumoAplicado = consumoEscalao3
                    },

                    new EscalaoFaturaView
                    {
                        Escalao = "4.º escalão",
                        Intervalo = "Acima de 25 m³",
                        ValorUnitario = 1.60m,
                        ConsumoAplicado = consumoEscalao4
                    }
                };

            dgEscaloes.ItemsSource = escaloes;

            decimal subtotal = escaloes.Sum(e => e.Total);

            txtSubtotal.Text = subtotal.ToString("N2") + " €";

            txtTotalFinal.Text = subtotal.ToString("N2") + " €";
        }

        private void BtnLimpar_Click(object sender, RoutedEventArgs e)
        {
            LimparSelecao();
        }

        private async void BtnGerarFatura_Click(object sender, RoutedEventArgs e)
        {
            Consumo consumoSelecionado = cmbConsumos.SelectedItem as Consumo;

            if (consumoSelecionado == null)
            {
                MessageBox.Show("Selecione um consumo para gerar a fatura.", "Campo obrigatório",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult confirmacao = MessageBox.Show("Pretende gerar uma fatura para o consumo selecionado?\n\n" +
                "Consumo: " +
                consumoSelecionado.ConsumoCalculado.ToString("N2") +
                " m³\n" +
                "Data: " +
                consumoSelecionado.Data.ToString("dd/MM/yyyy"),
                "Confirmar faturação",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacao != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                btnGerarFatura.IsEnabled = false;
                cmbConsumos.IsEnabled = false;

                bool sucesso = await _faturaApiService.GerarFatura(consumoSelecionado.IdConsumo);

                if (sucesso)
                {
                    MessageBox.Show("Fatura gerada com sucesso.", "Sucesso",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LimparSelecao();

                    await CarregarDados();
                }
                else
                {
                    MessageBox.Show("Não foi possível gerar a fatura.\n" +
                        "Verifique se o consumo já foi faturado.",
                        "Erro",
                        MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbConsumos.IsEnabled = true;
                    btnGerarFatura.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao gerar a fatura.\n\n" +
                    ex.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                cmbConsumos.IsEnabled = true;
                btnGerarFatura.IsEnabled = true;
            }
        }

        private void LimparSelecao()
        {
            cmbConsumos.SelectedItem = null;

            LimparResumo();

            btnGerarFatura.IsEnabled = false;
        }

        private void LimparResumo()
        {
            txtNumeroFatura.Text = "—";
            txtNumeroContrato.Text = "—";
            txtIdConsumo.Text = "—";

            txtTitularContrato.Text = "—";
            txtMoradaContrato.Text = "—";

            txtDataConsumo.Text = "—";
            txtDataFatura.Text = "—";
            txtDataLimitePagamento.Text = "—";

            txtConsumoCalculado.Text = "0,00 m³";
            txtSubtotal.Text = "0,00 €";
            txtTotalFinal.Text = "0,00 €";

            dgEscaloes.ItemsSource = null;
        }
    }
}
