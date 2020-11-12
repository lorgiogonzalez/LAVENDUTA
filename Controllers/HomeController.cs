using LA_VENDUTA.Class;
using LA_VENDUTA.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace LA_VENDUTA.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string nameAnuncio = "")
        {
            
            if (nameAnuncio == "")
            {
                db.RevisarSubastas();
                Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
                Vendedor vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
                if (comprador != null)
                { nameAnuncio = comprador.Mensaje;
                    comprador.Mensaje = "";
                    db.Entry(comprador).State = EntityState.Modified;
                }
                if (vendedor != null)
                {
                    nameAnuncio = vendedor.Mensaje;
                    vendedor.Mensaje = "";
                    db.Entry(vendedor).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            var Anuncios = QuerysGeneric.Get_N_Obj_Mientras<Anuncio>(db.Anuncios, 10,m=> m.Cantidad>0 && m.SoloParaSubasta==false).OrderBy(m=>m.Calificacion);
            var Subastas = QuerysGeneric.Get_N_Obj_Ordenado_Por<Subasta, int>(db.Subastas.Include(a => a.Anuncio).Include(a => a.Comprador).Include(a => a.Anuncio.Producto).Include(a=>a.Anuncio.Files), 3, m => m.PujaActual,true);
            var Rebajas = QuerysGeneric.Get_N_Obj_Mientras<Anuncio>(db.Anuncios, 2, m => m.Precio > m.NuevoPrecio && m.Cantidad>0 && m.SoloParaSubasta == false);
            return View(new Tuple<IEnumerable<Anuncio>,IEnumerable<Subasta>,IEnumerable<Anuncio>,string>(Anuncios,Subastas,Rebajas, nameAnuncio));
        }
        public ActionResult IndexAccept(string nameAnuncio)
        {
            var Anuncios = QuerysGeneric.Get_N_Obj_Mientras<Anuncio>(db.Anuncios, 10, m => m.Cantidad > 0).OrderBy(m => m.Calificacion);
            var Subastas = QuerysGeneric.Get_N_Obj_Ordenado_Por<Subasta, int>(db.Subastas, 3, m => m.PujaActual, true);
            var Rebajas = QuerysGeneric.Get_N_Obj_Mientras<Anuncio>(db.Anuncios, 2, m => m.Precio > m.NuevoPrecio);
            return View(new Tuple<IEnumerable<Anuncio>, IEnumerable<Subasta>, IEnumerable<Anuncio>,string>(Anuncios, Subastas, Rebajas, nameAnuncio));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        
    }
}