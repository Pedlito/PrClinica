using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class PacientesController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Pacientes
        public ActionResult Index(string filtro = "")
        {
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
                         orderby t.nombre where t.nombre.Contains(filtro)
                         select t).ToList();
            }

            return View(lista);
        }

        // GET: Pacientes/Details/5
        public ActionResult HistoriaClinica(int codPaciente)
        {
            HistoriaClinica historia = new Models.HistoriaClinica();
            historia.consultas = (from t in db.tbConsulta where t.codPaciente == codPaciente select t).ToList();
            historia.paciente = (from t in db.tbPaciente where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.patologicos = (from t in db.tbAntecedentesPatologicos where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.noPatologicos = (from t in db.tbAntecedentesNoPatologicos where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.desarrollo = (from t in db.tbDesarrollo where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.perfilSocial = (from t in db.tbPerfilSocial where t.codPaciente == codPaciente select t).SingleOrDefault();
            if (historia.paciente.genero == false)
            {
                historia.mujeres = (from t in db.tbMujeres where t.codPaciente == codPaciente select t).SingleOrDefault();
            }
            historia.perfilSocial = (from t in db.tbPerfilSocial where t.codPaciente == codPaciente select t).SingleOrDefault();
            //historia.revision = (from t in db.tbRevisionSistemas where t.codConsulta == codPaciente select t).SingleOrDefault(); para despues
            return View(historia);
        }

        // GET: Pacientes/VerConsulta
        public ActionResult VerConsulta(int codConsulta)
        {
            consultas consulta = new consultas();
            consulta.consulta = (from t in db.tbConsulta where t.codConsulta == codConsulta select t).SingleOrDefault();
            consulta.codPaciente = consulta.consulta.codPaciente.Value;
            consulta.revision = (from t in db.tbRevisionSistemas where t.codConsulta == codConsulta select t).SingleOrDefault();
            consulta.signos = (from t in db.tbSignosVitales where t.codConsulta == codConsulta select t).SingleOrDefault();
            consulta.problemas = (from t in db.tbProblema where t.codConsulta == codConsulta select t).ToList();
            consulta.planes = (from t in db.tbPlanes where t.codConsulta == codConsulta select t).SingleOrDefault();
            consulta.terapeutico = (from t in db.tbPlanTerapeutico where t.codConsulta == codConsulta select t).SingleOrDefault();
            consulta.receta = (from t in db.tbReceta where t.codConsulta == codConsulta select t).ToList();
            consulta.diagnostico = (from t in db.tbDiagnostico where t.codConsulta == codConsulta select t).SingleOrDefault();
            return View(consulta);
        }

        // POST: Pacientes/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Pacientes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Pacientes/Edit/5
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

        // GET: Pacientes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Pacientes/Delete/5
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
