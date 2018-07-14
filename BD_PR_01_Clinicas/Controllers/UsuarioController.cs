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
  //  [PermisoAttribute(Permiso = RolesPermisos.administrar_usuarios)]
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
               
                lista = (from t in db.tbUsuario orderby t.nombre select t).ToList();

            }
            else
            {

                lista = (from t in db.tbUsuario where t.nombre.Contains(usuario) orderby t.nombre select t).Take(20).ToList();
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
            List<tbRol> rols = db.tbRol.Where(x=>x.estado==true).ToList();
            List<SelectListItem> roles = new SelectList(rols, "codTipoUsuario", "Rol").ToList();
            
            ViewBag.codTipoUsuario = roles;
            return View();
        }
        [HttpPost]
        public ActionResult CrearUsuario(FormCollection collection)
        {
            try
            {  
                    tbUsuario nuevo = new tbUsuario
                    {
                        codTipoUsuario = int.Parse(collection["codTipoUsuario"]),
                        nombre = collection["nombre"],
                        dpi = collection["dpi"],
                        carnet = collection["carnet"],
                        fechaNacimiento = DateTime.Parse(collection["fechaNacimiento"]),
                        usuario = collection["usuario"],
                        password = collection["password"],
                        estado = true
                        
                    };
                db.tbUsuario.InsertOnSubmit(nuevo);
                db.SubmitChanges();
                return RedirectToAction("Index");
  
            }
            catch
            {
                    ModelState.AddModelError("", "Hubo un error al crear el registro");
                    return View();
            }


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
                List<tbRol> rols = db.tbRol.Where(s=>s.codTipoUsuario!=usuario.codTipoUsuario).ToList();        
                List<SelectListItem> roles = new SelectList(rols, "codTipoUsuario", "Rol").ToList();          
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
                usr.usuario = colecction["usuario"];
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
