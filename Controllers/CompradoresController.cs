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


namespace LA_VENDUTA.Controllers
{
    [Authorize(Roles = RoleName.Admin)]
    public class CompradoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Compradores
        public async Task<ActionResult> Index()
        {
            return View(await db.Compradores.ToListAsync());
        }

        // GET: Compradores/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comprador comprador = await db.Compradores.FindAsync(id);
            if (comprador == null)
            {
                return HttpNotFound();
            }
            return View(comprador);
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
