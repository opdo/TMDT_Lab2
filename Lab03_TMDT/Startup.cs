using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Lab03_TMDT.Startup))]
namespace Lab03_TMDT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
