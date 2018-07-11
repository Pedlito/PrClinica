using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BD_PR_01_Clinicas.Models
{

    public class FrontUser
    {
        //RolesPermisos valor
        public static bool TienePermiso(RolesPermisos valor)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                using (var context = new DataClasesDataContext())
                {

                    tbUsuario usuario = (from t in context.tbUsuario
                                         where t.codUsuario == SessionUsuario.Get.UserId
                                         select t).SingleOrDefault();

                    return usuario.tbRol.tbRolPermiso.Where(n => n.idPermiso == (int)valor).Any();

                }


            }
            else { return false; }

        }

        public static bool PuedeRegistrarse()
        {
            try
            {
                using (var mibase = new DataClasesDataContext())
                {
                    tbConfiguracion tc = mibase.tbConfiguracion.Where(x => x.codConfiguracion == 1).SingleOrDefault();
                    if (!HttpContext.Current.User.Identity.IsAuthenticated && tc != null)
                    {
                        return tc.valor;
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
    public enum RolesPermisos
    {
        #region permisos
        consultar_existencia = 1,
        administrar_entradas = 2,
        administrar_salidas = 3,
        administrar_catalogo_prod = 4,
        administrar_categorias = 5,
        administrar_presentaciones = 6,
        administrar_usuarios = 7,
        administrar_registro = 8,
        administrar_roles = 9,
        administrar_consultas = 10,
        administrar_pacientes = 11,
        administrar_rotaciones = 12,
        administrar= 13
        #endregion
    }


    public class PermisoAttribute : ActionFilterAttribute
    {
        public RolesPermisos Permiso { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!FrontUser.TienePermiso(this.Permiso))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",//direccionar a una pagina donde se indique que no tiene permiso.
                    action = "Index"
                }));
            }
        }
    }

    // Si no estamos logeado, regresamos al login
    public class AutenticadoAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Account",
                    action = "Login"
                }));
            }
        }
    }

    //Si estamos logeado ya no podemos acceder a la página de Login
    public class NoLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Home",
                    action = "Index"
                }));
            }
        }
    }
}