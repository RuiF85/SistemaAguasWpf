using System;
using System.Windows.Controls;

namespace WpfFrontEnd.Views.Dashboard
{
    /// <summary>
    /// Interaction logic for SobreControl.xaml
    /// </summary>
    public partial class SobreControl : UserControl
    {
        public SobreControl()
        {
            InitializeComponent();


            // Data de hoje, sempre atualizada automaticamente
            TxtData.Text = DateTime.Now.ToString("dd/MM/yyyy");


            // Ano do copyright, também dinâmico
            TxtCopyright.Text = $"© {DateTime.Now.Year} - Todos os direitos reservados";
        }

    }
}

