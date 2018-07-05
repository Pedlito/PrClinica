using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace BD_PR_01_Clinicas.Controllers
{
    public class InventarioController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Inventario
        public ActionResult Index()
        {
            List<int> lista = (from t in db.tbDetalleEntrada group t by t.codProducto into g select g.Key).ToList();
            //List<Inventario> inventario = (from cat in db.tbCategoria
            //                               join vista in db.vInventario on cat.codCategoria equals vista.codCategoria
            //                               join pres in db.tbPresentacion on vista.codPresentacion equals pres.codPresentacion
            //                               where db.existencias(vista.codProducto) > 0
            //                               orderby vista.producto
            //                               select new Inventario
            //                               {
            //                                   codProducto = vista.codProducto,
            //                                   producto = vista.producto,
            //                                   categoria = cat.categoria,
            //                                   presentacion = pres.presentacion,
            //                                   codVolumen = vista.codVolumen.Value,
            //                                   existencia = db.existencias(vista.codProducto).Value
            //                               }).ToList();
            List<Inventario> inventario = (from cat in db.tbCategoria join prod in db.tbProducto on cat.codCategoria equals prod.codCategoria
                                           join pres in db.tbPresentacion on prod.codPresentacion equals pres.codPresentacion
                                           where lista.Contains(prod.codProducto) && db.existencias(prod.codProducto) > 0
                                           orderby prod.producto
                                           select new Inventario
                                           {
                                               codProducto = prod.codProducto,
                                               producto = prod.producto,
                                               categoria = cat.categoria,
                                               presentacion = pres.presentacion,
                                               dosis = prod.dosis.Value,
                                               codVolumen = prod.codVolumen.Value,
                                               existencia = db.existencias(prod.codProducto).Value
                                           }).ToList();
            return View(inventario);
        }

        public ActionResult ReporteDeInventario() {
            List<int> lista = (from t in db.tbDetalleEntrada group t by t.codProducto into g select g.Key).ToList();
            List<Inventario> inventario = (from cat in db.tbCategoria
                                           join prod in db.tbProducto on cat.codCategoria equals prod.codCategoria
                                           join pres in db.tbPresentacion on prod.codPresentacion equals pres.codPresentacion
                                           where lista.Contains(prod.codProducto) && db.existencias(prod.codProducto) > 0
                                           orderby prod.producto
                                           select new Inventario
                                           {
                                               producto = prod.producto,
                                               categoria = cat.categoria,
                                               presentacion = pres.presentacion + " " +
                                               Convert.ToString(prod.dosis.Value) + " " + (prod.codVolumen.Value == 1 ? "mg" : "ml"),
                                               existencia = db.existencias(prod.codProducto).Value
                                           }).ToList();

            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "Productos.rpt"));
            rd.SetDataSource(inventario);
            Response.Buffer = false;
            Response.Clear();
            Response.ClearHeaders();

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "aplication/pdf", "Inventario.pdf");
        }
        //solo para hacer pruebas
       //public List<Inventario> obtenerDatos()
       // {
       //     List<Inventario> inv = new List<Inventario>();
       //     for (int i = 1; i <= 100; i++)
       //     {
       //         inv.Add(new Inventario { producto = "jkjkjkjkjkjf", categoria = "fdfdfdffdfffffffff", presentacion = "fffffffffff", existencia = 2125 });
       //     }
       //     return inv;
       // }

        // GET: Inventario/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Inventario/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Inventario/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Inventario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Inventario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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
    }
}
