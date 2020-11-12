using LA_VENDUTA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LA_VENDUTA.Controllers
{
    [AllowAnonymous]
    public class FilesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Files
        public ActionResult Index(int id)
        {
            var files = db.Files.Find(id);
            return File(files.Content, files.ContentType);
        }
    }
}