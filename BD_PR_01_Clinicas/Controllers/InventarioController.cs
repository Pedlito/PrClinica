﻿using System;
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
                                               dosis = RegistroProducto.Dosis(prod.dosis.ToString(), prod.codVolumen.Value, prod.dosis2.ToString(), prod.codVolumen2.Value),
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
                                               //cambiar por un campo dosis por que no deja concatenar
                                               presentacion = pres.presentacion,
                                               dosis = RegistroProducto.Dosis(prod.dosis.ToString(), prod.codVolumen.Value, prod.dosis2.ToString(), prod.codVolumen2.Value),
                                               existencia = db.existencias(prod.codProducto).Value
                                           }).ToList();
            int id = SessionUsuario.Get.UserId;
            var us = db.tbUsuario.Where(x => x.codUsuario == id).SingleOrDefault();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "Productos.rpt"));
            rd.SetDataSource(inventario);
            Response.Buffer = false;
            Response.Clear();
            Response.ClearHeaders();

            rd.SetParameterValue("name", us.nombre);

            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "aplication/pdf", "Inventario.pdf");
        }
     
    }
}
