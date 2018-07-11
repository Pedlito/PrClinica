using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;

namespace BD_PR_01_Clinicas.Controllers
{
    public class ReportesController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Reportes
        public ActionResult Index()
        {
            return View();
        }

        // GET: Reportes/Details/5
        public ActionResult ConsultasFechas(string inicio = "", string fin = "")
        {
            List<ConsultasFechas> lista = null;
            if (inicio != "" && fin != "")
            {
                lista = (from consulta in db.tbConsulta
                         join diagnostico in db.tbDiagnostico on consulta.codConsulta equals diagnostico.codConsulta
                         where diagnostico.fecha.Value.Date >= DateTime.Parse(inicio) & diagnostico.fecha.Value.Date <= DateTime.Parse(fin)
                         select new ConsultasFechas
                         {
                             medico = diagnostico.tbUsuario.nombre,
                             estudiante = diagnostico.tbUsuario1.nombre,
                             fecha = diagnostico.fecha.Value,
                             paciente = consulta.tbPaciente.nombre
                         }).ToList();
            }
            ViewBag.inicio = inicio;
            ViewBag.fin = fin;
            return View(lista);
        }

        // GET: Reportes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reportes/Create
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

        // GET: Reportes/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reportes/Edit/5
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

        // GET: Reportes/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reportes/Delete/5
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
