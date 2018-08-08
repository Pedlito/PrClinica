using BD_PR_01_Clinicas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BD_PR_01_Clinicas.Controllers
{
    [AutenticadoAttribute]
    public class HomeController : Controller
    {
        

        public ActionResult Index()
        {
          
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
       [PermisoAttribute(Permiso = RolesPermisos.administrar)]
        public ActionResult Administracion()
        {
            return View();
        }
  
    }
}