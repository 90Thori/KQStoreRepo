using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KQStore.Startup))]
namespace KQStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
