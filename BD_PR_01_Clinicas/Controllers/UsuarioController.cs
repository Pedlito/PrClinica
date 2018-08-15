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
    [PermisoAttribute(Permiso = RolesPermisos.administrar_usuarios)]
    public class UsuarioController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Usuario
        public ActionResult Index(int? page, string usuario, string filtroActual)
        {
            List<tbUsuario> lista = null;

            if (usuario != null)
            {
                page = 1;

            }
            else
            {
                usuario = filtroActual;
            }

            ViewBag.filtroActual = usuario;

            if (usuario == null)
            {
               
                lista = (from t in db.tbUsuario orderby t.codTipoUsuario select t).ToList();

            }
            else
            {

                lista = (from t in db.tbUsuario where t.nombre.Contains(usuario) orderby t.codTipoUsuario select t).Take(30).ToList();
            }
          
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
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
        public ActionResult CrearUsuario()
        {
            List<tbRol> rols = db.tbRol.Where(x=>x.estado==true && x.codTipoUsuario!=4).ToList();
            var us = new RegisterViewModel();
            us.roles = rols;
            us.FechaNacimiento = DateTime.Now.AddYears(-25);
            return View(us);
        }
        [HttpPost]
        public ActionResult CrearUsuario(RegisterViewModel model)
        {
            List<tbRol> rols = null;
            try
            {
                if (!string.IsNullOrEmpty(model.Usuario) && db.tbUsuario.Where(m => m.usuario == model.Usuario.ToUpper()).Any()) { ModelState.AddModelError("Usuario", "El usuario ingresado ya existe."); }

                if (ModelState.IsValid)
                 {
                    tbUsuario NuevoUsuario = new tbUsuario
                    {

                    codTipoUsuario = model.selectedRol,
                    nombre = model.Nombre,
                    dpi = model.Dpi,
                    carnet = model.Carnet,
                    fechaNacimiento = model.FechaNacimiento,
                    usuario = model.Usuario.ToUpper(),
                    password = model.Password,          
                    estado = true

                };

                db.tbUsuario.InsertOnSubmit(NuevoUsuario);
                db.SubmitChanges();

                return RedirectToAction("Index", "Usuario");
            }
            
        }
            catch (Exception)
            {
                ViewBag.errores = "Error en la conexion con la base de datos";
                return View("VistaDeErrores");
    }

    rols = db.tbRol.Where(x => x.estado == true).ToList();
    model.roles = rols;
           
            return View(model);
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
        public ActionResult EditarUsuario(int? codUsuario)
        {
            
            if (codUsuario!=null) 
                {
                tbUsuario usuario = db.tbUsuario.Where(x=>x.codUsuario==codUsuario).SingleOrDefault();     
                  
                List<tbRol> rols = db.tbRol.Where(s=>s.estado==true && s.codTipoUsuario!=usuario.codTipoUsuario && s.codTipoUsuario!=4).ToList();        
                List<SelectListItem> roles = new SelectList(rols, "codTipoUsuario", "Rol").ToList();
              //  tbRol rl = db.tbRol.Where(x => x.codTipoUsuario == usuario.codTipoUsuario).SingleOrDefault();
       
                roles.Insert(0,(new SelectListItem {Text=usuario.tbRol.Rol,Value=usuario.tbRol.codTipoUsuario.ToString() }));
                //el viewBag que lleva los roles
                ViewBag.codTipoUsuario = roles;
                if (usuario != null) { return View(usuario); } 
            }
            ViewBag.errores = "No existe el usuario"; 
            return View("VistaDeErrores");
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        public ActionResult EditarUsuario(FormCollection colecction)
        {
            try
            {
                tbUsuario usr =  db.tbUsuario.Where(x => x.codUsuario == int.Parse(colecction["codUsuario"])).SingleOrDefault();
               
                usr.codTipoUsuario = int.Parse(colecction["codTipoUsuario"]);
                usr.nombre = colecction["nombre"];
                usr.carnet = colecction["carnet"];
                usr.dpi = colecction["dpi"];
                usr.fechaNacimiento = DateTime.Parse(colecction["fechaNacimiento"]);
                usr.password = colecction["password"];
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
        [HttpPost]
        public JsonResult UsuarioRepetido(string usuario)
        {
            var respuestaModel = new respuestaModelo
            {
                respuesta = true,
                 mensaje =""
            };

            try
            {
                  if (db.tbUsuario.Where(m => m.usuario == usuario.ToUpper()).Any())
                  {
                    respuestaModel.respuesta = false;
                    respuestaModel.mensaje = "El usuario ya existe";
                    return Json(respuestaModel);
                  }

                return Json(respuestaModel);
            }
            catch
            {
                respuestaModel.respuesta = false;
                respuestaModel.mensaje = "Error: conexion con servidor";
                return Json(respuestaModel);
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
