using System;
using System.Threading.Tasks;
using System.Windows;
using WpfFrontEnd.Services;
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
            MessageBox.Show("Clientes");
        }

        private void BtnContadores_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Contadores");

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
            MessageBoxResult resposta = MessageBox.Show("deseja sair da aplicação?","Sair",
                MessageBoxButton.YesNo,MessageBoxImage.Question);

            if (resposta == MessageBoxResult.Yes)
            {
                Close();
            }

        }
    }
}
