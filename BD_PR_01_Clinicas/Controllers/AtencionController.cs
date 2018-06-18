using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class AtencionController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Atencion
        public ActionResult Index()
        {
            List<tbPaciente> lista = (from cons in db.tbConsulta
                                      join pac in db.tbPaciente on cons.codPaciente equals pac.codPaciente
                                      where cons.atendido == false
                                      orderby cons.fechaLlegada
                                      select pac).ToList();
            return View(lista);
        }

        // GET: Atencion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Atencion/Crear
        public ActionResult Crear()
        {
            List<tbTipoSangre> tiposSangre = (from t in db.tbTipoSangre where t.estado == true select t).ToList();
            ViewBag.codTipoSangre = new SelectList(tiposSangre, "codTipoSangre", "tipoSangre");
            return View();
        }

        // POST: Atencion/Crear
        [HttpPost]
        public string Crear(int codPaciente, string motivoConsulta, string HistoriaEnfermedad)
        {
            try
            {
                // TODO: Add insert logic here
                tbConsulta nueva = new tbConsulta
                {
                    codPaciente = codPaciente,
                    motivoConsulta = motivoConsulta,
                    HistoriaEnfermedad = HistoriaEnfermedad,
                    fechaLlegada = DateTime.Now,
                    atendido = false
                };
                db.tbConsulta.InsertOnSubmit(nueva);
                db.SubmitChanges();
                return Url.Action("Index", "Atencion");
            }
            catch
            {
                return Url.Action("Crear", "Atencion");
            }
        }

        public ActionResult Pacientes(string filtro = "")
        {
            //este es el codigo que se ejecuta cuando se habre el modal apachando el boton buscar, tiene un filtro como cualquier otro
            //pero al final, dice return PartialView("_Productos", lista), osea que llama a la vista parcial _Productos y le pasa la lista
            //como el modelo
            List<tbPaciente> lista = null;
            if (filtro == "")
            {
                lista = (from t in db.tbPaciente
                         orderby t.nombre
                         select t).ToList();
            }
            else
            {
                lista = (from t in db.tbPaciente
                         where t.nombre.Contains(filtro)
                         orderby t.codPaciente
                         select t).ToList();
            }
            return PartialView("_Pacientes", lista);
        }


        // GET: Atencion/Edit/5
        public JsonResult NuevoPaciente(tbPaciente paciente)
        {
            db.tbPaciente.InsertOnSubmit(paciente);
            db.SubmitChanges();
            return Json(new { codPaciente = paciente.codPaciente, nombre = paciente.nombre });
        }

        // GET: Atencion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Atencion/Edit/5
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

        // GET: Atencion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Atencion/Delete/5
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
