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
        public ActionResult Index()
        {
            List<tbPaciente> lista = (from pac in db.tbPaciente
                                      join cons in db.tbConsulta on pac.codPaciente equals cons.codPaciente
                                      where cons.atendido == true
                                      orderby pac.nombre
                                      select pac).ToList();
            return View(lista);
        }

        // GET: Pacientes/Details/5
        public ActionResult HistoriaClinica(int codPaciente)
        {
            HistoriaClinica historia = new Models.HistoriaClinica();
            historia.paciente = (from t in db.tbPaciente where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.patologicos = (from t in db.tbAntecedentesPatologicos where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.noPatologicos = (from t in db.tbAntecedentesNoPatologicos where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.desarrollo = (from t in db.tbDesarrollo where t.codPaciente == codPaciente select t).SingleOrDefault();
            return View(historia);
        }

        // GET: Pacientes/Create
        public ActionResult Create()
        {
            return View();
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
