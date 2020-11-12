using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LA_VENDUTA.Models;
using LA_VENDUTA.Class;

namespace LA_VENDUTA.Controllers
{
    [Authorize(Roles = RoleName.Comprador)]
    public class Producto_VendidoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Producto_Vendido/Create
        public ActionResult Create(int carritoId=-1,int Total=0)
        {
            
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            IEnumerable<TarjetaDeCredito> tarjetas = QuerysGeneric.GetTarjetas(comprador.Tarjetas);
            Carrito carrito;
            if (carritoId!=-1)
                carrito = db.Carritos.Find(carritoId);
            ViewBag.TarjetaDeCreditoId = new SelectList(tarjetas, "NumDeTarjeta", "NumDeTarjeta", new Producto_Vendido());
            ViewBag.Total = Total;
            ViewBag.carritoId = carritoId;
            return View(new Producto_Vendido());
        }

        // POST: Producto_Vendido/Create
        [HttpPost]
        public async Task<ActionResult> Create(Producto_Vendido producto_Vendido, int carritoId,int Total)
        {
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            IEnumerable<TarjetaDeCredito> tarjetas = QuerysGeneric.GetTarjetas(comprador.Tarjetas);
            ViewBag.NumDesuTarjeta = new SelectList(tarjetas, "NumDeTarjeta", "NumDeTarjeta", new Producto_Vendido());
            producto_Vendido.NumDesuTarjeta = db.Tarjetas.Find(producto_Vendido.TarjetaDeCreditoId);
            TarjetaDeCredito tarjeta = producto_Vendido.NumDesuTarjeta;
            if (tarjeta.CreditoDisponible < Total)
            {
                return RedirectToAction("Index", "Anuncios", new { nameAnuncio = "ErrorCompra" });
            }
            tarjeta.CreditoDisponible = tarjeta.CreditoDisponible - Total;
            tarjeta.CreditoContable = tarjeta.CreditoContable - Total;
            db.Entry(tarjeta).State = EntityState.Modified;

          if (carritoId != -1)
            {
                
                Carrito carrito = db.Carritos.Find(producto_Vendido.CarritoId);
                producto_Vendido.CarritoId = carritoId;
                producto_Vendido.Cantidad = carrito.Cantidad;
                producto_Vendido.AnuncioId = carrito.AnuncioId;
                producto_Vendido.CompradorId = carrito.CompradorId;
                producto_Vendido.FechaSeparado = carrito.FechaSeparado;

                if (ModelState.IsValid)
                {
                    db.Producto_Vendido.Add(producto_Vendido as Producto_Vendido);
                    db.Carritos.Remove(carrito);
                    
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Anuncios", new { nameAnuncio = "AcceptCompra" });
                }

            }
            else
            {

                var carros = QuerysGeneric.Get_N_Obj_Mientras(db.Carritos, null, m => (m.CompradorId == comprador.Id && !(m is Producto_Vendido))).ToList();
               
                foreach (var item in carros)
                {
                    Producto_Vendido producto = new Producto_Vendido();
                    producto.CarritoId = carritoId;
                    producto.Cantidad = item.Cantidad;
                    producto.AnuncioId = item.AnuncioId;
                    producto.CompradorId = item.CompradorId;
                    producto.FechaSeparado = item.FechaSeparado;

                    if (ModelState.IsValid)
                    {
                        db.Producto_Vendido.Add(producto as Producto_Vendido);
                        db.Carritos.Remove(item);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index","Anuncios",new { nameAnuncio = "AcceptCompra" });
                    }

                }
                
            }
            return RedirectToAction("Index", "Anuncios", new { nameAnuncio = "ErrorCompra" });
        }
        
        public ActionResult CrearTarjeta()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CrearTarjeta([Bind(Include = "tarjeta")] double tarjeta)
        {
            if (tarjeta > 0)
            {
                Comprador user;
                if (User.IsInRole(RoleName.Comprador))
                {
                    int credito = QuerysGeneric.random.Next(100, 1000);
                    TarjetaDeCredito tarjetaDeCredito = await db.Tarjetas.FindAsync(tarjeta);
                    if (tarjetaDeCredito == null)
                    {
                        tarjetaDeCredito = new TarjetaDeCredito()
                        {
                            NumDeTarjeta = tarjeta,
                            CreditoDisponible = credito,
                            CreditoContable = credito

                        };
                        db.Tarjetas.Add(tarjetaDeCredito);
                        await db.SaveChangesAsync(); 
                    }

                    user = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
                    UsuarioConTarjeta usuario = new UsuarioConTarjeta()
                    {
                        Comprador = user,
                        CompradorId = user.Id,
                        TarjetadeCredito = tarjetaDeCredito
                    };
                    db.UsuarioConTarjetas.Add(usuario);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home", new { nameAnuncio = "AcceptTarjeta" });
                }
                

            }
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
