using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        List<Volumen> volumenes = new List<Volumen> { (new Volumen { codVolumen = 1, volumen = "mg" }),
                                             (new Volumen { codVolumen = 2, volumen = "ml" }) };
        // GET: Producto
        public ActionResult Index(string producto = "", string categoria = "", string presentacion = "")
        {
            if (true)
            {

            }
            List<tbProducto> lista = (from t in db.tbProducto orderby t.estado descending, t.producto select t).ToList();
            return View(lista);
        }

        // GET: Producto/Crear
        public ActionResult Crear()
        {
            ViewBag.codPresentacion = new SelectList(db.tbPresentacion, "codPresentacion", "presentacion");
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria");
            ViewBag.codVolumen = new SelectList(volumenes, "codVolumen", "volumen");
            return View();
        }

        // POST: Producto/Crear
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbProducto nuevo = new tbProducto
                {
                    producto = collection["producto"],
                    codCategoria = int.Parse(collection["codCategoria"]),
                    codPresentacion = int.Parse(collection["codPresentacion"]),
                    dosis = decimal.Parse(collection["dosis"]),
                    codVolumen = int.Parse(collection["codVolumen"]),
                    estado = true
                };
                db.tbProducto.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producto/Editar/5
        public ActionResult Editar(int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            ViewBag.codPresentacion = new SelectList(db.tbPresentacion, "codPresentacion", "presentacion");
            ViewBag.codCategoria = new SelectList(db.tbCategoria, "codCategoria", "categoria");
            ViewBag.codVolumen = new SelectList(volumenes, "codVolumen", "volumen");
            return View(producto);
        }

        // POST: Producto/Editar/5
        [HttpPost]
        public ActionResult Editar(int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
                producto.producto = collection["producto"];
                producto.codCategoria = int.Parse(collection["codCategoria"]);
                producto.codPresentacion = int.Parse(collection["codPresentacion"]);
                producto.dosis = decimal.Parse(collection["dosis"]);
                producto.codVolumen = int.Parse(collection["codVolumen"]);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producto/Delete/5
        public ActionResult Deshabilitar(int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost]
        public ActionResult Deshabilitar(int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
                producto.estado = false;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producto/Habilitar/5
        public ActionResult Habilitar(int codProducto)
        {
            tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost]
        public ActionResult Habilitar(int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
                producto.estado = true;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
