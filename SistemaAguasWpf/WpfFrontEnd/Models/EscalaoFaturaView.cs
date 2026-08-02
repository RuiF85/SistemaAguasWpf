using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
