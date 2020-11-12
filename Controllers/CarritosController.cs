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
    public class CarritosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Carritos
        public  ActionResult Index()
        {
            var comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            var carritoes = Articulos(comprador.Id);
            return View(carritoes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ModifyCant([Bind(Include = "Cantidad")]int Cantidad, int CarritoId)
        {
            
            Carrito carrito = await db.Carritos.FindAsync(CarritoId);
            Anuncio anuncio = carrito.Anuncio;
            anuncio.Cantidad += carrito.Cantidad;
            if (Cantidad > 0 && Cantidad <= carrito.Anuncio.Cantidad)
            {

                carrito.Cantidad = Cantidad;
                anuncio.Cantidad -= carrito.Cantidad;
            }
            else
            {
                await db.SaveChangesAsync();
                RedirectToAction("Index", "Home", new { nameAnuncio = "ErrorCantidad" });
            }
            db.Entry(carrito).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Carritos");
        }
        // GET: Carritos/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carrito carrito = await db.Carritos.FindAsync(id);
            if (carrito == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details","Anuncio",id);
        }

/// <summary>
/// 
/// </summary>
/// <param name="id"></param>
/// <param name="compra"></param>
/// <returns></returns>
        // GET: Carritos/Delete/5
        public async Task<ActionResult> Delete(int? id,bool compra=false,double numdeSuTarjeta=0)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Carrito carrito = db.Carritos.Find(id);
            if (carrito == null)
            {
                return HttpNotFound();
            }
            if (compra && !(carrito is Producto_Vendido))
            {
                
                //Producto_Vendido producto = new Producto_Vendido { Anuncio = carrito.Anuncio, AnuncioId = carrito.AnuncioId, Cantidad = carrito.Cantidad, CarritoId = carrito.CarritoId, Comprador = carrito.Comprador, CompradorId = carrito.CompradorId, FechaSeparado = DateTime.Now, NumDesuTarjeta = numdeSuTarjeta };
                //db.Carritos.Remove(carrito);
                //db.Producto_Vendido.Add(producto);
                //await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(carrito);
        }

        // POST: Carritos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Carrito carrito = await db.Carritos.FindAsync(id);
            db.Carritos.Remove(carrito);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="compro"></param>
        /// <returns></returns>
        public ActionResult VaciarCarrito(bool compro,double numedeTarjeta=0)
        {
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            foreach (var item in Articulos(comprador.Id))
            {
                db.Carritos.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IEnumerable<Carrito> Articulos(int id)
        {
            
            var carros = QuerysGeneric.Get_N_Obj_Mientras(db.Carritos, null, m => (m.CompradorId == id && !(m is Producto_Vendido))).ToList();
         
            return carros;
        }



    }
}
