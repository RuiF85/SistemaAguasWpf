using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Dashboard
{
    /// <summary>
    /// UserControl that displays the application dashboard.
    /// </summary>
    public partial class DashboardControl : UserControl
    {
        private readonly ClienteApiService _clienteService;
        private readonly ConsumoApiService _consumoService;
        private readonly FaturaApiService _faturaService;
        private readonly ContadorApiService _contadorService;

        private List<Cliente> _clientes;
        private List<Consumo> _consumos;
        private List<Fatura> _faturas;
        private List<Contador> _contadores;

        public DashboardControl()
        {
            InitializeComponent();

            _clienteService = new ClienteApiService();
            _consumoService = new ConsumoApiService();
            _faturaService = new FaturaApiService();
            _contadorService = new ContadorApiService();

            Loaded += DashboardControl_Loaded;
        }

        /// <summary>
        /// Loads the dashboard data when the control is displayed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void DashboardControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _clientes = await _clienteService.ObterClientes();
                _contadores = await _contadorService.ObterContadores();
                _consumos = await _consumoService.ObterConsumos();
                _faturas = await _faturaService.ObterFaturas();

                // ===== Cartões de cima =====

                txtClientesAtivos.Text = _clientes.Count(c => c.Ativo).ToString();

                txtConsumoMensal.Text = _consumos.Where(c => c.Data.Month == DateTime.Now.Month && c.Data.Year == DateTime.Now.Year)
                    .Sum(c => c.ConsumoCalculado).ToString("N2");

                txtFaturasPendentes.Text = _faturas.Count(f => f.Estado == "Pendente").ToString();

                txtLeiturasValidar.Text = _consumos.Count(c => c.IdFatura == null).ToString();


                // ===== Cartões do meio =====

                txtTotalClientes.Text = _clientes.Count.ToString();

                txtTotalContadores.Text = _contadores.Count.ToString();

                txtTotalConsumos.Text = _consumos.Count.ToString();

                txtValorFaturado.Text = _faturas.Sum(f => f.ValorTotal).ToString("N2") + " €";


                CarregarAtividade();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar o Dashboard.\n\n" + ex.Message, "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Loads the latest consumption activity into the DataGrid.
        /// </summary>
        private void CarregarAtividade()
        {
            List<AtividadeDashboard> lista = new List<AtividadeDashboard>();

            foreach (Consumo consumo in _consumos.OrderByDescending(c => c.Data).Take(10))
            {
                Contador contador = _contadores.FirstOrDefault(c => c.IdContador == consumo.IdContador);

                Cliente cliente = null;

                if (contador != null)
                {
                    cliente = _clientes.FirstOrDefault(c => c.IdCliente == contador.IdCliente);
                }

                lista.Add(new AtividadeDashboard
                {
                    Cliente = cliente != null ? cliente.NomeCompleto : "-",

                    Contador = contador != null ? contador.NumeroContador : "-",

                    Leitura = consumo.Data.ToString("dd/MM/yyyy"),

                    Consumo = consumo.ConsumoCalculado.ToString("N2") + " m³",

                    Estado = consumo.IdFatura == null ? "Por faturar" : "Faturado"
                });
            }

            dgAtividade.ItemsSource = lista;
        }
    }
}