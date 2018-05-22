using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class UsuarioController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Usuario
        public ActionResult Index()
        {
            List<tbUsuario> lista = (from t in db.tbUsuario select t).ToList();
            return View(lista);
        }

        // GET: Usuario/Details/5
        public ActionResult Detalles(int codUsuario)
        {
            tbUsuario item = (from t in db.tbUsuario
                              where t.codUsuario == codUsuario
                              select t).SingleOrDefault();
            return View(item);
        }

        // GET: Usuario/CrearDoctor
        public ActionResult CrearDoctor()
        {
            return View();
        }

        // POST: Usuario/CrearDoctor
        [HttpPost]
        public ActionResult CrearDoctor(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (collection["password"] == collection["repetir"])
                {
                    tbUsuario nuevo = new tbUsuario
                    {
                        nombre = collection["nombre"],
                        dpi = collection["dpi"],
                        fechaNacimiento = DateTime.Parse(collection["fechaNacimiento"]),
                        usuario = collection["password"],
                        password = collection["password"],
                        codTipoUsuario = 1
                    };
                    db.tbUsuario.InsertOnSubmit(nuevo);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Las Contraseñas no coinciden.");
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/CrearEstudiante
        public ActionResult CrearEstudiante()
        {
            return View();
        }

        // POST: Usuario/CrearEstudiante
        [HttpPost]
        public ActionResult CrearEstudiante(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (collection["password"] == collection["repetir"])
                {
                    tbUsuario nuevo = new tbUsuario
                    {
                        nombre = collection["nombre"],
                        dpi = collection["dpi"],
                        carnet = collection["carnet"],
                        fechaNacimiento = DateTime.Parse(collection["fechaNacimiento"]),
                        usuario = collection["password"],
                        password = collection["password"],
                        codTipoUsuario = 2
                    };
                    db.tbUsuario.InsertOnSubmit(nuevo);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Las Contraseñas no coinciden.");
                    return View();
                }
                
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Usuario/Edit/5
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

        // GET: Usuario/Deshabilitar/5
        public ActionResult Deshabilitar(int codUsuario)
        {
            tbUsuario deshabilitar = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
            return View(deshabilitar);
        }

        // POST: Usuario/Deshabilitar/5
        [HttpPost]
        public ActionResult Deshabilitar(int codUsuario, FormCollection collection)
        {
            try
            {
                // TODO: Add Deshabilitar logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/Habilitar/5
        public ActionResult Habilitar(int id)
        {
            return View();
        }

        // POST: Usuario/Habilitar/5
        [HttpPost]
        public ActionResult Habilitar(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add Habilitar logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Usuario/Delete/5
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
