using System;

namespace WpfFrontEnd.Models
{
    public class Fatura
    {
        public int IdFatura { get; set; }

        public int IdCliente { get; set; }

        public int IdContador { get; set; }

        public DateTime DataFatura { get; set; }

        public decimal Consumo { get; set; }

        public decimal ValorTotal { get; set; }

        public int IdEstadoFatura { get; set; }

        public string NomeCliente { get; set; }

        public string NumeroContador { get; set; }

        public string Estado {  get; set; }
    }
}
