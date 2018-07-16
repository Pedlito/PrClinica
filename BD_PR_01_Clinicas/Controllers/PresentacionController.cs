using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using PagedList;
namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
    [PermisoAttribute(Permiso = RolesPermisos.administrar_presentaciones)]
    public class PresentacionController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Presentacion
        [AutenticadoAttribute]
        public ActionResult Index(int? page, string presentacion, string filtroActual )
        {
            List<tbPresentacion> lista = null;

            if (presentacion != null)
            {
                page = 1;

            }
            else
            {
                presentacion = filtroActual;
            }

            ViewBag.filtroActual = presentacion;

            if (presentacion == null)
            {
                lista = (from t in db.tbPresentacion orderby t.estado descending, t.presentacion select t).ToList();
            }
            else
            {
                lista = (from t in db.tbPresentacion where t.presentacion.Contains(presentacion) orderby t.estado descending, t.presentacion select t).ToList();
            }

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }
     
        // GET: Presentacion/Crear
        public ActionResult Crear()
        {
            
            return View();
        }

        // POST: Presentacion/Crear
        [HttpPost]
        public ActionResult Crear(tbPresentacion pre)
        {
            try
            {
                if (db.tbPresentacion.Where(x => x.presentacion == pre.presentacion).Any()) { ModelState.AddModelError("presentacion","La presentacion ya existe"); return View(pre); }
                tbPresentacion nueva = new tbPresentacion
                {
                    presentacion = pre.presentacion,
                    estado = true
                };
                db.tbPresentacion.InsertOnSubmit(nueva);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.errores = "No se pudo completar la operacion";
                return View("VistaDeErrores");
            }

        }

        // GET: Presentacion/Editar/5
        public ActionResult Editar(int codPresentacion)
        {
            tbPresentacion editar = (from t in db.tbPresentacion where t.codPresentacion == codPresentacion select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Presentacion/Editar/5
        [HttpPost]
        public ActionResult Editar(tbPresentacion pre)
        {
            try
            {
                //TODO: Add update logic here
                tbPresentacion editar = (from t in db.tbPresentacion where t.codPresentacion == pre.codPresentacion select t).SingleOrDefault();
                if (db.tbPresentacion.Where(x => x.presentacion == pre.presentacion).Any()) { ModelState.AddModelError("presentacion", "La presentacion ya existe"); return View(pre); }
                editar.presentacion = pre.presentacion;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.errores = "No se pudo completar la operacion";
                return View("VistaDeErrores");
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
