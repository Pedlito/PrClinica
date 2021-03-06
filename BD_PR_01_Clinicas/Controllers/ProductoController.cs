﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using PagedList;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
    [PermisoAttribute(Permiso = RolesPermisos.administrar_catalogo_prod)]
    public class ProductoController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        List<Volumen> volumenes = new List<Volumen> { (new Volumen { codVolumen = 1, volumen = "mg" }),
                                             (new Volumen { codVolumen = 2, volumen = "ml" }) };
        // GET: Producto      
        public ActionResult Index(int? accionActual, string filtro, string filtroActual, int? accion, int? page)
        {
            List<tbProducto> lista = null;

            if (filtro != null)
            {
                page = 1;
              
            }
            else
            {
                filtro = filtroActual;
                accion = accionActual;
            }
            ViewBag.filtroActual = filtro;

            if (accion == null) { accion = 1; };

            ViewBag.accionActual = accion;



            if (!String.IsNullOrEmpty(filtro))
            {
                if (accion == 1)
                    lista = (from t in db.tbProducto where t.producto.Contains(filtro) orderby t.estado descending, t.producto select t).ToList(); 
                else if (accion == 2)
                    lista = (from t in db.tbProducto where t.tbCategoria.categoria.Contains(filtro) orderby t.estado descending, t.producto select t).ToList();
                else if (accion == 3)
                    lista = (from t in db.tbProducto where t.tbPresentacion.presentacion.Contains(filtro) orderby t.estado descending, t.producto select t).ToList();

            }
            else 
            {
                lista = (from t in db.tbProducto orderby t.estado descending, t.producto select t).ToList();

            }
         

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        // GET: Producto/Crear
   
        public ActionResult Crear()
        {
            List<tbPresentacion> presentaciones = (from t in db.tbPresentacion where t.estado == true orderby t.presentacion select t).ToList();
            List<tbCategoria> categorias = (from t in db.tbCategoria where t.estado == true orderby t.categoria select t).ToList();
            ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
            ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
            ViewBag.codVolumen = new SelectList(volumenes, "codVolumen", "volumen");
            ViewBag.codVolumen2 = new SelectList(volumenes, "codVolumen", "volumen");
            return View();
        }

        // GET: Producto/nuevaCategoria
        public JsonResult nuevaCategoria(tbCategoria categoria)
        {
            categoria.estado = true;
            db.tbCategoria.InsertOnSubmit(categoria);
            db.SubmitChanges();
            return Json(categoria);
        }

        // GET: Producto/NuevaPresentacion
        public JsonResult NuevaPresentacion(tbPresentacion presentacion)
        {
            presentacion.estado = true;
            db.tbPresentacion.InsertOnSubmit(presentacion);
            db.SubmitChanges();
            return Json(presentacion);
        }

        // POST: Producto/Crear
        [HttpPost]

        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                int paraDosis2 = 0;
                if (collection["dosis2"] != "")
                {
                    paraDosis2 = int.Parse(collection["dosis2"]);
                }
                tbProducto nuevo = new tbProducto
                {
                    producto = collection["producto"],
                    codCategoria = int.Parse(collection["codCategoria"]),
                    codPresentacion = int.Parse(collection["codPresentacion"]),
                    dosis = int.Parse(collection["dosis"]),
                    codVolumen = int.Parse(collection["codVolumen"]),
                    dosis2 = paraDosis2,
                    codVolumen2 = int.Parse(collection["codVolumen2"]),
                    estado = true
                };
                db.tbProducto.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                List<tbPresentacion> presentaciones = (from t in db.tbPresentacion where t.estado == true orderby t.presentacion select t).ToList();
                List<tbCategoria> categorias = (from t in db.tbCategoria where t.estado == true orderby t.categoria select t).ToList();
                ViewBag.codPresentacion = new SelectList(presentaciones, "codPresentacion", "presentacion");
                ViewBag.codCategoria = new SelectList(categorias, "codCategoria", "categoria");
                ViewBag.codVolumen = new SelectList(volumenes, "codVolumen", "volumen");
                ViewBag.codVolumen2 = new SelectList(volumenes, "codVolumen", "volumen");
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
            ViewBag.codVolumen2 = new SelectList(volumenes, "codVolumen", "volumen");
            return View(producto);
        }

        // POST: Producto/Editar/5
        [HttpPost]

        public ActionResult Editar(int codProducto, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                int paraDosis2 = 0;
                if (collection["dosis2"] != "")
                {
                    paraDosis2 = int.Parse(collection["dosis2"]);
                }
                tbProducto producto = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
                producto.producto = collection["producto"];
                producto.codCategoria = int.Parse(collection["codCategoria"]);
                producto.codPresentacion = int.Parse(collection["codPresentacion"]);
                producto.dosis = int.Parse(collection["dosis"]);
                producto.codVolumen = int.Parse(collection["codVolumen"]);
                producto.dosis2 = paraDosis2;
                producto.codVolumen2 = int.Parse(collection["codVolumen2"]);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Producto/CambiarEstado/5
        public ActionResult CambiarEstado(int codProducto, bool estado)
        {
            tbProducto cambio = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
            return View(cambio);
        }

        // POST: Producto/CambiarEstado/5
        [HttpPost]
    
        public ActionResult CambiarEstado(int codProducto, bool estado, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbProducto cambio = (from t in db.tbProducto where t.codProducto == codProducto select t).SingleOrDefault();
                cambio.estado = estado;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult ReporteDeCatalogo()
        {
            List<Inventario> lista = (from t in db.tbProducto orderby t.producto, t.producto select new Inventario {
                producto = t.producto,
                categoria = t.tbCategoria.categoria,
                presentacion = t.tbPresentacion.presentacion + " " + RegistroProducto.Dosis(t.dosis.ToString(), t.codVolumen.Value, t.dosis2.ToString(), t.codVolumen2.Value)
            }).ToList();

            int id = SessionUsuario.Get.UserId;
            var us = db.tbUsuario.Where(x => x.codUsuario==id).SingleOrDefault();
            ReportDocument rd = new ReportDocument();
            rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "catalogo.rpt"));
            rd.SetDataSource(lista);
            Response.Buffer = false;
            Response.Clear();
            Response.ClearHeaders();
            rd.SetParameterValue("name", us.nombre);
     
            Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "aplication/pdf", "Catalogo_de_productos.pdf");
        }

        public int NuevoProducto(tbProducto producto)
        {
            if (producto.dosis2 == null)
            {
                producto.dosis2 = 0;
            }
            producto.estado = true; 
            db.tbProducto.InsertOnSubmit(producto);
            db.SubmitChanges();
            return producto.codProducto;
        }
    }
}
