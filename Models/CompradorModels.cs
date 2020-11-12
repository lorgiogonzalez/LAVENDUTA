using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Comprador:Usuario
    {
        
        [Display(Name = "Numeros de Tarjeta")]
        public virtual List<UsuarioConTarjeta> Tarjetas { get; set; }
        public virtual List<Subasta> ParticpasSubasta { get; set; }
        public virtual List<Carrito> ProductosEnCompra { get; set; }


    }
}