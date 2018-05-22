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
        // GET: Entrada
        public ActionResult Index()
        {
            List<tbEntrada> lista = (from t in db.tbEntrada orderby t.fechaEntrada select t).ToList();
            return View(lista);
        }

        // GET: Entrada/Detalles/5
        public ActionResult Detalles(int codEntrada)
        {
            List<tbDetalleEntrada> lista = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada select t).ToList();
            return View(lista);
        }

        // GET: Entrada/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Entrada/Crear
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbEntrada nueva = new tbEntrada
                {
                    descripcion = collection["descripcion"],
                    fechaEntrada = DateTime.Now
                };
                db.tbEntrada.InsertOnSubmit(nueva);
                db.SubmitChanges();
                int codEntrada = (from t in db.tbEntrada orderby t.codEntrada descending select t.codEntrada).First();
                return RedirectToAction("ListaProductos", new { codEntrada = codEntrada });
            }
            catch
            {
                return View();
            }
        }

        // GET: Entrada/ListaProductos/5
        public ActionResult ListaProductos(int codEntrada)
        {
            List<tbDetalleEntrada> lista = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada select t).ToList();
            ViewBag.codEntrada = codEntrada;
            return View(lista);
        }

        // POST: Entrada/ListaProductos/5
        [HttpPost]
        public ActionResult ListaProductos(int codEntrada, bool accion, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                if (!accion)
                {
                    // TODO: Add cancel logic here
                    tbEntrada eliminar = (from t in db.tbEntrada where t.codEntrada == codEntrada select t).SingleOrDefault();
                    db.tbEntrada.DeleteOnSubmit(eliminar);
                    db.SubmitChanges();
                }
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
            List<tbProducto> productos = (from t in db.tbProducto where t.estado == true orderby t.producto select t).ToList();
            ViewBag.codEntrada = codEntrada;
            return View(productos);
        }

        // GET: Entrada/cantidad/5
        public ActionResult cantidad(int codEntrada, int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            ViewBag.codProducto = codProducto;
            ViewBag.codEntrada = codEntrada;
            ViewBag.producto = producto.producto;
            ViewBag.categoria = producto.tbCategoria.categoria;
            ViewBag.presentacion = producto.tbPresentacion.presentacion;
            ViewBag.codVolumen = producto.codVolumen;
            return View();
        }

        // POST: Entrada/cantidad/5
        [HttpPost]
        public ActionResult cantidad(int codEntrada, int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbDetalleEntrada existente = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada && t.codProducto == codProducto select t).SingleOrDefault();
                if (existente == null)
                {
                    tbDetalleEntrada nueva = new tbDetalleEntrada
                    {
                        codEntrada = codEntrada,
                        codProducto = codProducto,
                        cantidad = int.Parse(collection["cantidad"])
                    };
                    db.tbDetalleEntrada.InsertOnSubmit(nueva);
                }
                else
                {
                    existente.cantidad += int.Parse(collection["cantidad"]);
                }
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codEntrada = codEntrada });
            }
            catch (Exception e)
            {
                return View();
            }
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

        // GET: Entrada/QuitarProducto/5
        public ActionResult QuitarProducto(int codEntrada, int codProducto)
        {
            tbDetalleEntrada quitar = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada && t.codProducto == codProducto select t).SingleOrDefault();
            db.tbDetalleEntrada.DeleteOnSubmit(quitar);
            db.SubmitChanges();
            return RedirectToAction("ListaProductos", new { codEntrada = codEntrada });
        }

        // GET: Entrada/EditarCantidad/5
        public ActionResult EditarCantidad(int codEntrada, int codProducto)
        {
            tbDetalleEntrada editar = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada && t.codProducto == codProducto select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Entrada/EditarCantidad/5
        [HttpPost]
        public ActionResult EditarCantidad(int codEntrada, int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbDetalleEntrada editar = (from t in db.tbDetalleEntrada where t.codEntrada == codEntrada && t.codProducto == codProducto select t).SingleOrDefault();
                editar.cantidad = int.Parse(collection["cantidad"]);
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codEntrada = codEntrada });
            }
            catch
            {
                return View();
            }
        }
    }
}
