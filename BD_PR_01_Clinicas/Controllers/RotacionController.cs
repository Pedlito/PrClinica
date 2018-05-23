using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    [Authorize]
    public class RotacionController : Controller
    {
       
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Rotacion
        public ActionResult Index()
        {
            List<tbRotacion> lista = (from t in db.tbRotacion select t).ToList();
            return View(lista);
        }

        // GET: Rotacion/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Rotacion/Crear
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
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

        // GET: Rotacion/Integrantes/5
        public ActionResult Integrantes(int id)
        {
            List<tbRotacionUsuario> lista = (from t in db.tbRotacionUsuario where t.codRotacion == id  && t.estado == true select t).ToList();
            tbRotacion rotacion = (from t in db.tbRotacion where t.codRotacion == id select t).SingleOrDefault();
            ViewBag.idRotacion = id;
            ViewBag.fechaInicio = rotacion.fechaInicio.Value.ToShortDateString();
            ViewBag.fechaFinal = rotacion.fechaFinal.Value.ToShortDateString();
            return View(lista);
        }

        // GET: Rotacion/AgregarIntegrante
        public ActionResult AgregarIntegrante(int idRotacion)
        {
            ViewBag.codUsuario = new SelectList(db.tbUsuario, "codUsuario", "nombre");
            ViewBag.idRotacion = idRotacion;
            return View();
        }

        // POST: Rotacion/AgregarIntegrante
        [HttpPost]
        public ActionResult AgregarIntegrante(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                tbRotacionUsuario nuevo = new tbRotacionUsuario
                {
                    codRotacion = int.Parse(collection["codRotacion"]),
                    codUsuario = int.Parse(collection["codUsuario"]),
                    estado = true
                };
                db.tbRotacionUsuario.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                return RedirectToAction("Integrantes");
            }
            catch
            {
                return View();
            }
        }

        // GET: Rotacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Rotacion/Edit/5
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

        // GET: Rotacion/Delete/5
        public ActionResult Delete(int idRotacion, int idUsuario)
        {
            return View();
        }

        // POST: Rotacion/Delete/5
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
        [AllowAnonymous]
        public ActionResult ActivarDesacivarRegistro()
        {
            tbConfiguracion tc = (from v in db.tbConfiguracion where v.codConfiguracion == 1 select v).SingleOrDefault();
            return View(tc);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ActivarDesacivarRegistro(tbConfiguracion confi)
        {
            tbConfiguracion config = null;
            try
            {
                // TODO: Add update logic here
                config = (from t in db.tbConfiguracion where t.codConfiguracion == confi.codConfiguracion select t).SingleOrDefault();
                config.valor = confi.valor;
                db.SubmitChanges();
                return RedirectToAction("Administracion", "Home");
            }
            catch
            {
                ModelState.AddModelError("Error: al modificar ", config.valor.ToString());
                return View();
            }
        }
        [AllowAnonymous]
        public ActionResult MensajeDeRegistro()
        {

            return View();
        }
    }
}
