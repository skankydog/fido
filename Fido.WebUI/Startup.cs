using Microsoft.Owin;
using Owin;
using Fido.Core;
using Fido.ViewModel;

[assembly: OwinStartupAttribute(typeof(Fido.WebUI.Startup))]
namespace Fido.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder App)
        {
            ViewModelFactory.Boot();

            ConfigureAuth(App);
        }
    }
}
