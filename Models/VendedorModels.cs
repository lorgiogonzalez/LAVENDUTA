using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Vendedor: Usuario
    {
        [DataType(DataType.CreditCard)]
        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        public virtual double TarjetadeCreditoId { get; set; }
        public virtual TarjetaDeCredito TarjetaDeCredito { get; set; }

    }
}