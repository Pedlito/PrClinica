using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using PagedList;

namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
    [PermisoAttribute(Permiso = RolesPermisos.consultar_existencia)]
    public class InventarioController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
    
       
        public ActionResult Index(int? page)
        {
            List<int> lista = (from t in db.tbDetalleEntrada group t by t.codProducto into g select g.Key).ToList();

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
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (inventario == null) { return View(); }
            return View(inventario.ToPagedList(pageNumber, pageSize));
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
     
    }
}
