using System.Windows;
using WpfFrontEnd.Views.Clientes;
using WpfFrontEnd.Views.Consumos;
using WpfFrontEnd.Views.Contadores;
using WpfFrontEnd.Views.Dashboard;
using WpfFrontEnd.Views.Faturas;

namespace WpfFrontEnd
{
    /// <summary>
    /// Main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            conteudoPrincipal.Content = new DashboardControl();
        }

        /// <summary>
        /// Shows or hides the Clients submenu.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        #region Clientes USerControl
        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            if (SubMenuClientes.Visibility == Visibility.Collapsed)
            {
                SubMenuClientes.Visibility = Visibility.Visible;
            }
            else
            {
                SubMenuClientes.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnNovoCliente_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new NovoClienteControl();
        }

        private void BtnEditarCliente_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new EditarClienteControl();
        }

        private void BtnApagarCliente_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new ApagarClienteControl();
        }
        #endregion

        /// <summary>
        /// Shows or hides the Clients submenu.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        #region Contadores UserControl
        private void BtnContadores_Click(object sender, RoutedEventArgs e)
        {
            if (SubMenuContadores.Visibility == Visibility.Collapsed)
            {
                SubMenuContadores.Visibility = Visibility.Visible;
            }
            else
            {
                SubMenuContadores.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnNovoContador_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new NovoContadorControl();
        }

        private void BtnEditarContador_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new EditarContadorControl();
        }

        private void BtnApagarContador_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new ApagarContadorControl();
        }
        #endregion

        /// <summary>
        /// Opens the Edit Client view.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        #region Consumos UserControl
        private void BtnConsumos_Click(object sender, RoutedEventArgs e)
        {
            if (SubMenuConsumos.Visibility == Visibility.Collapsed)
            {
                SubMenuConsumos.Visibility = Visibility.Visible;
            }
            else
            {
                SubMenuConsumos.Visibility = Visibility.Collapsed;
            }
        }

        private void BtnNovoConsumo_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new NovoConsumoControl();
        }

        private void BtnEditarConsumo_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new EditarConsumoControl();
        }

        private void BtnApagarConsumo_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new ApagarConsumoControl();
        }
        #endregion

        /// <summary>
        /// Opens the Delete Client view.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        #region Fatura UserControl
        private void BtnFaturas_Click(object sender, RoutedEventArgs e)
        {
            if (SubMenuFaturas.Visibility == Visibility.Collapsed)
            {
                SubMenuFaturas.Visibility = Visibility.Visible;
            }
            else
            {
                SubMenuFaturas.Visibility = Visibility.Collapsed;
            }
        }
        private void BtnGerarFaturas(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new GerarFaturaControl();
        }
        private void BtnGerirFaturas(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new GerirFaturasControl();
        }
        #endregion

        /// <summary>
        /// Opens the About view.   
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnSobre_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new SobreControl();
        }

        /// <summary>
        /// Opens the Dashboard view.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {
            conteudoPrincipal.Content = new DashboardControl();
        }

        /// <summary>
        /// Closes the application after user confirmation.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resposta = MessageBox.Show("Deseja sair da aplicação?", "Sair",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resposta == MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}
