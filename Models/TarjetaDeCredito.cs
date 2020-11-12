using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LA_VENDUTA.Models
{
    public class TarjetaDeCredito
    {
        [DataType(DataType.CreditCard)]
        [Display(Name = "Numero de Tarjeta")]
        [Key]
        public double NumDeTarjeta { get; set; }

        [Required]
        public double CreditoContable { get; set; }

        [Required]
        public double CreditoDisponible { get; set; }
    }
}