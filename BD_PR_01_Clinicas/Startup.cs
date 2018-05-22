using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BD_PR_01_Clinicas.Startup))]
namespace BD_PR_01_Clinicas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
