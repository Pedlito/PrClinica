﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
using PagedList;
namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
    [PermisoAttribute(Permiso = RolesPermisos.administrar_rotaciones)]
    public class RotacionController : Controller
    {
       
        DataClasesDataContext db = new DataClasesDataContext();

        // GET: Rotacion
        public ActionResult Index(int? page)
        {
            List<tbRotacion> lista = (from t in db.tbRotacion where t.estado == true select t).ToList();

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult RevisarFechasEditar(Rotaciones rot) {
            var rotaciones = db.tbRotacion.Where(x=>x.codRotacion!=rot.rotacionId);
            foreach (var r in rotaciones){
                if ((DateTime.Parse(rot.fechaIni) >= r.fechaInicio) && (DateTime.Parse(rot.fechaIni) <= r.fechaFinal)||
                   (DateTime.Parse(rot.fechaFin) >= r.fechaInicio) && (DateTime.Parse(rot.fechaFin) <= r.fechaFinal))
                   { return Json("aaaaa"); }
            }
            return Json("a");
        }
        public JsonResult RevisarFechasCrear(Rotaciones rot)
        {
            var rotaciones = db.tbRotacion.ToList();
            var ultimaRotacion = rotaciones.Select(x => x.fechaFinal).Max();
            var primeraRotacion = rotaciones.Select(x => x.fechaInicio).Min();
            foreach (var r in rotaciones)
            {
                if ((DateTime.Parse(rot.fechaIni) >= r.fechaInicio) && (DateTime.Parse(rot.fechaIni) <= r.fechaFinal) ||
                   (DateTime.Parse(rot.fechaFin) >= r.fechaInicio) && (DateTime.Parse(rot.fechaFin) <= r.fechaFinal) ||
                   (DateTime.Parse(rot.fechaIni) < primeraRotacion) && (DateTime.Parse(rot.fechaFin) > ultimaRotacion))
                { return Json("aaaaa"); }
            }
            return Json("a");
        }
        public ActionResult Crear()
        {
            try
            {    //se obtiene fecha inicio y fin de la ultima rotacion;
            var ultimaRotacion = db.tbRotacion.Select(x=>x.fechaFinal).Max();
           // var primeraRotacion = db.tbRotacion.Select(x => x.fechaInicio).Min();
            var rotacionU = db.tbRotacion.Where(x => x.fechaFinal == ultimaRotacion).SingleOrDefault();
            //var rotacionP = db.tbRotacion.Where(x => x.fechaInicio == primeraRotacion).SingleOrDefault();

            if (rotacionU!=null)
            {

                ViewBag.fechaFinUR = rotacionU.fechaFinal.Value.AddDays(1).ToString("dd/MM/yyyy");//limite superior
                   // ViewBag.fechaInPR = rotacionP.fechaInicio.Value.ToString("dd/MM/yyyy"); //limite inferior
            }
            else
            {
                ViewBag.fechaFinUR = DateTime.Now.AddYears(-1).ToString("dd/mm/yyyy");
              //  ViewBag.fechaInPR = DateTime.Now.AddYears(-1).ToString("dd/mm/yyyy");
            }
            //se envia la info a a vista

            //lista de usuarios
            List<tbUsuario> doctores = (from u in db.tbUsuario where u.estado == true && u.codTipoUsuario == 3 select u).ToList();
                List<tbUsuario> estudiantes = (from u in db.tbUsuario where u.estado == true && u.codTipoUsuario == 2 select u).ToList();
                //para los selects
                ViewBag.codDoctor = new SelectList(doctores, "codUsuario", "nombre");
                ViewBag.codEstudiante = new SelectList(estudiantes, "codUsuario", "nombre");

                return View();
        }
            catch (Exception){
                ViewBag.errores += "Se genero un error en la conexion con la base de datos";
            }

            return View("VistaDeErrores");
}


        [HttpPost]
        public JsonResult Crear(Rotaciones rotacion) {
            String resultado = "";
            try
            {
                tbRotacion nuevaRotacion = new tbRotacion()
                {
                    fechaInicio = DateTime.Parse(rotacion.fechaIni),
                    fechaFinal = DateTime.Parse(rotacion.fechaFin),
                    estado = true
                };

                    foreach (var d in rotacion.integrantes)
                    {
                        nuevaRotacion.tbRotacionUsuario.Add(new tbRotacionUsuario
                        {
                            codUsuario = d.codUser,
                            estado = true
                        });
                    }  
              
                db.tbRotacion.InsertOnSubmit(nuevaRotacion);

            }
            catch (Exception )
            {

                resultado ="Ha ocurrido un error en la conexion con la base de datos";
            }

            try { db.SubmitChanges(); } catch (Exception) { resultado += " ERROR: No se ha podido guradar el registro"; }

            return Json(resultado);
        }
        // GET: Rotacion/Integrantes/5
        public ActionResult Integrantes(int? id)
        {
                tbRotacion rotacion = (from t in db.tbRotacion where t.codRotacion == id select t).SingleOrDefault();

            if (rotacion == null)
            {
                @ViewBag.errores = "La accion anterior no esta permitida";
                return View("VistaDeErrores");
            }
          
             
          

            ViewBag.fechaInicio = rotacion.fechaInicio.Value.ToString("dd/MM/yyyy");
            ViewBag.fechaFinal = rotacion.fechaFinal.Value.ToString("dd/MM/yyyy");
  
            return View(rotacion.tbRotacionUsuario.ToList());
        }

        public void ReporteDeRotacion(int? codR) {

            //if (codR != null) {
            //    tbRotacion rotacion = (from t in db.tbRotacion where t.codRotacion == codR select t).SingleOrDefault();

            //}
           

            //ReportDocument rd = new ReportDocument();
            //rd.Load(Path.Combine(Server.MapPath("~/Reportes"), "Productos.rpt"));
            //rd.SetDataSource(inventario);
            //Response.Buffer = false;
            //Response.Clear();
            //Response.ClearHeaders();

            //Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            //stream.Seek(0, SeekOrigin.Begin);
            //return File(stream, "aplication/pdf", "Inventario.pdf");
            
        }
        public ActionResult AgregarDocs(IEnumerable<ItemUser> listUser)
        {
            List<tbUsuario> lista = new List<tbUsuario>();
            
            if (listUser != null)
            {

                foreach (ItemUser item in listUser)
                {
                    //por cada item dentro de la lista detalle creo un registroProducto
                    tbUsuario nuevoUs =new  tbUsuario();
                    var tbu = (from t in db.tbUsuario where t.codUsuario == item.codUser
                               orderby t.codUsuario select t).SingleOrDefault();

                        nuevoUs.codUsuario = tbu.codUsuario;
                        nuevoUs.nombre = tbu.nombre;
                        nuevoUs.dpi = tbu.dpi;
                        nuevoUs.usuario = tbu.usuario;

                    lista.Add(nuevoUs);
                }
            }

            return PartialView("_Doctores", lista);
        }

        public ActionResult AgregarEstus(IEnumerable<ItemUser> listUser)
        {
            List<tbUsuario> lista = new List<tbUsuario>();
         
            if (listUser != null)
            {

                foreach (ItemUser item in listUser)
                {
                    
                    tbUsuario nuevoUs = new tbUsuario();
                    var tbu = (from t in db.tbUsuario where t.codUsuario == item.codUser
                               orderby t.codUsuario select t).SingleOrDefault();
                    nuevoUs.codUsuario = tbu.codUsuario;
                    nuevoUs.nombre = tbu.nombre;
                    nuevoUs.carnet = tbu.carnet;
                    nuevoUs.usuario = tbu.usuario;

                    lista.Add(nuevoUs);
                }
            }

            return PartialView("_Estudiantes", lista);
        }


        // GET: Rotacion/Editar/5, permite editar una rotacion
        public ActionResult Editar(int? id)
        {


            tbRotacion rotacion = (from t in db.tbRotacion where t.codRotacion == id select t).SingleOrDefault();
            if (rotacion != null)
            {   //consulta para validar las fechas inicio y fin
                var ultimaRotacion = db.tbRotacion.Select(x => x.fechaFinal).Max();
               // var primeraRotacion = db.tbRotacion.Select(x => x.fechaInicio).Min();
                var rotacionU = db.tbRotacion.Where(x => x.fechaFinal == ultimaRotacion).SingleOrDefault();
              //  var rotacionP = db.tbRotacion.Where(x => x.fechaInicio == primeraRotacion).SingleOrDefault();
                ViewBag.fechaFinUR = rotacionU.fechaFinal.Value.ToString("dd/MM/yyyy");//limite superior
               // ViewBag.fechaInPR = rotacionP.fechaInicio.Value.ToString("yyyy-MM-dd");//limite inferior
                //muestra las fechas en grande
                ViewBag.fechaInicio = rotacion.fechaInicio.Value.ToString("dd/MM/yyyy");
                ViewBag.fechaFinal = rotacion.fechaFinal.Value.ToString("dd/MM/yyyy");
                //para editarlo
                ViewBag.fechaIni = rotacion.fechaInicio.Value.ToString("dd/MM/yyyy"); ;
                ViewBag.fechaFin = rotacion.fechaFinal.Value.ToString("dd/MM/yyyy"); ;
                //id de la rotacion actual
                ViewBag.id = rotacion.codRotacion;
                List<tbUsuario> listaIntegrantes = (from u in db.tbUsuario where u.estado == true  select u).ToList();
                foreach (var integrante in rotacion.tbRotacionUsuario.Select(x=>x.tbUsuario))
                {
                    listaIntegrantes.Remove(integrante);
                }
                //estudiantes y doctores por separado, si se agregan nuevos se obtiene la lista competa, en la vista se validan repetidos.
                List<tbUsuario> doctores = (from u in listaIntegrantes where u.codTipoUsuario == 3 select u).ToList();
                List<tbUsuario> estudiantes = (from u in listaIntegrantes where u.codTipoUsuario == 2 select u).ToList();

                //combox seleccionar usuarios
                ViewBag.codDoctor = new SelectList(doctores, "codUsuario", "nombre");
                ViewBag.codEstudiante = new SelectList(estudiantes, "codUsuario", "nombre");
                //lista de usuarios de la rotacion
                return View(rotacion.tbRotacionUsuario.ToList());
            }
            else
            {
                @ViewBag.errores = "Operacion invalida";
                return View("VistaDeErrores");
            }
        }


        //POST: Rotacion/Edit/5
        [HttpPost]
        public JsonResult Editar(Rotaciones rotacion)
        {
            String resultado="";

            try
            {
                tbRotacion RotAmod = (from R in db.tbRotacion where R.codRotacion == rotacion.rotacionId select R).SingleOrDefault();
                List<tbRotacionUsuario> origin = (from det in RotAmod.tbRotacionUsuario select det).ToList();

                if (RotAmod.fechaInicio != DateTime.Parse(rotacion.fechaIni)) { RotAmod.fechaInicio = DateTime.Parse(rotacion.fechaIni); }
                if (RotAmod.fechaFinal != DateTime.Parse(rotacion.fechaFin)) { RotAmod.fechaFinal = DateTime.Parse(rotacion.fechaFin); }

                //romover de la base los removidos en la  vista
           
                    foreach (var d in origin)
                    {
                        if (!rotacion.integrantes.Any(x => x.codUser == d.codUsuario))
                        {
                            tbRotacionUsuario tbru = (from r in db.tbRotacionUsuario where r.codUsuario == d.codUsuario && r.codRotacion == rotacion.rotacionId select r).SingleOrDefault();
                            db.tbRotacionUsuario.DeleteOnSubmit(tbru);

                        }
                    }
                    //agregar en la base los agragados en la vista.
                    foreach (var d in rotacion.integrantes)
                    {
                        if (!RotAmod.tbRotacionUsuario.Any(x => x.codUsuario == d.codUser))
                        {
                            tbRotacionUsuario rn = new tbRotacionUsuario
                            {
                                codRotacion = rotacion.rotacionId,
                                codUsuario = d.codUser,
                                estado = true
                            };
                            db.tbRotacionUsuario.InsertOnSubmit(rn);
                        }
                    } 
              
            }
            catch (Exception e)
            {

                resultado = e.Message;
            }


            if (resultado=="") { db.SubmitChanges(); }
           

            return Json(resultado);
        }

        // GET: Rotacion/Delete/5
        public JsonResult Archivar(int? id)
        {
            string resultado = "";
            if (id != null)
            {
                try
                {
                    tbRotacion Rot = (from r in db.tbRotacion where (r.codRotacion == id) select r).SingleOrDefault();
                    Rot.estado = false;

                    foreach (var item in Rot.tbRotacionUsuario.ToList())
                    {
                        tbUsuario us = (from u in db.tbUsuario where u.codUsuario == item.codUsuario select u).SingleOrDefault();
                        if (us.codTipoUsuario == 2) { us.estado = false; }

                    }
                    db.SubmitChanges();
                }
                catch (Exception)
                {

                    resultado = "Error: Operacion fallida";
                }
            }
            else
            {
                resultado = "No se pudo realizar la accion";
            }
          

            return Json(resultado);
        }
        public ActionResult Archivados(int? anios) {
            List<tbRotacion> lista = db.tbRotacion.OrderByDescending(x=>x.fechaInicio).Where(x=>x.estado==false).Take(10).ToList();
            var an = db.tbRotacion.Select(x => x.fechaInicio.Value.Year).Distinct();
            ViewBag.anios = new SelectList(an);

            if (anios != null)
            {
                lista = lista.Where(x => x.fechaInicio.Value.Year == anios).ToList();
            }

            return View(lista);
        }
        public ActionResult Restablecer(int? id)
        {

            try
            {
                tbRotacion Rot = (from r in db.tbRotacion where (r.codRotacion == id) select r).SingleOrDefault();
                Rot.estado = true;

                foreach (var item in Rot.tbRotacionUsuario.ToList())
                {
                    tbUsuario us = (from u in db.tbUsuario where u.codUsuario == item.codUsuario select u).SingleOrDefault();
                    if (us.codTipoUsuario == 2) { us.estado = true; }

                }
                db.SubmitChanges();
            }
            catch
            {
                ViewBag.errores = "No se ha podido reestablecer la rotacion";
                return View("VistaDeErrores");
            }


            return RedirectToAction("Index");
        }
   
        [HttpPost]
        public JsonResult ObtenerDocs(int? id) {

   
            List<ItemUser> codsDocs = (from R in db.tbRotacionUsuario
                                       where R.codRotacion == id && R.tbUsuario.codTipoUsuario == 3 select 
                                       new ItemUser{ codUser = R.codUsuario }).ToList();

            return Json(codsDocs.ToArray()); 
        }
        public JsonResult ObtenerEstus(int? id) {
            List<ItemUser> codsEstus = (from R in db.tbRotacionUsuario
                                        where R.codRotacion == id && R.tbUsuario.codTipoUsuario == 2
                                        select new ItemUser{codUser = R.codUsuario }).ToList();
            return Json(codsEstus.ToArray());
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
