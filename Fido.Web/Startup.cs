using Microsoft.Owin;
using Owin;
using Fido.Core;
using Fido.ViewModel;

[assembly: OwinStartupAttribute(typeof(Fido.Web.Startup))]
namespace Fido.Web
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
