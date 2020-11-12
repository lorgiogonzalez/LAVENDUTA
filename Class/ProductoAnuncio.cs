using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web;
using System.Web.Mvc;

using LA_VENDUTA.Models;

namespace LA_VENDUTA.Class
{
    public class ProductoAnuncio
    {
       public int ProductoId { get; set; }
       public int AnuncioId { get; set; }

        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [Display(Name = "Nombre Del Producto")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [Display(Name = "Tipo del Producto")]
        public Tipo Tipo { get; set; }
      
        public int Especificacion { get; set; }

        [Display(Name = "Precio del Producto")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int Precio { get; set; }

        [Display(Name = "Cantidad de Productos")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int Cantidad { get; set; }

        public string Etiqueta { get; set; }

        public string Descripcion { get; set; }


    }
}