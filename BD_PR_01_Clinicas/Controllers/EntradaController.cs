using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class EntradaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        List<Volumen> volumenes = new List<Volumen> { (new Volumen { codVolumen = 1, volumen = "mg" }),
                                             (new Volumen { codVolumen = 2, volumen = "ml" }) };
        // GET: Entrada
        public ActionResult Index()
        {
            List<tbEntrada> lista = (from t in db.tbEntrada orderby t.fechaEntrada select t).ToList();
            return View(lista);
        }

        // GET: Entrada/Crear
        public ActionResult Crear()
        {
            List<tbPresentacion> presentaciones = (from t in db.tbPresentacion where t.estado == true orderby t.presentacion select t).ToList();
            List<tbCategoria> categorias = (from t in db.tbCategoria where t.estado == true orderby t.categoria select t).ToList();
            ViewBag.nCodPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            ViewBag.nCodCategoria = new SelectList(categorias, "codCategoria", "categoria");
            ViewBag.nCodVolumen = new SelectList(volumenes, "codVolumen", "volumen");
            return View(new Movimiento());
        }

        // POST: Entrada/Crear
        [HttpPost]
        public ActionResult Crear(Datos datos)
        {
            //aqui creo el objeto tbEntrada y todo lo del detalle, esto tu me lo enseñaste no voy a explicarlo jajajajajajajajaslkdjfa;sldkfja;lskdjf;
            tbEntrada entrada = new tbEntrada
            {
                descripcion = datos.descripcion,
                fechaEntrada = DateTime.Now
            };
            foreach (Item item in datos.detalle)
            {
                entrada.tbDetalleEntrada.Add(new tbDetalleEntrada
                {
                    codProducto = item.codProducto,
                    cantidad = item.cantidad
                });
            }
            db.tbEntrada.InsertOnSubmit(entrada);
            db.SubmitChanges();
            return RedirectToAction("Index");
        }

        // GET: Entrada/Productos
        public ActionResult Productos(string filtro = "")
        {
            //este es el codigo que se ejecuta cuando se habre el modal apachando el boton buscar, tiene un filtro como cualquier otro
            //pero al final, dice return PartialView("_Productos", lista), osea que llama a la vista parcial _Productos y le pasa la lista
            //como el modelo
            List<RegistroProducto> lista = null;
            if (filtro == "")
            {
                lista = (from t in db.tbProducto
                         where t.estado == true
                         orderby t.producto
                         select new RegistroProducto
                         {
                             codProducto = t.codProducto,
                             nombre = t.producto,
                             categoria = t.tbCategoria.categoria,
                             presentacion = t.tbPresentacion.presentacion,
                             dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                            }).ToList();
            }
            else
            {
                lista = (from t in db.tbProducto
                            where t.producto.Contains(filtro) && t.estado == true//cambiado de & a && (no evalua la segunda expr si la 1era ea false)
                            orderby t.producto
                            select new RegistroProducto
                            {
                                codProducto = t.codProducto,
                                nombre = t.producto,
                                categoria = t.tbCategoria.categoria,
                                presentacion = t.tbPresentacion.presentacion,
                                dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml")
                            }).ToList();
            }
            return PartialView("_Productos", lista);
        }

        public ActionResult MostrarDetalle(IEnumerable<Item> detalle)
        {
            //este action recibe como parametro la lista de productos Item, tenes que verla en los modelos, esta dentro del modelo Entrada,
            //es un objeto con un int codProducto y un int cantidad, que son los dos campos del array en javascript
            //creo una lista
            List<RegistroProducto> lista = new List<RegistroProducto>();
            if (detalle != null)
            {
                foreach (Item item in detalle)
                {
                    //por cada item dentro de la lista detalle creo un registroProducto
                    lista.Add((from t in db.tbProducto
                               where t.codProducto == item.codProducto
                               orderby t.producto
                               select new RegistroProducto
                               {
                                   codProducto = t.codProducto,
                                   nombre = t.producto,
                                   categoria = t.tbCategoria.categoria,
                                   presentacion = t.tbPresentacion.presentacion,
                                   dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                                   cantidad = item.cantidad
                               }).SingleOrDefault());
                }
            }
            //el retorno sera el html creado en la vista parcial _detalle pasandole como modelo la lista
            return PartialView("_Detalle", lista);
        }

        // GET: Entrada/Detalles/5
        public ActionResult Detalles(int codEntrada)
        {
            List<RegistroProducto> lista = (from det in db.tbDetalleEntrada
                                            join pro in db.tbProducto on det.codProducto equals pro.codProducto
                                            where det.codEntrada == codEntrada
                                            select new RegistroProducto
                                            {
                                                codProducto = det.codProducto,
                                                nombre = pro.producto,
                                                categoria = pro.tbCategoria.categoria,
                                                presentacion = pro.tbPresentacion.presentacion,
                                                dosis = pro.dosis.ToString() + ((pro.codVolumen == 1) ? " mg" : " ml"),
                                                cantidad = det.cantidad.Value
                                            }).ToList();
            tbEntrada entrada = (from t in db.tbEntrada where t.codEntrada == codEntrada select t).SingleOrDefault();
            ViewBag.descripcion = entrada.descripcion;
            ViewBag.fechaEntrada = entrada.fechaEntrada.Value.ToLongDateString();
            return View(lista);
        }

        public JsonResult CrearProducto(tbProducto producto)
        {
            db.tbProducto.InsertOnSubmit(producto);
            db.SubmitChanges();
            return Json(producto.codProducto);
        }

        // GET: Entrada/ListaProductos/5
        public ActionResult ListaProductos(int codEntrada)
        {
            return View();
        }

        // POST: Entrada/ListaProductos/5
        [HttpPost]
        public ActionResult ListaProductos(int codEntrada, bool accion, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Entrada/AgregarProducto/5
        public ActionResult AgregarProducto(int codEntrada)
        {
            return View(); 
        }

        // GET: Entrada/NuevaCategoria
        public JsonResult NuevaCategoria(tbCategoria categoria)
        {
            db.tbCategoria.InsertOnSubmit(categoria);
            db.SubmitChanges();
            return Json(categoria);
        }

        // GET: Entrada/NuevaPresentacion
        public JsonResult NuevaPresentacion(tbPresentacion presentacion)
        {
            db.tbPresentacion.InsertOnSubmit(presentacion);
            db.SubmitChanges();
            return Json(presentacion);
        }

        // GET: Entrada/NuevoProducto/5
        public ActionResult NuevoProducto(int codEntrada)
        {
            List<Volumen> edades = new List<Volumen> { (new Volumen { codVolumen = 1, volumen = "Niños" }),
                                             (new Volumen { codVolumen = 2, volumen = "Adultos" }) };
            ViewBag.codPresentacion = new SelectList(db.tbPresentacion, "codPresentacion", "presentacion");
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria");
            ViewBag.codVolumen = new SelectList(edades, "codVolumen", "volumen");
            ViewBag.codEntrada = codEntrada;
            return View();
        }

        // POST: Entrada/NuevoProducto/5
        [HttpPost]
        public ActionResult NuevoProducto(int codEntrada, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbProducto nuevo = new tbProducto
                {
                    producto = collection["producto"],
                    codCategoria = int.Parse(collection["codCategoria"]),
                    codPresentacion = int.Parse(collection["codPresentacion"]),
                    codVolumen = int.Parse(collection["codVolumen"]),
                    estado = true
                };
                db.tbProducto.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                int codProducto = (from t in db.tbProducto orderby t.codProducto descending select t.codProducto).First();
                return RedirectToAction("Cantidad", new { codEntrada = codEntrada, codProducto = codProducto });
            }
            catch
            {
                return View();
            }
        }

       
    }
}
