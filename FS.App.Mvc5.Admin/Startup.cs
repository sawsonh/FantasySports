using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FS.App.Mvc5.Admin.Startup))]
namespace FS.App.Mvc5.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
