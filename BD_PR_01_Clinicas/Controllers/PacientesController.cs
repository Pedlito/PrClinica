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
    [PermisoAttribute(Permiso = RolesPermisos.administrar_pacientes)]
    public class PacientesController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Pacientes
        public ActionResult Index(int? page, string filtroActual, string filtro )
        {
            List<tbPaciente> lista = null;
            if (filtro!= null)
            {
                page = 1;

            }
            else
            {
                filtro = filtroActual;
            }

            ViewBag.filtroActual = filtro;

            if (filtro == null)
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
            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));

        }

        // GET: Pacientes/Details/5
        public ActionResult HistoriaClinica(int codPaciente)
        {
            HistoriaClinica historia = new Models.HistoriaClinica();
            historia.consultas = (from t in db.tbConsulta where t.codPaciente == codPaciente && t.estado == 3 orderby t.fechaLlegada descending select t).ToList();
            historia.paciente = (from t in db.tbPaciente where t.codPaciente == codPaciente select t).SingleOrDefault();
            historia.patologicos = historia.paciente.tbAntecedentesPatologicos;
            historia.noPatologicos = historia.paciente.tbAntecedentesNoPatologicos;
            historia.desarrollo = historia.paciente.tbDesarrollo;
            historia.perfilSocial = historia.paciente.tbPerfilSocial;
            if (historia.paciente.genero == false)
            {
                historia.mujeres = historia.paciente.tbMujeres;
            }
            historia.perfilSocial = historia.paciente.tbPerfilSocial;
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
                                   dosis = RegistroProducto.Dosis(t.dosis.ToString(), t.codVolumen.Value, t.dosis2.ToString(), t.codVolumen2.Value),
                                   descripcion = item.descripcion
                               }).SingleOrDefault());
                }
            }
            //el retorno sera el html creado en la vista parcial _detalle pasandole como modelo la lista
            return PartialView("_Detalle", lista);
        }
    }
}
