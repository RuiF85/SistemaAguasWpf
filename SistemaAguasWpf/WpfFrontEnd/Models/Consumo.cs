using System;

namespace WpfFrontEnd.Models
{
    public class Consumo
    {
        public int IdConsumo { get; set; }

        public int IdContador { get; set; }

        public DateTime Data { get; set; }

        public decimal LeituraAtual { get; set; }

        public decimal ConsumoCalculado { get; set; }

        public int? IdFatura { get; set; }  //? corresponde null no SqlS


    }
}
