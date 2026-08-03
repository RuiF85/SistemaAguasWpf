namespace WpfFrontEnd.Models
{
    public class EscalaoFaturaView
    {
        public string Escalao { get; set; }
        public string Intervalo { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ConsumoAplicado { get; set; }

        public decimal Total
        {
            get
            {
                return ValorUnitario * ConsumoAplicado;
            }
        }

    }
}
