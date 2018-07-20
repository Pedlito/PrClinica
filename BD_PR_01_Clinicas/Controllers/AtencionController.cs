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
    [PermisoAttribute(Permiso = RolesPermisos.administrar_consultas)]
    public class AtencionController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();

        // GET: Atencion
        public ActionResult Index(int? page)
        {
            tbUsuario usuario = (from t in db.tbUsuario where t.codUsuario == SessionUsuario.Get.UserId select t).SingleOrDefault();
            List<tbConsulta> lista = (from t in db.tbConsulta
                                      where t.estado == 1 || (t.estado == 2 && (t.codEstudiante == usuario.codUsuario || (usuario.codTipoUsuario == 4 || usuario.codTipoUsuario == 3)))
                                      select t).ToList();
            ViewBag.codTipoUsuario = usuario.codTipoUsuario;
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        // GET: Atencion/Details/5
        public ActionResult Cancelar(int codPaciente)
        {
            tbConsulta consulta = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.estado == 1 select t).SingleOrDefault();
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
        public string Crear(tbConsulta consulta)
        {
            try
            {   
                tbConsulta existe = (from t in db.tbConsulta where t.codPaciente == consulta.codPaciente && t.estado != 3 select t).SingleOrDefault();
                if (existe == null)
                {
                    consulta.fechaLlegada = DateTime.Now;
                    consulta.estado = 1;
                    db.tbConsulta.InsertOnSubmit(consulta);
                    db.SubmitChanges();
                    return Url.Action("Index", "Atencion");
                }
                else
                {
                    return "error";
                }
                
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
            //listas para dropdowns
            List<tbTipoSangre> paciente_codTipoSangre = (from t in db.tbTipoSangre where t.estado == true select t).ToList();
            ViewBag.paciente_codTipoSangre = new SelectList(paciente_codTipoSangre, "codTipoSangre", "tipoSangre");
            List<tbUsuario> medicos = (from t in db.tbUsuario where t.codTipoUsuario == 3 && t.estado == true orderby t.nombre select t).ToList();
            ViewBag.consulta_codMedico = new SelectList(medicos, "codUsuario", "nombre");

            //pide la consulta y le asigna los valores 
            tbConsulta consulta = (from t in db.tbConsulta where t.codPaciente == codPaciente && (t.estado == 1 || (t.estado == 2 && t.codEstudiante == SessionUsuario.Get.UserId)) select t).SingleOrDefault();
            consulta.estado = 2;
            consulta.codEstudiante = SessionUsuario.Get.UserId;
            consulta.fechaAtencion = DateTime.Now;
            db.SubmitChanges();
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
            historia.revision = consulta.tbRevisionSistemas;
            historia.signos = consulta.tbSignosVitales;
            historia.planes = consulta.tbPlanes;
            historia.terapeutico = consulta.tbPlanTerapeutico;
            historia.problemas = (from t in db.tbProblema where t.codConsulta == consulta.codConsulta select t).ToList();
            historia.receta = (from t in db.tbReceta where t.codConsulta == consulta.codConsulta select t).ToList();
            historia.diagnostico = consulta.tbDiagnostico;
            return View(historia);
        }

        // POST: Atencion/GuardarHC
        public string GuardarHC(HistoriaClinica historia)
        {
            #region paciente
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
            #endregion

            #region consulta
            //actualizacion de datos para consulta
            tbConsulta consulta = (from t in db.tbConsulta where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            consulta.motivoConsulta = historia.consulta.motivoConsulta;
            consulta.codMedico = historia.consulta.codMedico;
            consulta.HistoriaEnfermedad = historia.consulta.HistoriaEnfermedad;
            consulta.fechaFinalizacion = DateTime.Now;
            #endregion

            #region patologicos
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
            #endregion

            #region noPatologicos
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
            #endregion

            #region desarrollo
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
            #endregion

            #region perfil
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
            #endregion

            #region mujeres
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
            #endregion

            #region revision
            tbRevisionSistemas revision = (from t in db.tbRevisionSistemas where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            if (revision == null)
            {
                db.tbRevisionSistemas.InsertOnSubmit(historia.revision);
            }
            else
            {
                revision.conducta = historia.revision.conducta;
                revision.piel = historia.revision.piel;
                revision.cabeza = historia.revision.cabeza;
                revision.ojos = historia.revision.ojos;
                revision.oidos = historia.revision.oidos;
                revision.nariz = historia.revision.nariz;
                revision.vicios = historia.revision.vicios;
            }
            #endregion

            #region planes
            tbPlanes planes = (from t in db.tbPlanes where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            if (planes == null)
            {
                db.tbPlanes.InsertOnSubmit(historia.planes);
            }
            else
            {
                planes.planInicial = historia.planes.planInicial;
                planes.planDiagnostico = historia.planes.planDiagnostico;
                planes.diagnosticoDiferencial = historia.planes.diagnosticoDiferencial;
                planes.planEducacional = historia.planes.planEducacional;
            }
            #endregion

            #region terapeutico
            tbPlanTerapeutico terapeutico = (from t in db.tbPlanTerapeutico where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            if (terapeutico == null)
            {
                db.tbPlanTerapeutico.InsertOnSubmit(historia.terapeutico);
            }
            else
            {
                terapeutico.actividad = historia.terapeutico.actividad;
                terapeutico.dieta = historia.terapeutico.dieta;
                terapeutico.controlesEspecificos = historia.terapeutico.controlesEspecificos;
                terapeutico.otrasTerapias = historia.terapeutico.otrasTerapias;
            }
            #endregion

            #region problemas
            if (historia.problemas != null)
            {
                List<tbProblema> problemasGuardados = (from t in db.tbProblema where t.codConsulta == consulta.codConsulta select t).ToList();
                if (problemasGuardados == null)
                {
                    db.tbProblema.InsertAllOnSubmit(historia.problemas);
                }
                else
                {
                    int guardados = problemasGuardados.Count;
                    int noGuardados = historia.problemas.Count;
                    if (guardados > noGuardados)
                    {
                        int i;
                        for (i = 0; i < noGuardados; i++)
                        {
                            problemasGuardados[i].problema = historia.problemas[i].problema;
                            problemasGuardados[i].subjetivos = historia.problemas[i].subjetivos;
                            problemasGuardados[i].objetivos = historia.problemas[i].objetivos;
                            problemasGuardados[i].analisis = historia.problemas[i].analisis;
                        }
                        for (int j = i; j < guardados; j++)
                        {
                            db.tbProblema.DeleteOnSubmit(problemasGuardados[j]);
                        }
                    }
                    else if (guardados == noGuardados)
                    {
                        for (int i = 0; i < guardados; i++)
                        {
                            problemasGuardados[i].problema = historia.problemas[i].problema;
                            problemasGuardados[i].subjetivos = historia.problemas[i].subjetivos;
                            problemasGuardados[i].objetivos = historia.problemas[i].objetivos;
                            problemasGuardados[i].analisis = historia.problemas[i].analisis;
                        }
                    }
                    else
                    {
                        int i;
                        for (i = 0; i < guardados; i++)
                        {
                            problemasGuardados[i].problema = historia.problemas[i].problema;
                            problemasGuardados[i].subjetivos = historia.problemas[i].subjetivos;
                            problemasGuardados[i].objetivos = historia.problemas[i].objetivos;
                            problemasGuardados[i].analisis = historia.problemas[i].analisis;
                        }
                        for (int j = i; j < noGuardados; j++)
                        {
                            db.tbProblema.InsertOnSubmit(historia.problemas[j]);
                        }
                    }
                }
            }
            #endregion

            #region receta
            if (historia.receta != null)
            {
                List<tbReceta> recetasGuardadas = (from t in db.tbReceta where t.codConsulta == consulta.codConsulta select t).ToList();
                if (recetasGuardadas == null)
                {
                    db.tbProblema.InsertAllOnSubmit(historia.problemas);
                }
                else
                {
                    int guardados = recetasGuardadas.Count;
                    int noGuardados = historia.receta.Count;
                    if (guardados > noGuardados)
                    {
                        int i;
                        for (i = 0; i < noGuardados; i++)
                        {
                            recetasGuardadas[i].codProducto = historia.receta[i].codProducto;
                            recetasGuardadas[i].descripcion = historia.receta[i].descripcion;
                        }
                        for (int j = i; j < guardados; j++)
                        {
                            db.tbReceta.DeleteOnSubmit(recetasGuardadas[j]);
                        }
                    }
                    else if (guardados == noGuardados)
                    {
                        for (int i = 0; i < guardados; i++)
                        {
                            recetasGuardadas[i].codProducto = historia.receta[i].codProducto;
                            recetasGuardadas[i].descripcion = historia.receta[i].descripcion;
                        }
                    }
                    else
                    {
                        int i;
                        for (i = 0; i < guardados; i++)
                        {
                            recetasGuardadas[i].codProducto = historia.receta[i].codProducto;
                            recetasGuardadas[i].descripcion = historia.receta[i].descripcion;
                        }
                        for (int j = i; j < noGuardados; j++)
                        {
                            db.tbReceta.InsertOnSubmit(historia.receta[j]);
                        }
                    }
                }
            }
            #endregion

            #region signos
            tbSignosVitales signos = (from t in db.tbSignosVitales where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            if (signos == null)
            {
                db.tbSignosVitales.InsertOnSubmit(historia.signos);
            }
            else
            {
                signos.peso = historia.signos.peso;
                signos.talla = historia.signos.talla;
                signos.indiceMasaCorporal = historia.signos.indiceMasaCorporal;
                signos.presionArterial = historia.signos.presionArterial;
                signos.frecuenciaCardiaca = historia.signos.frecuenciaCardiaca;
                signos.frecuenciaRespiratoria = historia.signos.frecuenciaRespiratoria;
                signos.temperatura = historia.signos.temperatura;
                signos.circCefalica = historia.signos.circCefalica;
                signos.circAbdominal = historia.signos.circAbdominal;
                signos.focoFetal = historia.signos.focoFetal;
                signos.alturaFondoUterino = historia.signos.alturaFondoUterino;
                signos.pulso = historia.signos.pulso;
                signos.saturacionOxigeno = historia.signos.saturacionOxigeno;
            }
            #endregion

            #region diagnostico
            tbDiagnostico diagnostico = (from t in db.tbDiagnostico where t.codConsulta == historia.consulta.codConsulta select t).SingleOrDefault();
            if (diagnostico == null)
            {
                db.tbDiagnostico.InsertOnSubmit(historia.diagnostico);
            }
            else
            {
                diagnostico.impresionClinica = historia.diagnostico.impresionClinica;
            }
            #endregion

            if (historia.accion == 2)
            {
                consulta.estado = 3;
                db.SubmitChanges();
                return Url.Action("Index", "Atencion");
            }
            else
            {
                db.SubmitChanges();
                return @Url.Action("CrearHC", new { codPaciente = historia.paciente.codPaciente });
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

        [HttpPost]
        public ActionResult LlenarEditar(itemProblema problema)
        {
            ViewBag.index = problema.index;
            return PartialView("_EditarProblema", problema.problema);
        }

        public ActionResult LlenarConsultas(int codPaciente)
        {
            List<tbConsulta> listado = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.estado == 3 select t).ToList();
            return PartialView("_Consultas", listado);
        }
    }

    public class itemProblema
    {
        public int index { get; set; }
        public tbProblema problema { get; set; }
    }
}
