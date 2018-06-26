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

        #region Crear
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
        #endregion

        //Envia lista de pacientes
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


        // GET: Atencion/NuevoPaciente
        public JsonResult NuevoPaciente(tbPaciente paciente)
        {
            db.tbPaciente.InsertOnSubmit(paciente);
            db.SubmitChanges();
            return Json(new { codPaciente = paciente.codPaciente, nombre = paciente.nombre });
        }

        #region Editar
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
        #endregion

        #region Eliminar
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
        #endregion

        #region HC
        // GET: Atencion/CrearHC
        public ActionResult CrearHC(int codPaciente)
        {
            List<tbTipoSangre> codTipoSangre = (from t in db.tbTipoSangre where t.estado == true select t).ToList();
            ViewBag.codTipoSangre = new SelectList(codTipoSangre, "codTipoSangre", "tipoSangre");
            List<tbUsuario> medicos = (from t in db.tbUsuario where t.codTipoUsuario == 1 && t.estado == true orderby t.nombre select t).ToList();
            ViewBag.codUsuario = new SelectList(medicos, "codUsuario", "nombre");
            tbConsulta consulta = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.atendido == false select t).SingleOrDefault();
            return View(consulta);
        }

        // POST: Atencion/GuardarHC
        public string GuardarHC(HistoriaClinica historia)
        {
            try
            {
                // TODO: Add delete logic here
                tbPaciente paciente = (from t in db.tbPaciente where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
                tbConsulta consulta = (from t in db.tbConsulta where t.codConsulta == historia.codConsulta select t).SingleOrDefault();
                consulta.atendido = true;
                paciente.nombre = historia.paciente.nombre;
                paciente.genero = historia.paciente.genero;
                paciente.fechaNacimiento = historia.paciente.fechaNacimiento;
                paciente.estadoCivil = historia.paciente.estadoCivil;
                paciente.residencia = historia.paciente.residencia;
                paciente.procedencia = historia.paciente.procedencia;
                paciente.religion = historia.paciente.religion;
                paciente.profesion = historia.paciente.profesion;
                paciente.razaEtnia = historia.paciente.razaEtnia;
                paciente.escolaridad = historia.paciente.escolaridad;
                paciente.codTipoSangre = historia.paciente.codTipoSangre;
                db.tbAntecedentesPatologicos.InsertOnSubmit(historia.patologicos);
                db.tbAntecedentesNoPatologicos.InsertOnSubmit(historia.noPatologicos);
                db.tbDesarrollo.InsertOnSubmit(historia.desarrollo);
                db.tbPerfilSocial.InsertOnSubmit(historia.perfilSocial);
                if (paciente.genero == false)
                {
                    db.tbMujeres.InsertOnSubmit(historia.mujeres);
                }
                db.tbRevisionSistemas.InsertOnSubmit(historia.revision);
                db.tbPlanes.InsertOnSubmit(historia.planes);
                db.tbDiagnostico.InsertOnSubmit(historia.diagnostico);
                db.tbPlanTerapeutico.InsertOnSubmit(historia.terapeutico);
                db.tbProblema.InsertAllOnSubmit(historia.problemas);
                db.tbReceta.InsertAllOnSubmit(historia.receta);
                db.tbSignosVitales.InsertOnSubmit(historia.signos);
                db.SubmitChanges();
                return Url.Action("Index", "Atencion");
            }
            catch (Exception e)
            {
                return Url.Action("CrearHC", "Atencion");
            }
        }
        #endregion

        public ActionResult Filtrar(string filtro = "")
        {
            List<RegistroProducto> lista = null;
            if (filtro == "")
            {
                lista = (from t in db.tbProducto
                         where t.estado == true
                         orderby t.producto
                         select new RegistroProducto
                         {
                             codProducto = t.codProducto,
                             nombre = t.producto,
                             categoria = t.tbCategoria.categoria,
                             presentacion = t.tbPresentacion.presentacion,
                             dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                         }).Take(10).ToList();
            }
            else
            {
                lista = (from t in db.tbProducto
                         where t.producto.Contains(filtro) && t.estado == true//cambiado de & a && (no evalua la segunda expr si la 1era ea false)
                         orderby t.producto
                         select new RegistroProducto
                         {
                             codProducto = t.codProducto,
                             nombre = t.producto,
                             categoria = t.tbCategoria.categoria,
                             presentacion = t.tbPresentacion.presentacion,
                             dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml")
                         }).Take(10).ToList();
            }
            return PartialView("_Medicamentos", lista);
        }

        public ActionResult DetalleReceta(IEnumerable<tbReceta> detalle)
        {
            List<RegistroProducto> lista = new List<RegistroProducto>();
            if (detalle != null)
            {
                foreach (tbReceta item in detalle)
                {
                    //por cada item dentro de la lista detalle creo un registroProducto
                    lista.Add((from t in db.tbProducto
                               where t.codProducto == item.codProducto
                               orderby t.producto
                               select new RegistroProducto
                               {
                                   codProducto = t.codProducto,
                                   nombre = t.producto,
                                   categoria = t.tbCategoria.categoria,
                                   presentacion = t.tbPresentacion.presentacion,
                                   dosis = t.dosis.ToString() + ((t.codVolumen == 1) ? " mg" : " ml"),
                                   descripcion = item.descripcion
                               }).SingleOrDefault());
                }
            }
            //el retorno sera el html creado en la vista parcial _detalle pasandole como modelo la lista
            return PartialView("_Detalle", lista);
        }

        public ActionResult Problemas(List<tbProblema> problemas)
        {
            return PartialView("_Problemas", problemas);
        }
    }
}
