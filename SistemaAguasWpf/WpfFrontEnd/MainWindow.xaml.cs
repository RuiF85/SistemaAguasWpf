using System;
using System.Threading.Tasks;
using System.Windows;
using WpfFrontEnd.Services;

namespace WpfFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClienteApiService clienteService = new ClienteApiService();

        public MainWindow()
        {
            InitializeComponent();

            CarregarClientes();
        }

        private async void CarregarClientes()
        {
            var clientes = await clienteService.ObterClientes();
            
            dgClientes.ItemsSource = clientes;
        }
    }
}
