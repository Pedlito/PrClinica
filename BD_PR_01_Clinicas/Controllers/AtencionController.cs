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
    public class AtencionController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Atencion
        public ActionResult Index(int? page)
        {
            List<tbPaciente> lista = (from cons in db.tbConsulta
                                      join pac in db.tbPaciente on cons.codPaciente equals pac.codPaciente
                                      where cons.atendido == false
                                      orderby cons.fechaLlegada
                                      select pac).ToList();
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        // GET: Atencion/Details/5
        public ActionResult Cancelar(int codPaciente)
        {
            tbConsulta consulta = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.atendido == false select t).SingleOrDefault();
            db.tbConsulta.DeleteOnSubmit(consulta);
            db.SubmitChanges();
            return RedirectToAction("Index");
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
        //public string Crear(int codPaciente, string motivoConsulta, string HistoriaEnfermedad)
        public string Crear(tbConsulta consulta)
        {
            try
            {
                // TODO: Add insert logic here
                consulta.fechaLlegada = DateTime.Now;
                consulta.atendido = false;
                db.tbConsulta.InsertOnSubmit(consulta);
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
                         select t).Take(15).ToList();
            }
            else
            {
                lista = (from t in db.tbPaciente
                         where t.nombre.Contains(filtro)
                         orderby t.codPaciente
                         select t).Take(15).ToList();
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
            List<tbTipoSangre> paciente_codTipoSangre = (from t in db.tbTipoSangre where t.estado == true select t).ToList();
            ViewBag.paciente_codTipoSangre = new SelectList(paciente_codTipoSangre, "codTipoSangre", "tipoSangre");
            List<tbUsuario> medicos = (from t in db.tbUsuario where t.codTipoUsuario == 1 && t.estado == true orderby t.nombre select t).ToList();
            ViewBag.diagnostico_codUsuario = new SelectList(medicos, "codUsuario", "nombre");
            tbConsulta consulta = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.atendido == false select t).SingleOrDefault();
            HistoriaClinica historia = new HistoriaClinica();
            historia.consulta = consulta;
            historia.paciente = consulta.tbPaciente;
            historia.patologicos = consulta.tbPaciente.tbAntecedentesPatologicos;
            historia.noPatologicos = consulta.tbPaciente.tbAntecedentesNoPatologicos;
            historia.desarrollo = consulta.tbPaciente.tbDesarrollo;
            historia.perfilSocial = consulta.tbPaciente.tbPerfilSocial;
            if (consulta.tbPaciente.genero == false)
            {
                historia.mujeres = consulta.tbPaciente.tbMujeres;
            }

            return View(historia);
        }

        // POST: Atencion/GuardarHC
        public string GuardarHC(HistoriaClinica historia)
        {
            
            //actualizacion de datos para paciente
            tbPaciente paciente = (from t in db.tbPaciente where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
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

            //actualizacion de datos para consulta
            tbConsulta consulta = (from t in db.tbConsulta where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            consulta.motivoConsulta = historia.consulta.motivoConsulta;
            consulta.HistoriaEnfermedad = historia.consulta.HistoriaEnfermedad;

            //creacion o actualizacion de antecedentes patologicos
            tbAntecedentesPatologicos patologicos = (from t in db.tbAntecedentesPatologicos where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
            if (patologicos == null)
            {
                db.tbAntecedentesPatologicos.InsertOnSubmit(historia.patologicos);
            }
            else
            {
                patologicos.padre = historia.patologicos.padre;
                patologicos.madre = historia.patologicos.madre;
                patologicos.medicos = historia.patologicos.medicos;
                patologicos.quirurgicos = historia.patologicos.quirurgicos;
                patologicos.traumaticos = historia.patologicos.traumaticos;
                patologicos.alergicos = historia.patologicos.alergicos;
                patologicos.ginecoObstetricos = historia.patologicos.ginecoObstetricos;
                patologicos.viciosManias = historia.patologicos.viciosManias;
            }

            //creacion o actualizacion de antecedentes no patologicos
            tbAntecedentesNoPatologicos noPatologicos = (from t in db.tbAntecedentesNoPatologicos where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
            if (noPatologicos == null)
            {
                db.tbAntecedentesNoPatologicos.InsertOnSubmit(historia.noPatologicos);
            }
            else
            {
                noPatologicos.prenatal = historia.noPatologicos.prenatal;
                noPatologicos.natal = historia.noPatologicos.natal;
                noPatologicos.posnatal = historia.noPatologicos.posnatal;
                noPatologicos.inmunizaciones = historia.noPatologicos.inmunizaciones;
                noPatologicos.alimentacion = historia.noPatologicos.alimentacion;
                noPatologicos.habitos = historia.noPatologicos.habitos;
            }

            //creacion o actualizacion de desarrollo
            tbDesarrollo desarrollo = (from t in db.tbDesarrollo where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
            if (desarrollo == null)
            {
                if (historia.desarrollo.uno != null && historia.desarrollo.dos != null && historia.desarrollo.tres != null && historia.desarrollo.cuatro != null && historia.desarrollo.cinco != null &&
                    historia.desarrollo.seis != null && historia.desarrollo.siete != null && historia.desarrollo.ocho != null && historia.desarrollo.nueve != null && historia.desarrollo.diez != null)
                {
                    db.tbDesarrollo.InsertOnSubmit(historia.desarrollo);
                }
            }
            else
            {
                desarrollo.uno = historia.desarrollo.uno;
                desarrollo.dos = historia.desarrollo.dos;
                desarrollo.tres = historia.desarrollo.tres;
                desarrollo.cuatro = historia.desarrollo.cuatro;
                desarrollo.cinco= historia.desarrollo.cinco;
                desarrollo.seis = historia.desarrollo.seis;
                desarrollo.siete = historia.desarrollo.siete;
                desarrollo.ocho = historia.desarrollo.ocho;
                desarrollo.nueve = historia.desarrollo.nueve;
                desarrollo.diez = historia.desarrollo.diez;
                desarrollo.oncee = historia.desarrollo.oncee;
                desarrollo.doce = historia.desarrollo.doce;
            }

            //creacion o actualizacion del perfil social
            tbPerfilSocial perfil = (from t in db.tbPerfilSocial where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
            if (perfil == null)
            {
                db.tbPerfilSocial.InsertOnSubmit(historia.perfilSocial);
            }
            else
            {
                perfil.estiloVida = historia.perfilSocial.estiloVida;
                perfil.vivienda = historia.perfilSocial.vivienda;
                perfil.situacionFamiliar = historia.perfilSocial.situacionFamiliar;
                perfil.ingresoEconomico = historia.perfilSocial.ingresoEconomico;
                perfil.animales = historia.perfilSocial.animales;
                perfil.tendenciaSexual = historia.perfilSocial.tendenciaSexual;
                perfil.puntoVista = historia.perfilSocial.puntoVista;
            }

            //creacion o actualizacion de mujeres
            if (paciente.genero == false)
            {
                tbMujeres mujeres = (from t in db.tbMujeres where t.codPaciente == historia.paciente.codPaciente select t).SingleOrDefault();
                if (mujeres == null)
                {
                    db.tbMujeres.InsertOnSubmit(historia.mujeres);
                }
                else
                {
                    mujeres.menarquia = historia.mujeres.menarquia;
                    mujeres.ritmo = historia.mujeres.ritmo;
                    mujeres.ultimaRegla = historia.mujeres.ultimaRegla;
                    mujeres.numGestas = historia.mujeres.numGestas;
                    mujeres.partos = historia.mujeres.partos;
                    mujeres.cesareas = historia.mujeres.cesareas;
                    mujeres.abortos = historia.mujeres.abortos;
                    mujeres.hijosVivos = historia.mujeres.hijosVivos;
                    mujeres.hijosMuertos = historia.mujeres.hijosMuertos;
                    mujeres.metodoPlanificacion = historia.mujeres.metodoPlanificacion;
                }
            }
            db.tbRevisionSistemas.InsertOnSubmit(historia.revision);
            db.tbPlanes.InsertOnSubmit(historia.planes);

            historia.diagnostico.fecha = DateTime.Now;
            db.tbDiagnostico.InsertOnSubmit(historia.diagnostico);
            db.tbPlanTerapeutico.InsertOnSubmit(historia.terapeutico);
            if (historia.problemas != null)
            {
                db.tbProblema.InsertAllOnSubmit(historia.problemas);
            }
            if (historia.receta != null)
            {
                db.tbReceta.InsertAllOnSubmit(historia.receta);
            }
            db.tbSignosVitales.InsertOnSubmit(historia.signos);
            consulta.atendido = true;
            db.SubmitChanges();
            return Url.Action("Index", "Atencion");
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
