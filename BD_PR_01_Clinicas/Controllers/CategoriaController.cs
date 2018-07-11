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
    [PermisoAttribute(Permiso = RolesPermisos.administrar_categorias)]
    public class CategoriaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Categoria
        public ActionResult Index(string filtroActual, int? page, string categoria )
        {
            List<tbCategoria> lista = null;

            if (categoria !=null)
            {
                page = 1;

            }
            else
            {
                categoria = filtroActual;   
            }

            ViewBag.filtroActual = categoria;

            if (categoria == null)
            {
                lista = (from t in db.tbCategoria orderby t.estado descending, t.categoria select t).ToList();
   
            }
            else
            {
                lista = (from t in db.tbCategoria where t.categoria.Contains(categoria) orderby t.estado descending, t.categoria select t).ToList();
            }

            

            int pageSize = 15;
            int pageNumber = (page ?? 1);
            if (lista == null) { return View(); }
            return View(lista.ToPagedList(pageNumber, pageSize));
        }

        // GET: Categoria/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Categoria/Create
        [HttpPost]
        public ActionResult Crear(FormCollection collection)
        {
            try
            {
                 //TODO: Add insert logic here
                tbCategoria nueva = new tbCategoria
                {
                    categoria = collection["categoria"],
                    estado = true
                };
                db.tbCategoria.InsertOnSubmit(nueva);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categoria/Editar/5
        public ActionResult Editar(int codCategoria)
        {
            tbCategoria editar = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
            return View(editar);
        }

        // POST: Categoria/Editar/5
        [HttpPost]
        public ActionResult Editar(int codCategoria, FormCollection collection)
        {
            try
            {
                //TODO: Add update logic here
                tbCategoria editar = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
                editar.categoria = collection["categoria"];
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Categoria/Delete/5
        public ActionResult CambiarEstado(int codCategoria, bool estado)
        {
            tbCategoria cambio = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
            return View(cambio);
        }

        // POST: Categoria/Delete/5
        [HttpPost]
        public ActionResult CambiarEstado(int codCategoria, bool estado, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                tbCategoria cambio = (from t in db.tbCategoria where t.codCategoria == codCategoria select t).SingleOrDefault();
                cambio.estado = estado;
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
