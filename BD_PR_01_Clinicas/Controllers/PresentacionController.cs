using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{ 
    [Authorize]
    public class PresentacionController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Presentacion
        public ActionResult Index()
        {
            List<tbPresentacion> lista = (from t in db.tbPresentacion orderby t.estado descending, t.presentacion select t).ToList();
            return View(lista);
        }

        // GET: Presentacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Presentacion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbPresentacion nueva = new tbPresentacion
                {
                    presentacion = collection["presentacion"],
                    estado = true
                };
                db.tbPresentacion.InsertOnSubmit(nueva);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Presentacion/Edit/5
        public ActionResult Edit(int id)
        {
            tbPresentacion editar = (from t in db.tbPresentacion where t.codPresentacion == id select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Presentacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                tbPresentacion editar = (from t in db.tbPresentacion where t.codPresentacion == id select t).SingleOrDefault();
                editar.presentacion = collection["presentacion"];
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Presentacion/CambiarEstado/5
        public ActionResult CambiarEstado(int codPresentacion, bool estado)
        {
            tbPresentacion cambio = (from t in db.tbPresentacion where t.codPresentacion == codPresentacion select t).SingleOrDefault();
            return View(cambio);
        }

        // POST: Presentacion/CambiarEstado/5
        [HttpPost]
        public ActionResult CambiarEstado(int codPresentacion, bool estado, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbPresentacion cambio = (from t in db.tbPresentacion where t.codPresentacion == codPresentacion select t).SingleOrDefault();
                cambio.estado = estado;
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
