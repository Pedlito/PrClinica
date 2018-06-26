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
        static tbRotacion rot = new tbRotacion();
       static List<tbUsuario> miembros = new List<tbUsuario>();
      
        public ActionResult Crear()
        {
            // List<tbUsuario> usuariosValidos = (from u in db.tbUsuario where u.estado == true select u).ToList();
            if (rot.fechaInicio!=null) {
                ViewBag.fechaInicio = rot.fechaInicio.Value.ToString("yyyy-MM-dd");
                ViewBag.fechaFinal = rot.fechaFinal.Value.ToString("yyyy-MM-dd");
            }
            List<tbUsuario> doctores = (from u in db.tbUsuario where u.codTipoUsuario == 1 select u).ToList();
            List<tbUsuario> estudiantes = (from u in db.tbUsuario where u.codTipoUsuario==2 select u).ToList();

            ViewBag.codDoctor = new SelectList(doctores, "codUsuario", "nombre");
            ViewBag.codEstudiante = new SelectList(estudiantes, "codUsuario", "nombre");
            return View(miembros);
        }

        // POST: Rotacion/Crear
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {

            if (collection["fechaInicio"] !="")
            {
                rot.fechaInicio = DateTime.Parse(collection["fechaInicio"]);
                rot.fechaFinal = DateTime.Parse(collection["fechaFinal"]);
            }
      
            string str = Request.Params["btn"];

            if (str == "AgregarD")
            {

                tbUsuario usuar = (from u in db.tbUsuario where u.codUsuario == int.Parse(collection["codDoctor"]) select u).SingleOrDefault();
   
                if (!miembros.Any(x => x.codUsuario == usuar.codUsuario)) { miembros.Add(usuar); } else { /*mesaje de repetidos */}

                return RedirectToAction("Crear");
            }
            if (str == "AgregarE")
            {

                tbUsuario usuar = (from u in db.tbUsuario where u.codUsuario == int.Parse(collection["codEstudiante"]) select u).SingleOrDefault();

                if (!miembros.Any(x => x.codUsuario == usuar.codUsuario)) { miembros.Add(usuar); } else { /*mesaje de repetidos */}

                return RedirectToAction("Crear");
            }
            if (str == "Guardar")
            {
                try
                {
                    tbRotacion rotacion = new tbRotacion()
                    {
                        fechaInicio = DateTime.Parse(collection["fechaInicio"]),
                        fechaFinal = DateTime.Parse(collection["fechaFinal"])
                    };

                    foreach (var d in miembros) {
                        rotacion.tbRotacionUsuario.Add(new tbRotacionUsuario{
                          codUsuario = d.codUsuario,
                          estado = true
                        });
                    }
                    db.tbRotacion.InsertOnSubmit(rotacion);
                    db.SubmitChanges();
                    miembros.Clear();
                    rot.fechaInicio = null; rot.fechaFinal = null;
                    return RedirectToAction("Index");
                }
                catch
                {
                    return RedirectToAction("Index");
                }

            }
            if (str == "Cancelar")
            {
                miembros.Clear();
                rot.fechaInicio = null; rot.fechaFinal = null;
                return RedirectToAction("Index");

            }
            return RedirectToAction("Index");

        }

        // GET: Rotacion/Integrantes/5
        public ActionResult Integrantes(int? id , int? accion, string error="")
        {
            if (accion == 1) { ViewBag.mostrar = true; } else { ViewBag.mostrar = false; }
            if (id != null)
            {
                List<tbRotacionUsuario> lista = (from t in db.tbRotacionUsuario where t.codRotacion == id && t.estado == true select t).ToList();
                tbRotacion rotacion = (from t in db.tbRotacion where t.codRotacion == id select t).SingleOrDefault();
                ViewBag.idRotacion = id;
                ViewBag.fechaInicio = rotacion.fechaInicio.Value.ToString("dd/MM/yyyy");
                ViewBag.fechaFinal = rotacion.fechaFinal.Value.ToString("dd/MM/yyyy");
                //lista de usuarios en la vista.
                //List<tbUsuario> usuariosValidos = (from u in db.tbUsuario where u.estado == true select u).ToList();
                ViewBag.codUsuario = new SelectList(db.tbUsuario, "codUsuario", "nombre");
                if (error!="") { ModelState.AddModelError("mensaje", "Ya ha sido agregado"); }
                return View(lista);
            }
            else {

                return HttpNotFound();
            }
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

            tbRotacionUsuario tbru = (from r in db.tbRotacionUsuario where r.codUsuario == int.Parse(collection["codUsuario"]) && r.codRotacion == int.Parse(collection["codRotacion"]) select r).SingleOrDefault();
            if (tbru != null){ return RedirectToAction("Integrantes", "Rotacion", new { id = int.Parse(collection["codRotacion"]), accion = 1, error ="Repeticion"}); }
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
                    //int? id = nuevo.codRotacion;

                    return RedirectToAction("Integrantes", "Rotacion", new { id = nuevo.codRotacion, accion = 1 });
                    //return RedirectToAction("Index", "Home", new { id = 2 });
                    //return RedirectToAction("/Rotacion/Integrantes/");
                }
                catch
                {
                    return View();
                }
          
        }

        // GET: Rotacion/Edit/5
        public ActionResult Descartar(int? id)
        {
         var us =   miembros.Find(m => m.codUsuario == id);
            miembros.Remove(us);
            return RedirectToAction("Crear");
        }

        // POST: Rotacion/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        // GET: Rotacion/Delete/5
        public ActionResult Delete(int? idRotacion, int? idUsuario)
        {
            //ModeloRotacionUsuario usuario = (from rot in db.tbRotacionUsuario
            //                       join  us in db.tbUsuario on rot.codUsuario equals us.codUsuario where rot.codRotacion == idRotacion
            //                       select new ModeloRotacionUsuario { idRotacion = rot.codRotacion, idUsuario=us.codUsuario, nombreUs = us.nombre}).SingleOrDefault();
     
            tbRotacionUsuario Rot = (from r in db.tbRotacionUsuario where (r.codUsuario == idUsuario && r.codRotacion == idRotacion) select r).SingleOrDefault();
            ViewBag.usuario =(from t in db.tbUsuario where t.codUsuario == idUsuario select t.nombre).SingleOrDefault();
    
            return View(Rot);
        }

        // POST: Rotacion/Delete/5
        [HttpPost]
        public ActionResult Delete(FormCollection col)
        {
   

            tbRotacionUsuario tbru = (from r in db.tbRotacionUsuario where r.codUsuario == int.Parse(col["codUsuario"]) && r.codRotacion == int.Parse(col["codRotacion"]) select r).SingleOrDefault();
            try
            {
            
         
                db.tbRotacionUsuario.DeleteOnSubmit(tbru);
                db.SubmitChanges();
                return RedirectToAction("Integrantes", "Rotacion", new { id = tbru.codRotacion});
            }
            catch(Exception)
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
                if (config == null)
                {
 
                    config = new tbConfiguracion();
                    config.codConfiguracion = 1;
                    config.descripcion = "Permitir registros al usuario";
                    config.valor = confi.valor;
                    db.tbConfiguracion.InsertOnSubmit(config);
                    db.SubmitChanges();
                    
                }
                else
                {
                    config.valor = confi.valor;
                    db.SubmitChanges();
                }

                return RedirectToAction("Administracion", "Home");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error: ", ex.Message);//aqui esta el error
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
