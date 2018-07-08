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
        public ActionResult EditarUsuario(int? id)
        {
            
            if (id!=null) 
                {
                tbUsuario usuario = db.tbUsuario.Where(x=>x.codUsuario==id).SingleOrDefault();
                tbRol t = new tbRol();
                
                  
                List<tbRol> rols = db.tbRol.Where(s=>s.codTipoUsuario!=usuario.codTipoUsuario).ToList();
                
                List<SelectListItem> roles = new SelectList(rols, "codTipoUsuario", "Rol").ToList();
                
                roles.Insert(0,(new SelectListItem {Text=usuario.tbRol.Rol,Value=usuario.tbRol.codTipoUsuario.ToString() }));
                ViewBag.codTipoUsuario = roles;
                if (usuario != null) { return View(usuario); } else { }
            }

            ViewBag.errores = "No existe el usuario"; 
            return View("VistaDeErrores");
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public ActionResult EditarUsuario(tbUsuario usuar)
        {
            try
            {
           tbUsuario usr =  db.tbUsuario.Where(x => x.codUsuario == usuar.codUsuario).SingleOrDefault();
                usr.nombre = usuar.nombre;
                usr.codTipoUsuario = usuar.codTipoUsuario;
                usr.carnet = usuar.carnet;
                usr.dpi = usuar.dpi;
                usr.fechaNacimiento = usuar.fechaNacimiento;
                usr.usuario = usuar.usuario;
                usr.password = usuar.password;
                db.SubmitChanges();   
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.errores = "No se pudo terminar la operacion";
                return View("VistaDeErrores");
            }
        }

        // GET: Usuario/Deshabilitar,Habilitar/5
        public ActionResult CambiarEstadoUsuario(int? codUsuario)
        {
            if (codUsuario != null)
            {
                tbUsuario cambiar = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
                if (cambiar!=null) { return View(cambiar); }
            }
          
                ViewBag.errores = "Usuario no encontrado";
                return View("VistaDeErrores");  
        }

        // POST: Usuario/Deshabilitar/5
        [HttpPost]
        public ActionResult CambiarEstadoUsuario(FormCollection collection)
        {
            try
            {

                tbUsuario usuario = (from user in db.tbUsuario where user.codUsuario == int.Parse(collection["codUsuario"]) select user).SingleOrDefault();
                string str = Request.Params["btn"];

                if (str == "deshabilitar")
                {
                    usuario.estado = false;
                }
                else
                {
                    usuario.estado = true;
                }
                         
                    db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                ViewBag.errores = "No se pudo completar la operacion";
                return View("VistaDeErrores");
            }
        }

        //// GET: Usuario/Habilitar/5
        //public ActionResult Habilitar(int codUsuario, bool estado)
        //{
        //    tbUsuario habilitar = (from t in db.tbUsuario where t.codUsuario == codUsuario select t).SingleOrDefault();
        //    return View(habilitar);
        //}

        //// POST: Usuario/Habilitar/5
        //[HttpPost]
        //public ActionResult Habilitar(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add Habilitar logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

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
