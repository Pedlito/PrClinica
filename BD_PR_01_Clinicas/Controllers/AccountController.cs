using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BD_PR_01_Clinicas.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BD_PR_01_Clinicas.Controllers
{

   
    public class AccountController : Controller
    {
        

        DataClasesDataContext db = new DataClasesDataContext();
  
        [NoLoginAttribute]
        public ActionResult Login(string returnUrl)
        {

            try
            {
                ViewBag.registro = (from t in db.tbConfiguracion where t.codConfiguracion == 1 select t.valor).SingleOrDefault();
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception)
            {

                return HttpNotFound("Error en la conexion con la base de datos");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {

            try
            {
                // Verification.    
                if (ModelState.IsValid)
                {
                    // Initialization.    
                    var loginInfo = db.Logiar(model.UserName, model.Password).ToList();
                    // Verification.    

                    if (loginInfo != null && loginInfo.Count() > 0)
                    {
                        // Initialization.    
                        var logindetails = loginInfo.First();
                        //JsonConvert.SerializeObject(
                        // Your User Data
                        var JUser = JsonConvert.SerializeObject(new CurrentUser
                        {
                            UserId = logindetails.codUsuario,
                            UserName = logindetails.usuario
                        });
                       
                        // Login In.
                        SignInUser(JUser, false);
                        //id del usuario
                        //Session["UserId"] = loginInfo.ElementAt(0).codUsuario;
                        // Info.    
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        //   Console.Write(loginInfo.ToString());
                        // Setting.    
                        ViewBag.registro = (from t in db.tbConfiguracion where t.codConfiguracion == 1 select t.valor).SingleOrDefault();
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    }
                }
            }
            catch (Exception)
            {
                // quitar esto es solo para prueba              
                ModelState.AddModelError(string.Empty, "Fallo en la conexion con la base de datos");
            }

            ViewBag.registro = FrontUser.PuedeRegistrarse();
            ViewBag.ReturnUrl = returnUrl;
            return this.View(model);
        }

      
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        { 
            if (FrontUser.PuedeRegistrarse())
            {
                return View();
            }
           
            return RedirectToAction("MensajeDeRegistro","Rotacion");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult Register(RegisterViewModel model)
        {
            //se corrigio un error feo que da cuando no se puede conectar a la base de datos
            if (!string.IsNullOrEmpty(model.Usuario)&&db.tbUsuario.Where(m => m.usuario == model.Usuario.ToUpper()).Any()) { ModelState.AddModelError("Usuario","El usuario ingresado ya existe."); }

            if (ModelState.IsValid)
            {
                tbUsuario NuevoUsuario = new tbUsuario
                {
                    nombre = model.Nombre,
                    dpi = model.Dpi,
                    carnet = model.Carnet,
                    fechaNacimiento = model.FechaNacimiento,
                    usuario = model.Usuario.ToUpper(),
                    password = model.Password,
                    codTipoUsuario = 1,
                    estado = true
                  
                };
                try
                {
                    db.tbUsuario.InsertOnSubmit(NuevoUsuario);
                    db.SubmitChanges();
                }
                catch (Exception )
                {
                    ModelState.AddModelError(string.Empty, "No se pudo realizar la operacion");
                    return View(model);
                }

               return RedirectToAction("Login", "Account");
                
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
           
            try
            {
                // Setting.    
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign Out.    
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
            // Info.    
            return this.RedirectToAction("Index", "Home");
        }


    
        private void SignInUser(string datos, bool isPersistent)
        {
            // Initialization.    
            var claims = new List<Claim>();
            try
            {
                // Setting    
                claims.Add(new Claim(ClaimTypes.UserData, datos));
              

                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;
                // Sign In.    
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                // Info    
                throw ex;
            }
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }


        [AutenticadoAttribute]
        public ActionResult DatosUsuario() {

    
                try
                {
                    using (var context = new DataClasesDataContext())
                    {

                        tbUsuario item = (from t in context.tbUsuario
                                          where t.codUsuario == SessionUsuario.Get.UserId         
                                          select t).SingleOrDefault();
                        return View(item);
                    }



                }
                catch (Exception)
                {
                    ViewBag.errores = "No se ha podido cargar los datos";

                }
      
            return View("VistaDeErrores");

        }
        private static bool autorizado = false;
        [AutenticadoAttribute]
        public JsonResult autenticarse(string usuario, string contrasenia)
        {
            string resultado = "";

            if (usuario != null && contrasenia != null)
            {

                var loginInfo = db.Logiar(usuario, contrasenia).ToList();

                if (loginInfo != null && loginInfo.Count() > 0)
                {


                    resultado = "ok";
                    autorizado = true;

                }
                else
                {
                    resultado = "contraseña invalida";
                }

            }

            return Json(resultado);
        }

        [AutenticadoAttribute]
        public ActionResult AutoEditarUsuario(int? id) {

            if (autorizado)
            {
                try
                {
                    using (var context = new DataClasesDataContext())
                    {
                        tbUsuario item = (from t in context.tbUsuario
                                          where t.codUsuario == id
                                          select t).SingleOrDefault();
                        autorizado = false;
                        return View(item);
                    }

                }
                catch (Exception)
                {
                    ViewBag.errores = "Error: Al al cargar los datos ";

                }
            }
            else { ViewBag.errores = "Error: Operacion no autorizada"; }
      
            return View("VistaDeErrores");
        }
        [HttpPost]
        [AutenticadoAttribute]
        public ActionResult AutoEditarUsuario(tbUsuario user)
        {

            if (user != null)
            {
                using (var contexto = new DataClasesDataContext())
                {

                    try
                    {
                        tbUsuario item = (from t in contexto.tbUsuario
                                          where t.codUsuario == user.codUsuario
                                          select t).SingleOrDefault();

                        item.nombre = user.usuario;
                        item.dpi = user.dpi;
                        item.carnet = user.carnet;
                        item.fechaNacimiento = user.fechaNacimiento;
                       // item.usuario = user.usuario;
                        item.password = user.password;
                        contexto.SubmitChanges();
                        return RedirectToAction("DatosUsuario");
                    }
                    catch (Exception)
                    {

                        ViewBag.errores = "Error: No se puedo guardar los cambios";
                    }
                }
            }
            else
            {
                ViewBag.errores = "Error: No se puedo guardar los cambios";

            }
            return View("VistaDeErrores");
        }

        [HttpPost]
        public JsonResult UsuarioRepetido(string usuario)
        {
            try
            {
                if (db.tbUsuario.Where(x => x.usuario == usuario).Any())
                {
                    return Json(usuario);
                }

                return Json("");
            }
            catch
            {
                return Json("Fatal");
            }
        }
    }


}