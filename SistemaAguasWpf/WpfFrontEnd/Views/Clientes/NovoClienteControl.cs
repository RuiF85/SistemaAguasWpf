using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfFrontEnd.Models;
using WpfFrontEnd.Services;

namespace WpfFrontEnd.Views.Clientes
{

    /// <summary>
    /// Interaction logic for NovoClienteControl.xaml
    /// </summary>
    public partial class NovoClienteControl : UserControl
    {
        private ClienteApiService clienteService = new ClienteApiService();

        public NovoClienteControl()
        {
            InitializeComponent();
        }

        public async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
                string nome = txtNome.Text.Trim();
                string apelido = txtApelido.Text.Trim();
                string nif = txtNif.Text.Trim();
                string contacto = txtContacto.Text.Trim();
                string email = txtEmail.Text.Trim();
                string morada = txtMorada.Text.Trim();
                string codigoPostal = txtCodigoPostal.Text.Trim();
                string localidade = txtLocalidade.Text.Trim();


            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(apelido) ||
                string.IsNullOrWhiteSpace(nif) ||
                string.IsNullOrWhiteSpace(contacto) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(morada) ||
                string.IsNullOrWhiteSpace(codigoPostal) ||
                string.IsNullOrWhiteSpace(localidade))
            {
                
                MessageBox.Show("Preencha todos os campos obrigatórios.", "Campos obrigatórios",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            if (txtNif.Text.Length != 9 || !txtNif.Text.All(char.IsDigit))
            {
                MessageBox.Show("O NIF deve ter exatamente 9 dígitos.","NIF inválido",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtNif.Focus();
                return;
            }

            if (txtContacto.Text.Length != 9 ||
                !txtContacto.Text.All(char.IsDigit))
            {
                MessageBox.Show("O contacto deve ter exatamente 9 dígitos.", "Contacto inválido",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtContacto.Focus();
                return;
            }

            if (!txtEmail.Text.Contains("@") ||
                !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Introduza um email válido.", "Email inválido",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                txtEmail.Focus();
                return;
            }

            if (txtCodigoPostal.Text.Length != 8 ||  txtCodigoPostal.Text[4] != '-' ||
                !codigoPostal.Substring(0, 4).All(char.IsDigit) || !codigoPostal.Substring(5, 3).All(char.IsDigit)) 
            {
                MessageBox.Show("O código postal deve ter o formato 1234-567.",
                    "Código postal inválido",
                    MessageBoxButton.OK,MessageBoxImage.Warning);

                txtCodigoPostal.Focus();
                return;
            }

            Cliente cliente = new Cliente
            {
                Nome = nome,
                Apelido = apelido,
                Nif = nif,
                Contacto = contacto,
                Email = email,
                Morada = morada,
                CodigoPostal = codigoPostal,
                Localidade = localidade,
                Ativo = chkAtivo.IsChecked == true
            };

            bool sucesso = await clienteService.CriarCliente(cliente);

            if (sucesso)
            {
                MessageBox.Show("Cliente criado com sucesso.", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                LimparCampos();
            }
            else
            {
                MessageBox.Show("Não foi possível criar o cliente.", "Erro",
                    MessageBoxButton.OK,  MessageBoxImage.Error);
            }
        }

        private void BtnLimpar_Click( object sender, RoutedEventArgs e)
        {
            LimparCampos();
        }

        private void LimparCampos()
        {
            txtNome.Clear();
            txtApelido.Clear();
            txtNif.Clear();
            txtContacto.Clear();
            txtEmail.Clear();
            txtMorada.Clear();
            txtCodigoPostal.Clear();
            txtLocalidade.Clear();

            chkAtivo.IsChecked = true;

            txtNome.Focus();
        }
    }
}
