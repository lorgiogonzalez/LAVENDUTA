using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Web;
using System.Web.Mvc;

namespace LA_VENDUTA.Models
{
    public class Anuncio
    {
        public virtual Producto Producto { get; set; }
        public virtual int ProductoId { get; set; }
        [Key]
        public int AnuncioId { get; set; }
        [Required]
        [Display(Name = "Upload File")]

        public virtual Vendedor Vendedor { get; set; }

        public virtual int VendedorId { get; set; }

        
        [Display(Name = "Precio del Producto")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int Precio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Cantidad de Productos")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int Cantidad { get; set; }

        public int Calificacion { get; set; }
        [Required]
        public virtual ICollection<File> Files { get; set; }
        public string Etiqueta { get; set; }

        public string Descripcion { get; set; }

        public List<string> Comentarios { get; set; }


        [Display(Name = "ProductosDeseados")]
        public virtual List<Carrito> Deseados { get; set; }

        public bool SoloParaSubasta { get; set; }

        public int NuevoPrecio { get; set; }

    }
}