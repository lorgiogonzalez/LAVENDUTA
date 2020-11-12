using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Producto_Vendido:Carrito
    {
        public TarjetaDeCredito NumDesuTarjeta { get; set; }
        
        public double? TarjetaDeCreditoId { get; set; }
    }
}