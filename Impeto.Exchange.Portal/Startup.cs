using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Impeto.Exchange.Portal.Startup))]
namespace Impeto.Exchange.Portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
