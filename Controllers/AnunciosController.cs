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
using System.Drawing;
using System.Data.Entity.Infrastructure;
//using System.IO;

namespace LA_VENDUTA.Controllers
{
    
    public class AnunciosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Anuncios
        [AllowAnonymous]
        public async Task<ActionResult> Index(int vista=0,int filtros1=-1,int filtros2=-1,int filtros3=0,string nombre="",int precioMin=0,int PrecioMax=10000,string nameAnuncio="")
        {
            Vendedor vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
            IEnumerable<Anuncio> anuncios;
            if (filtros1 == -1)
            {
               
                    anuncios= QuerysGeneric.Get_N_Obj_Mientras(db.Anuncios.Include(a => a.Producto).Include(a => a.Vendedor), null, m => ((User.IsInRole(RoleName.Vendedor) && m.VendedorId == vendedor.Id) || m.Cantidad>0 )&& m.SoloParaSubasta == false);
               
            }
            else
            {
                if (filtros2 == -1)
                {

                    anuncios = QuerysGeneric.Get_N_Obj_Mientras(db.Anuncios.Include(a => a.Producto).Include(a => a.Vendedor), null, m => ((int)m.Producto.Tipo == filtros1 && ((User.IsInRole(RoleName.Vendedor) && m.VendedorId == vendedor.Id) || m.Cantidad > 0) && m.SoloParaSubasta == false));

                }
                else
                {

                    anuncios = QuerysGeneric.Get_N_Obj_Mientras(db.Anuncios.Include(a => a.Producto).Include(a => a.Vendedor), null, m => ((int)m.Producto.Tipo == filtros1 && m.Producto.Especificacion == filtros2 && ((User.IsInRole(RoleName.Vendedor) && m.VendedorId == vendedor.Id) || m.Cantidad > 0) && m.SoloParaSubasta == false));
                }
            }
            if (filtros3 != 0)
            {
                anuncios = anuncios.Where(m => m.Precio > m.NuevoPrecio);
            }
            if (nombre != "")
            {
                anuncios = anuncios.Where(m => m.Etiqueta.Contains(nombre)||m.Producto.Nombre.Contains(nombre)||m.Descripcion.Contains(nombre));
                vista = -1;
            }
            if (precioMin != 0 || PrecioMax != 10000)
            {
                vista = -1;
                anuncios = anuncios.Where(m => m.NuevoPrecio > precioMin && m.NuevoPrecio < PrecioMax);
            }
            
            return View(new Tuple<IEnumerable<Anuncio>, int,Filtros,string>(anuncios,vista,new Filtros(filtros1, filtros2,filtros3), nameAnuncio));
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Buscador([Bind(Include = "texto")] string texto, int filtros1 = -1, int filtros2 = -1, int filtros3 = 0)
        {
            return RedirectToAction("Index", new { filtros1 = filtros1, filtros2 = filtros2, filtros3 = filtros3, nombre = texto });
           
        }
        /// <summary>
        /// POST: /Anuncios/Index
        /// </summary>
        /// <param name="productoanuncio">Model parameter</param>
        /// <returns>Return - Response information</returns>

        // GET: Anuncios/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anuncio anuncio =  db.Anuncios.Include(s=>s.Files).SingleOrDefault(s=>s.AnuncioId==id);
            if (anuncio == null)
            {
                return HttpNotFound();
            }
            return View(anuncio);
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Details([Bind(Include = "Nombre,Tipo,Especificacion,Precio,Fecha,Cantidad,Calificacion,Etiqueta,Descripcion")] ProductoAnuncio productoanuncio,int cantidad)
        {
            Anuncio anuncio = db.Anuncios.Find(productoanuncio.AnuncioId);
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            if (anuncio.Cantidad < cantidad)
            {
                return RedirectToAction("Index");
            }
            else
            {
                anuncio.Cantidad = anuncio.Cantidad - cantidad;
                db.Carritos.Add(new Carrito { Comprador = comprador, CompradorId = comprador.Id, Anuncio = anuncio, AnuncioId = anuncio.AnuncioId, Cantidad = cantidad });
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Carritos");
        }
        [Authorize(Roles= RoleName.Vendedor)]
        // GET: Anuncios/Create
        public ActionResult Create()
        {

            return View();
        }
        [Authorize(Roles = RoleName.Vendedor)]
        public ActionResult CreateSolosubasta()
        {

            return View();
        }
        [HttpPost]
        [Authorize(Roles = RoleName.Vendedor)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateSolosubasta([Bind(Include = "Nombre,Tipo,Especificacion,Precio,Fecha,Cantidad,Calificacion,Etiqueta,Descripcion")] ProductoAnuncio productoanuncio, HttpPostedFileBase upload)
        {
            Anuncio anuncio = new Anuncio();
            anuncio.SoloParaSubasta = true;
            anuncio.Cantidad = productoanuncio.Cantidad;
            anuncio.Descripcion = productoanuncio.Descripcion;
            anuncio.Etiqueta = productoanuncio.Etiqueta;
            anuncio.Precio = productoanuncio.Precio;
            anuncio.NuevoPrecio = anuncio.Precio;
            anuncio.Fecha = DateTime.Now;
            anuncio.Vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
            anuncio.VendedorId = anuncio.Vendedor.Id;
            Producto producto = new Producto();
            producto.Nombre = productoanuncio.Nombre;
            producto.Tipo = productoanuncio.Tipo;
            string fileContent = string.Empty;
            string fileContentType = string.Empty;


            try
            {

                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        anuncio.Files = new List<File> { avatar };
                    }
                    List<Producto> productos = db.Productos.Where(m => (m.Nombre == producto.Nombre && m.Tipo == producto.Tipo)).ToList();
                    if (productos.Count != 0)
                    {
                        anuncio.Producto = productos[0];
                        anuncio.ProductoId = anuncio.Producto.ProductoId;
                    }
                    else
                    {
                        db.Productos.Add(producto);
                        anuncio.Producto = producto;
                        anuncio.ProductoId = producto.ProductoId;

                    }
                    db.Anuncios.Add(anuncio);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Create","Subastas",new { id=anuncio.AnuncioId});
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            //ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Nombre", anuncio.ProductoId);
            //ViewBag.VendedorId = new SelectList(db.Vendedores, "VendedorId", "Nombre", anuncio.VendedorId);
            return View(anuncio);
        }
        public JsonResult GetSpecification(string id)
        {
            var devolver = new List<SelectListItem>();
            var Vivienda = new List<SelectListItem>(){
        new SelectListItem()
        {
            Value="0", Text="Casa"
        },
        new SelectListItem()
        {
            Value="1", Text="Apartamento"
        },
        new SelectListItem()
        {
            Value="2", Text="Casa En La Playa"
        },
        new SelectListItem()
        {
            Value="3", Text="Finca"
        }

    };

            var Automotriz = new List<SelectListItem>(){
        new SelectListItem()
        {
            Value="0", Text="Carro"
        },
        new SelectListItem()
        {
            Value="1", Text="Moto"
        },
        new SelectListItem()
        {
            Value="2", Text="Moto Electrica"
        },
        new SelectListItem()
        {
            Value="3", Text="Bicicleta"
        },
        new SelectListItem()
        {
            Value="4", Text="Bicicleta Electrica"
        },
        new SelectListItem()
        {
            Value="5", Text="Piezas"
        }

    };

            var Vestimenta = new List<SelectListItem>(){
        new SelectListItem()
        {
            Value="0", Text="Bisuteria"
        },
        new SelectListItem()
        {
            Value="1", Text="Peleteria"
        },
        new SelectListItem()
        {
            Value="2", Text="Ropa"
        }

    };

            var CompElectr = new List<SelectListItem>(){
        new SelectListItem()
        {
            Value="0", Text="Laptop"
        },
        new SelectListItem()
        {
            Value="1", Text="PC de Escritorio"
        },
        new SelectListItem()
        {
            Value="2", Text="Piezas"
        },
        new SelectListItem()
        {
            Value="3", Text="Moviles"
        },
        new SelectListItem()
        {
            Value="4", Text="Tablet"
        },
        new SelectListItem()
        {
            Value="5", Text="Equipos de Musica"
        },
        new SelectListItem()
        {
            Value="6", Text="Conexión"
        },
        new SelectListItem()
        {
            Value="7", Text="Consola y VideoJuegos"
        }

    };

            var Electrodomes = new List<SelectListItem>(){
        new SelectListItem()
        {
            Value="0", Text="Televisor"
        },
        new SelectListItem()
        {
            Value="1", Text="Aire Acondicionado o Split"
        },
        new SelectListItem()
        {
            Value="2", Text="Batidora"
        },
        new SelectListItem()
        {
            Value="3", Text="Refrigerador"
        },
        new SelectListItem()
        {
            Value="4", Text="Lavadora"
        },
        new SelectListItem()
        {
            Value="5", Text="Ventilador"
        },
        new SelectListItem()
        {
            Value="6", Text="Microondas"
        },
        new SelectListItem()
        {
            Value="7", Text="Cocina"
        }

    };
            switch ((Tipo)int.Parse(id))
            {
                case Tipo.Automotriz:
                    {
                        devolver = Automotriz;
                        break;
                    }
                case Tipo.Vivienda:
                    {
                        devolver = Vivienda;
                        break;
                    }
                case Tipo.Vestimenta:
                    {
                        devolver = Vestimenta;
                        break;
                    }
                case Tipo.Computacion_y_Electronica:
                    {
                        devolver = CompElectr;
                        break;
                    }
                case Tipo.Electrodomesticos:
                    {
                        devolver = Electrodomes;
                        break;
                    }
                case Tipo.Libros:
                    {

                        break;
                    }
                case Tipo.Productos_Alimenticios:
                    {

                        break;
                    }
                case Tipo.Salud:
                    {

                        break;
                    }
                case Tipo.Utiles_del_Hogar:
                    {

                        break;
                    }
            }
            return Json(new SelectList(devolver, "Value", "Text"));
        }



        // POST: Anuncios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleName.Vendedor)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Nombre,Tipo,Especificacion,Precio,Fecha,Cantidad,Calificacion,Etiqueta,Descripcion")] 
        ProductoAnuncio productoanuncio, HttpPostedFileBase upload)
        {
            Anuncio anuncio = new Anuncio();
            anuncio.Cantidad = productoanuncio.Cantidad;
            anuncio.Descripcion = productoanuncio.Descripcion;
            anuncio.Etiqueta = productoanuncio.Etiqueta;
            anuncio.Precio = productoanuncio.Precio;
            anuncio.NuevoPrecio = anuncio.Precio;
            anuncio.Fecha = DateTime.Now;
            anuncio.Vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
            anuncio.VendedorId = anuncio.Vendedor.Id;
            Producto producto = new Producto();
            producto.Nombre = productoanuncio.Nombre;
            producto.Tipo = productoanuncio.Tipo;
            string fileContent = string.Empty;
            string fileContentType = string.Empty;


            try
            {

                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        var avatar = new File
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Avatar,
                            ContentType = upload.ContentType
                        };
                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            avatar.Content = reader.ReadBytes(upload.ContentLength);
                        }
                        anuncio.Files = new List<File> { avatar };
                    }
                    List<Producto> productos = db.Productos.Where(m => (m.Nombre == producto.Nombre && m.Tipo == producto.Tipo)).ToList();
                    if (productos.Count != 0)
                    {
                        anuncio.Producto = productos[0];
                        anuncio.ProductoId = anuncio.Producto.ProductoId;
                    }
                    else
                    {
                        db.Productos.Add(producto);
                        anuncio.Producto = producto;
                        anuncio.ProductoId = producto.ProductoId;

                    }
                    db.Anuncios.Add(anuncio);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(anuncio);
        }
        [Authorize(Roles = RoleName.Vendedor)]
        // GET: Anuncios/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Anuncio anuncio = await db.Anuncios.FindAsync(id);
            Producto producto = await db.Productos.FindAsync(anuncio.ProductoId);
            ProductoAnuncio productoAnuncio = new ProductoAnuncio();
            productoAnuncio.Cantidad = anuncio.Cantidad;
            productoAnuncio.Descripcion = anuncio.Descripcion;
            productoAnuncio.Especificacion = producto.Especificacion;
            productoAnuncio.Etiqueta = anuncio.Etiqueta;
            productoAnuncio.Nombre = producto.Nombre;
            productoAnuncio.Precio = anuncio.NuevoPrecio;
            productoAnuncio.Tipo = producto.Tipo;
            productoAnuncio.ProductoId = producto.ProductoId;
            productoAnuncio.AnuncioId = anuncio.AnuncioId;
            if (anuncio == null)
            {
                return HttpNotFound();
            }
           // ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Nombre", anuncio.ProductoId);
           // ViewBag.VendedorId = new SelectList(db.Vendedores, "VendedorId", "Nombre", anuncio.VendedorId);
            return View(productoAnuncio);
        }

        // POST: Anuncios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = RoleName.Vendedor)]
        public async Task<ActionResult> Edit([Bind(Include = "ProductoId,AnuncioId,Nombre,Tipo,Especificacion,Precio,Fecha,Cantidad,Calificacion,Etiqueta,Descripcion")] ProductoAnuncio productoanuncio)
        {
            Anuncio anuncio =await db.Anuncios.FindAsync(productoanuncio.AnuncioId);
            anuncio.Cantidad = productoanuncio.Cantidad;
            anuncio.Descripcion = productoanuncio.Descripcion;
            anuncio.Etiqueta = productoanuncio.Etiqueta;
            anuncio.NuevoPrecio = productoanuncio.Precio;
            anuncio.Fecha = DateTime.Now;
            anuncio.Vendedor = QuerysGeneric.GetVendedor(User.Identity.Name, db.Vendedores);
            anuncio.VendedorId = anuncio.Vendedor.Id;
            Producto producto =new Producto();
            producto.Nombre = productoanuncio.Nombre;
            producto.Tipo = productoanuncio.Tipo;
            
            HttpPostedFileBase file = Request.Files["ImageData"];
            //anuncio.Image = ConvertToBytes(file);
            if (ModelState.IsValid)
            {
                List<Producto> productos = db.Productos.Where(m => (m.Nombre == producto.Nombre && m.Tipo == producto.Tipo)).ToList();
                if (productos.Count != 0)
                {
                    anuncio.Producto = productos[0];
                    anuncio.ProductoId = anuncio.Producto.ProductoId;
                }
                else
                {
                    db.Productos.Add(producto);
                    anuncio.Producto = producto;
                    anuncio.ProductoId = producto.ProductoId;

                }
                db.Entry(anuncio).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           // ViewBag.ProductoId = new SelectList(db.Productos, "ProductoId", "Nombre", anuncio.ProductoId);
            // ViewBag.VendedorId = new SelectList(db.Vendedores, "VendedorId", "Nombre", anuncio.VendedorId);
            return View(anuncio);
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
        /// <param name="idAnuncio"></param>
        /// <param name="cantidad"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleName.Comprador)]
        public async  Task<ActionResult> AñadirAlCarrito([Bind(Include = "Cantidad")]int? cantidad,int idAnuncio,string controlador="Anuncios")
        {
            Anuncio anuncio = db.Anuncios.Find(idAnuncio);
            Comprador comprador = QuerysGeneric.GetComprador(User.Identity.Name, db.Compradores);
            if (anuncio.Cantidad < cantidad)
            {
                return RedirectToAction("Index",new {  nameAnuncio ="ErrorCantidad"});
            }
            else
            {
                var pedidos = QuerysGeneric.Get_N_Obj_Mientras(db.Carritos, 1, m => (m.CompradorId == comprador.Id && m.AnuncioId == idAnuncio && !(m is Producto_Vendido)));
                if (pedidos.Count() == 0) { 
                anuncio.Cantidad =anuncio.Cantidad - (cantidad==null?1:(int)cantidad);
                db.Carritos.Add(new Carrito { Comprador = comprador, CompradorId = comprador.Id, Anuncio = anuncio, AnuncioId = anuncio.AnuncioId, Cantidad = (cantidad == null ? 1 : (int)cantidad),FechaSeparado=DateTime.Now });
                }
                else
                {
                    int Cantidad = (cantidad == null ? 1 : (int)cantidad);
                    var carrito=pedidos.ToList()[0];
                    if (Cantidad > 0 && Cantidad <= carrito.Anuncio.Cantidad)
                    {
                        anuncio.Cantidad += carrito.Cantidad;
                        carrito.Cantidad += Cantidad;
                        anuncio.Cantidad -= carrito.Cantidad;
                        carrito.FechaSeparado = DateTime.Today;
                    }
                    else
                    {
                        return RedirectToAction("Index", controlador, new {  nameAnuncio = anuncio.Producto.Nombre });
                    }
                    db.Entry(carrito).State = EntityState.Modified;
                }
   
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index",controlador, new {  nameAnuncio = anuncio.Producto.Nombre });
        }

    }
}
