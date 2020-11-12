using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LA_VENDUTA.Models
{
    public class Producto
    {
        [Key]
        public int ProductoId { get; set; }


        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [Display(Name="Nombre Del Producto")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "Rellenar este campo es obligatorio")]
        [Display(Name = "Tipo del Producto")]
        public Tipo Tipo { get; set; }

        [Display(Name = "Especificacion del Tipo")]
        public int Especificacion { get; set; }
        public virtual List<Producto_Vendido> Productos { get; set; } 

    }

    public enum Tipo
    {
        Electrodomesticos,
        Computacion_y_Electronica,
        Utiles_del_Hogar,
        Productos_Alimenticios,
        Vestimenta,
        Salud,
        Libros,
        Automotriz,
        Vivienda,

    }

    enum Electrodomesticos
    {
        Televisor,
        Aire_Acondicionado_O_Split,
        Batidora,
        Refrigerador,
        Lavadora,
        Ventilador,
        Microondas,
        Cocina
        
    }

    enum Computacion_y_Electronica
    {
        Laptop,
        PC_De_Escritorio,
        Piezas,
        Moviles,
        Tablet,
        Equipos_De_Musica,
        Conexion,
        Consola_Videojuego

    }

    enum Vestimenta
    {
        Bisuteria,
        Peleteria,
        Ropa
    }

    enum Automotriz
    {
        Carro,
        Moto,
        Moto_Electrica,
        Bicicleta,
        Piezas
    }

    enum Vivienda
    {
       Casa,
       Apartamento,
       Casa_En_La_Playa,
       Finca 
    }

}