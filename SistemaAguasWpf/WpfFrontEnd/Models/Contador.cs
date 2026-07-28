using System;

namespace WpfFrontEnd.Models
{
    public class Contador
    {
        public int IdContador { get; set; }

        public int IdCliente { get; set; }

        public string NumeroContador { get; set; }

        public DateTime DataInstalacao { get; set; }

        public bool Ativo { get; set; }

    }
}
