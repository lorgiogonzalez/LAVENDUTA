using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using LA_VENDUTA.Class;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LA_VENDUTA.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public Roles roles { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }
        public void RevisarSubastas()
        {
            var subastas = QuerysGeneric.Get_N_Obj_Mientras(Subastas.Include(m=>m.Comprador).Include(m=>m.Anuncio.Vendedor).Include(m=>m.Anuncio.Vendedor.TarjetaDeCredito).Include(m=>m.UsuarioConTarjeta.TarjetadeCredito).Include(m=>m.Anuncio).Include(m=>m.Anuncio.Producto), null, m => m.FechadeCulminacion <= DateTime.Now);
            foreach (var sub in subastas)
            {
                if (sub.Comprador != null)
                {
                    sub.Comprador.Mensaje = sub.Comprador.Mensaje + "@Ha Ganado La Subasta del Producto: " + sub.Anuncio.Producto.Nombre;
                    sub.UsuarioConTarjeta.TarjetadeCredito.CreditoContable = sub.UsuarioConTarjeta.TarjetadeCredito.CreditoContable - sub.PujaActual;
                    TarjetaDeCredito tar = sub.Anuncio.Vendedor.TarjetaDeCredito;
                    tar.CreditoContable += sub.PujaActual;
                    tar.CreditoDisponible += sub.PujaActual;
                    sub.Anuncio.Vendedor.Mensaje = sub.Anuncio.Vendedor.Mensaje + "@Se ha vendido su subasta del Producto: " + sub.Anuncio.Producto.Nombre + " en " + sub.PujaActual;
                    Entry(sub).State = EntityState.Modified;
                }
            }
           Subastas.RemoveRange(subastas);
           SaveChanges();
        }
        public static ApplicationDbContext Create()
        {
           
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Anuncio> Anuncios { get; set; }
        
        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Producto> Productos { get; set; }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Vendedor> Vendedores { get; set; }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Carrito> Carritos { get; set; }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Comprador> Compradores { get; set; }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Producto_Vendido> Producto_Vendido { get; set; }

        public System.Data.Entity.DbSet<LA_VENDUTA.Models.Subasta> Subastas { get; set; }

        public DbSet<File> Files { get; set; }
        public DbSet<TarjetaDeCredito> Tarjetas { get; set; }
        public DbSet<UsuarioConTarjeta> UsuarioConTarjetas { get; set; }
    }
}