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
                         where consulta.fechaLlegada.Value.Date >= DateTime.Parse(inicio) & consulta.fechaLlegada.Value.Date <= DateTime.Parse(fin)
                         select new ConsultasFechas
                         {
                             medico = consulta.tbMedico.nombre,
                             estudiante = consulta.tbEstudiante.nombre,
                             fecha = consulta.fechaLlegada.Value,
                             paciente = consulta.tbPaciente.nombre
                         }).ToList();
            }
            ViewBag.inicio = inicio;
            ViewBag.fin = fin;
            return View(lista);
        }

        // GET: Reportes/ConsultasEstudiante
        public ActionResult ConsultasEstudiante()
        {
            return View();
        }

        // GET: Reportes/FiltrarEstudiantes/5
        public ActionResult FiltrarEstudiantes(string filtro)
        {
            List<tbUsuario> listado = (from t in db.tbUsuario where t.nombre.Contains(filtro) && t.codTipoUsuario == 2 select t).Take(15).ToList();
            return PartialView("_Estudiantes", listado);
        }
        public ActionResult GetConsultasEstudiante(int codUsuario)
        {
            List<ClasConsultasEstudiante> listado = (from t in db.tbConsulta
                                                     where t.codEstudiante == codUsuario && t.estado == 3
                                                     select new ClasConsultasEstudiante
                                                     {
                                                         paciente = t.tbPaciente.nombre,
                                                         medico = t.tbMedico.nombre,
                                                         fechaInicio = t.fechaAtencion.Value,
                                                         fechaFin = t.fechaFinalizacion.Value
                                                     }).ToList();
            return PartialView("_ConsultasEstudiante", listado);
        }

        public ActionResult ProductosMasRecetados()
        {
            List<RegistroProducto> listado = (from receta in db.tbReceta
                                              join producto in db.tbProducto on receta.codProducto equals producto.codProducto
                                              group receta by new
                                              {
                                                  receta.codProducto,
                                                  producto.producto,
                                                  producto.tbCategoria.categoria,
                                                  producto.tbPresentacion.presentacion,
                                                  producto.dosis,
                                                  producto.codVolumen,
                                              } into grupo
                                              select new RegistroProducto
                                              {
                                                  nombre = grupo.Key.producto,
                                                  categoria = grupo.Key.categoria,
                                                  presentacion = grupo.Key.presentacion,
                                                  dosis = grupo.Key.dosis.ToString() + ((grupo.Key.codVolumen == 1) ? " mg" : " ml"),
                                                  cantidad = grupo.Count()
                                              }).ToList();
            return View(listado);
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
