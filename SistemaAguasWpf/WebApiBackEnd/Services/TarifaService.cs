using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiBackEnd.Services
{

    public class TarifaService
    {
        public decimal CalcularValorFatura(decimal consumo)
        {
            decimal valorTotal = 0;

            if (consumo <= 5)
            {
                valorTotal = consumo * 0.30m;
            }
            else if (consumo <= 15)
            {
                valorTotal = 5 * 0.30m;
                valorTotal += (consumo - 5) * 0.80m;
            }
            else if (consumo <= 25)
            {
                valorTotal = 5 * 0.30m;
                valorTotal += 10 * 0.80m;
                valorTotal += (consumo - 15) * 1.20m;
            }
            else
            {
                valorTotal = 5 * 0.30m;
                valorTotal += 10 * 0.80m;
                valorTotal += 10 * 1.20m;
                valorTotal += (consumo - 25) * 1.60m;
            }

            return valorTotal;
        }
    }

}