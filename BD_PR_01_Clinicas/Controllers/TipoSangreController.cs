using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class TipoSangreController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: TipoSangre
        public ActionResult Index()
        {
            List<tbTipoSangre> lista = (from t in db.tbTipoSangre orderby t.tipoSangre select t).ToList();
            return View(lista);
        }

        // GET: TipoSangre/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: TipoSangre/Create
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                //TODO: Add insert logic here
                tbTipoSangre nuevo = new tbTipoSangre
                {
                    tipoSangre = collection["tipoSangre"],
                    estado = true
                };
                db.tbTipoSangre.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TipoSangre/Editar/5
        public ActionResult Editar(int codTipoSangre)
        {
            tbTipoSangre editar = (from t in db.tbTipoSangre where t.codTipoSangre == codTipoSangre select t).SingleOrDefault();
            return View(editar);
        }

        // POST: TipoSangre/Editar/5
        [HttpPost]
        public ActionResult Editar(int codTipoSangre, FormCollection collection)
        {
            try
            {
                //TODO: Add update logic here
                tbTipoSangre editar = (from t in db.tbTipoSangre where t.codTipoSangre == codTipoSangre select t).SingleOrDefault();
                editar.tipoSangre = collection["tipoSangre"];
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TipoSangre/Delete/5
        public ActionResult CambiarEstado(int codTipoSangre, bool estado)
        {
            tbTipoSangre cambio = (from t in db.tbTipoSangre where t.codTipoSangre == codTipoSangre select t).SingleOrDefault();
            return View(cambio);
        }

        // POST: TipoSangre/Delete/5
        [HttpPost]
        public ActionResult CambiarEstado(int codTipoSangre, bool estado, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbTipoSangre cambio = (from t in db.tbTipoSangre where t.codTipoSangre == codTipoSangre select t).SingleOrDefault();
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