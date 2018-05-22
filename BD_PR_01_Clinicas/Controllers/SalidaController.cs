using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class SalidaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Salida
        public ActionResult Index()
        {
            List<tbSalida> lista = (from t in db.tbSalida orderby t.fechaSalida select t).ToList();
            return View(lista);
        }

        // GET: Salida/Detalles/5
        public ActionResult Detalles(int codSalida)
        {
            List<tbDetalleSalida> lista = (from t in db.tbDetalleSalida where t.codSalida == codSalida select t).ToList();
            return View(lista);
        }
        // GET: Salida/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Salida/Crear
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbSalida nueva = new tbSalida
                {
                    descripcion = collection["descripcion"],
                    fechaSalida = DateTime.Now
                };
                db.tbSalida.InsertOnSubmit(nueva);
                db.SubmitChanges();
                int codSalida = (from t in db.tbSalida orderby t.codSalida descending select t.codSalida).First();
                return RedirectToAction("ListaProductos", new { codSalida = codSalida });
            }
            catch
            {
                return View();
            }
        }

        // GET: Salida/ListaProductos/5
        public ActionResult ListaProductos(int codSalida)
        {
            List<tbDetalleSalida> lista = (from t in db.tbDetalleSalida where t.codSalida == codSalida select t).ToList();
            ViewBag.codSalida = codSalida;
            return View(lista);
        }

        // POST: Salida/ListaProductos/5
        [HttpPost]
        public ActionResult ListaProductos(int codSalida, bool accion, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                if (!accion)
                {
                    // TODO: Add cancel logic here
                    tbSalida eliminar = (from t in db.tbSalida where t.codSalida == codSalida select t).SingleOrDefault();
                    db.tbSalida.DeleteOnSubmit(eliminar);
                    db.SubmitChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Salida/AgregarProducto/5
        public ActionResult AgregarProducto(int codSalida)
        {
            List<tbProducto> productos = (from t in db.tbProducto where t.estado == true && db.existencias(t.codProducto) > 0 orderby t.producto select t).ToList();
            ViewBag.codSalida = codSalida;
            return View(productos);
        }

        // GET: Salida/cantidad/5
        public ActionResult cantidad(int codSalida, int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            ViewBag.codProducto = codProducto;
            ViewBag.codSalida = codSalida;
            ViewBag.producto = producto.producto;
            ViewBag.categoria = producto.tbCategoria.categoria;
            ViewBag.presentacion = producto.tbPresentacion.presentacion;
            ViewBag.codVolumen = producto.codVolumen;
            return View();
        }

        // POST: Salida/cantidad/5
        [HttpPost]
        public ActionResult cantidad(int codSalida, int codProducto, FormCollection collection)
        {
            try
            {
                tbDetalleSalida existente = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
                if (existente == null)
                {
                    tbDetalleSalida nueva = new tbDetalleSalida
                    {
                        codSalida = codSalida,
                        codProducto = codProducto,
                        cantidad = int.Parse(collection["cantidad"])
                    };
                    db.tbDetalleSalida.InsertOnSubmit(nueva);
                }
                else
                {
                    existente.cantidad += int.Parse(collection["cantidad"]);
                }
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codSalida = codSalida });
            }
            catch
            {
                return View();
            }
        }

        // GET: Salida/QuitarProducto/5
        public ActionResult QuitarProducto(int codSalida, int codProducto)
        {
            tbDetalleSalida quitar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
            db.tbDetalleSalida.DeleteOnSubmit(quitar);
            db.SubmitChanges();
            return RedirectToAction("ListaProductos", new { codSalida = codSalida });
        }

        // GET: Salida/EditarCantidad/5
        public ActionResult EditarCantidad(int codSalida, int codProducto)
        {
            tbDetalleSalida editar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Salida/EditarCantidad/5
        [HttpPost]
        public ActionResult EditarCantidad(int codSalida, int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbDetalleSalida editar = (from t in db.tbDetalleSalida where t.codSalida == codSalida && t.codProducto == codProducto select t).SingleOrDefault();
                editar.cantidad = int.Parse(collection["cantidad"]);
                db.SubmitChanges();
                return RedirectToAction("ListaProductos", new { codSalida = codSalida });
            }
            catch
            {
                return View();
            }
        }
    }
}
