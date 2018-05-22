using System.Web;
using System.Web.Mvc;

namespace BD_PR_01_Clinicas
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
