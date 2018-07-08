using System.Security.Claims;
using Newtonsoft.Json;
using System.Web;
using Microsoft.AspNet.Identity;

namespace BD_PR_01_Clinicas.Models
{
    public class SessionUsuario
    {
        public static CurrentUser Get
        {
            get
            {
                var user = HttpContext.Current.User;
                var jUser = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.UserData);
                if (jUser != null) { var us = jUser.Value; return JsonConvert.DeserializeObject<CurrentUser>(us); }
                else { return null; }


            }
        }
    }

}