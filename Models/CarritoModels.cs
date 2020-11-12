using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Carrito
    {
        public virtual Comprador Comprador { get; set; }
        public virtual int CompradorId { get; set; }
        public virtual Anuncio Anuncio { get; set; }
        public virtual int AnuncioId { get; set; }
        [Key]
        public int CarritoId { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaSeparado { get; set; }
       
    }
}