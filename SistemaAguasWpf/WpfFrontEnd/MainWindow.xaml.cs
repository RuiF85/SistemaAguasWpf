using System.Windows;
using WpfFrontEnd.Views.Clientes;
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

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            if(SubMenuClientes.Visibility == Visibility.Collapsed)
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

        private void NovoContadorControl_Click(object sender, RoutedEventArgs e)
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






        private void BtnConsumos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Consumos");
        }

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
