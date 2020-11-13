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
    public class SubastasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [AllowAnonymous]
        // GET: Subastas
        public async Task<ActionResult> Index(int vista = 0, int filtros1 = -1, int filtros2 = -1)
        {
            IEnumerable<Subasta> subastas;
            if (filtros1 == -1)
            {
                subastas = await db.Subastas.Include(a => a.Anuncio).Include(a => a.Comprador).Include(a => a.Anuncio.Producto).ToListAsync();
            }
            else if (filtros2 == -1)
            {
                subastas = QuerysGeneric.Get_N_Obj_Mientras(db.Subastas.Include(a => a.Anuncio).Include(a => a.Comprador).Include(a=>a.Anuncio.Producto), null, m => ((int)m.Anuncio.Producto.Tipo == filtros1));
            }
            else
            {
                subastas = QuerysGeneric.Get_N_Obj_Mientras(db.Subastas.Include(a => a.Anuncio).Include(a => a.Comprador).Include(a => a.Anuncio.Producto), null, m => ((int)m.Anuncio.Producto.Tipo == filtros1 && m.Anuncio.Producto.Especificacion == filtros2));
            }
            return View(new Tuple<IEnumerable<Subasta>,int,int,int>(subastas, vista, filtros1, filtros2));
        }
        [Authorize(Roles = RoleName.Comprador)]
        // GET: Subastas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            IEnumerable<TarjetaDeCredito> tarjetas = QuerysGeneric.GetTarjetas(comprador.Tarjetas);
            ViewBag.TarjetaDeCreditoId = new SelectList(tarjetas, "NumDeTarjeta", "NumDeTarjeta", new Producto_Vendido());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = await db.Subastas.FindAsync(id);
            if (subasta == null)
            {
                return HttpNotFound();
            }
            return View(subasta);
        }
        [Authorize(Roles = RoleName.Vendedor)]
        // GET: Subastas/Create
        public ActionResult Create(int id)
        {
            Subasta subasta = new Subasta() { AnuncioId = id, Anuncio = db.Anuncios.Find(id) };
            return View(subasta);
        }
        public async Task<ActionResult> ElegirAnuncio()
        {
            var vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
            var anuncios = db.Anuncios.Where(m=>m.VendedorId==vendedor.Id).Include(a => a.Producto).Include(a => a.Vendedor);
            return View(new Tuple<IEnumerable<LA_VENDUTA.Models.Anuncio>, int>(await anuncios.ToListAsync(),0));
        }
        [Authorize(Roles = RoleName.Vendedor)]
        // POST: Subastas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "SubastaId,AnuncioId,Fecha,PujaInicial,Duracion,PujaActual,CompradorId,NumDesuTarjeta")] Subasta subasta,int AnuncioId)
        {
            subasta.AnuncioId = AnuncioId; 
            subasta.Anuncio = db.Anuncios.Find(AnuncioId);
            subasta.Fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Subastas.Add(subasta);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AnuncioId = new SelectList(db.Anuncios, "AnuncioId", "Etiqueta", subasta.AnuncioId);
            ViewBag.CompradorId = new SelectList(db.Compradores, "CompradorId", "Nombre", subasta.Comprador);
            return View(subasta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Comprador)]
        public async Task<ActionResult> Puja(int? subastaId, [Bind(Include = "puja")] int puja,[Bind(Include = "TarjetaDeCreditoId")] double TarjetaDeCreditoId)
        {
            if (subastaId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subasta subasta = await db.Subastas.FindAsync(subastaId);
            TarjetaDeCredito tarjetaDeCredito = db.Tarjetas.Find(TarjetaDeCreditoId);
            if (subasta.PujaActual<puja&&tarjetaDeCredito.CreditoDisponible>=puja)
            {
                Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
                UsuarioConTarjeta usuarioconTarjeta = subasta.UsuarioConTarjeta;
                if (usuarioconTarjeta != null)
                    usuarioconTarjeta.TarjetadeCredito.CreditoDisponible += subasta.PujaActual;
                usuarioconTarjeta = db.UsuarioConTarjetas.Where(m => (m.CompradorId == comprador.Id && m.TarjetadeCredito.NumDeTarjeta == TarjetaDeCreditoId)).ToList()[0];
                subasta.PujaActual = puja;
                subasta.UsuarioConTarjeta = usuarioconTarjeta;
                tarjetaDeCredito.CreditoDisponible = tarjetaDeCredito.CreditoDisponible - puja;
                subasta.Comprador = comprador;
                db.Entry(subasta).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index","Home", new { nameAnuncio = "AcceptSubasta" });
            }

            ViewBag.AnuncioId = new SelectList(db.Anuncios, "AnuncioId", "Etiqueta", subasta.AnuncioId);
            ViewBag.CompradorId = new SelectList(db.Compradores, "CompradorId", "Nombre", subasta.Comprador);
            return RedirectToAction("Index","Home",new { nameAnuncio = "ErrorSubasta" });
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
