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
    [Authorize(Roles = RoleName.Admin)]
    public class VendedoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Vendedores
        public async Task<ActionResult> Index()
        {
            return View(await db.Vendedores.ToListAsync());
        }

        // GET: Vendedores/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendedor vendedor = await db.Vendedores.FindAsync(id);
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            return View(vendedor);
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
