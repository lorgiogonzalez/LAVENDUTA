using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LA_VENDUTA.Models
{
    public abstract class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        public int Calificacion { get; set; }

        [Required]
        public string User { get; set; }

        public string Mensaje { get; set; }
    }
}