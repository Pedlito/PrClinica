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
        public AccountController()
        {

        }

      
   
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(LoginViewModel model, string returnUrl)
        //{

        //    try
        //    {
        //        // Verification.    
        //        if (ModelState.IsValid)
        //        {
        //            // Initialization.    
        //            var loginInfo = db.Logiar(model.UserName, model.Password).ToList();
        //            // Verification.    

        //            if (loginInfo != null && loginInfo.Count() > 0)
        //            {
        //                // Initialization.    
        //                var logindetails = loginInfo.First();
        //                // Login In.    
        //                SignInUser(logindetails.usuario, false);
        //                //id del usuario
        //                Session["UserId"] = loginInfo.ElementAt(0).codUsuario;
        //                // Info.    
        //                return RedirectToLocal(returnUrl);
        //            }
        //            else
        //            {
        //                Console.Write(loginInfo.ToString());
        //                // Setting.    
        //                ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // quitar esto es solo para prueba              
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //    }

        //    return this.View(model);
        //}
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
                        Console.Write(loginInfo.ToString());
                        // Setting.    
                        ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    }
                }
            }
            catch (Exception ex)
            {
                // quitar esto es solo para prueba              
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return this.View(model);
        }

       private static bool autorizado = false;
        public JsonResult autenticar(string usuario, string contrasenia) {
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
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //return View();
            tbConfiguracion tc = (from v in db.tbConfiguracion where v.codConfiguracion == 1 select v).SingleOrDefault();
            //new configuracioInicial().RegistroHabilitado
            if ((tc!=null)&&(tc.valor))
            {
                return View();
            }
            return RedirectToAction("MensajeDeRegistro", "Rotacion");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                tbUsuario NuevoUsuario = new tbUsuario
                {
                  nombre = model.Nombre,
                  dpi = model.Dpi,
                  carnet = model.Carnet,
                  fechaNacimiento =  model.FechaNacimiento,
                  usuario = model.Usuario,
                  password = model.Password,
                  codTipoUsuario = 2
                };
                try
                {
                    db.tbUsuario.InsertOnSubmit(NuevoUsuario);
                    db.SubmitChanges();
                }
                catch (Exception exs)
                {
                    ModelState.AddModelError(string.Empty, exs.Message);
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
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //return RedirectToAction("Index", "Home");
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


        //private void SignInUser(string username, bool isPersistent)
        //{
        //    // Initialization.    
        //    var claims = new List<Claim>();
        //    try
        //    {
        //        // Setting    
        //        claims.Add(new Claim(ClaimTypes.Name, username));
        //        //claims.Add(new Claim(ClaimTypes.UserData, username));

        //        var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
        //        var ctx = Request.GetOwinContext();
        //        var authenticationManager = ctx.Authentication;
        //        // Sign In.    
        //        authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Info    
        //        throw ex;
        //    }
        //}
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

        public ActionResult DatosUsuario() {

            if (Request.IsAuthenticated)
            {
                try
                {
                    using (var context = new DataClasesDataContext())
                    {

                        tbUsuario item = (from t in context.tbUsuario
                                          where t.codUsuario == HttpUsuario.Get.UserId
                                          select t).SingleOrDefault();
                        return View(item);
                    }



                }
                catch (Exception)
                {
                    ViewBag.errores = "No se ha podido cargar los datos";

                }
            }
            else { ViewBag.errores = "Error: Se ha cerrado sesion"; }

            return View("VistaDeErrores");


        }
  
        public ActionResult EditarUsuario(int? id) {

            if (Request.IsAuthenticated&&autorizado)
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
        public ActionResult EditarUsuario(tbUsuario user)
        {

            if (user != null && Request.IsAuthenticated)
            {
                using (var contexto = new DataClasesDataContext())
                {

                    try
                    {
                        tbUsuario item = (from t in contexto.tbUsuario
                                          where t.codUsuario == user.codUsuario
                                          select t).SingleOrDefault();

                        item.nombre = user.nombre;
                        item.dpi = user.dpi;
                        item.carnet = user.carnet;
                        item.fechaNacimiento = user.fechaNacimiento;
                        item.usuario = user.usuario;
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
    }


}