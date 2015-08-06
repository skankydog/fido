using Microsoft.Owin;
using Owin;
using Fido.Core;
using Fido.Action;

[assembly: OwinStartupAttribute(typeof(Fido.WebUI.Startup))]
namespace Fido.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder App)
        {
            ActionFactory.Boot();

            ConfigureAuth(App);
        }
    }
}
