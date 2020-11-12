using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LA_VENDUTA.Models
{
    public class UsuarioConTarjeta
    {
        [Key]
        public int id { get; set; }

        public virtual int CompradorId { get; set; }
         public virtual Comprador Comprador { get; set; }
        
        

        public virtual TarjetaDeCredito TarjetadeCredito{ get; set; }
    }
}