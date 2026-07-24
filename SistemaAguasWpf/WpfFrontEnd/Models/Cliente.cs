using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfFrontEnd.Models
{
    public class Cliente
    {
        public int  IdCliente   { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        public string Apelido { get; set; }
        [Required]
        public string Morada { get; set; }
        [Required]
        public string Nif {  get; set; }
        [Required]
        public string Contacto { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CodigoPostal { get; set; }
        [Required]
        public string Localidade { get; set; }
        [Required]
        public bool Ativo {  get; set; }

    }
}
