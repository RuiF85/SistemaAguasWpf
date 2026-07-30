using System.Windows;
using WpfFrontEnd.Views.Clientes;
using WpfFrontEnd.Views.Consumos;
using WpfFrontEnd.Views.Contadores;
using WpfFrontEnd.Views.Dashboard;

namespace WpfFrontEnd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            conteudoPrincipal.Content = new DashboardControl();
        }

        #region Clientes USerControl.
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

        #region Contadores UserControl.
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

        #region Consumos UserControl.
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

        private void BtnFaturas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Faturas");

        }



        private void BtnSobre_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sobre");
        }


        private void BtnSair_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult resposta = MessageBox.Show("deseja sair da aplicação?", "Sair",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (resposta == MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}
