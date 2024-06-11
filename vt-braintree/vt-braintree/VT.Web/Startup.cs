using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VT.Web.Startup))]
namespace VT.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
