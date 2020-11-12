using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LA_VENDUTA.Startup))]
namespace LA_VENDUTA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
           
        }
    }
}
