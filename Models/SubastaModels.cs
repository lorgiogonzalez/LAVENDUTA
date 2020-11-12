using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Subasta
    {
        public virtual Anuncio Anuncio { get; set; }
        public int AnuncioId { get; set; }
        [Key]
        public int SubastaId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Inicio")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        

        [Display(Name = "Puja Inicial")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int PujaInicial { get; set; }

        [Display(Name = "Duracion en Dias")]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public int Duracion { get; set; }

        public int  PujaActual { get; set; }

        public virtual Comprador Comprador { get; set; }

        public virtual UsuarioConTarjeta UsuarioConTarjeta { get; set; }

        public List<string> Comentarios { get; set; }

        public DateTime FechadeCulminacion { get { return Fecha.AddDays(Duracion); } }
    }
}