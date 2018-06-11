using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BD_PR_01_Clinicas.Models;
//TODO NO SE ESTA UTILIZANDO
namespace BD_PR_01_Clinicas.Controllers
{
    public class CuentaController : Controller
    {
        DataClasesDataContext db = new DataClasesDataContext();
        // GET: Cuenta
        public ActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(FormCollection collection)
        {
            tbUsuario user = (from t in db.tbUsuario
                              where t.usuario == collection["usuario"] && t.password == collection["password"]
                              select t).SingleOrDefault();
            if (user != null)
            {
                Session["usuario"] = collection["usuario"];
                Session["password"] = collection["password"];
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                return View();
            }
        }

        public ActionResult CerrarSesion()
        {
            Session["usuario"] = null;
            Session["password"] = null;
            return RedirectToAction("IniciarSesion", "Cuenta");
        }
    }
}