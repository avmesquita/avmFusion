using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Impeto.SalaDeReuniao.Startup))]
namespace Impeto.SalaDeReuniao
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
