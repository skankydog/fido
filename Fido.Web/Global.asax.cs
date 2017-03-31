using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Fido.ViewModel;
using Fido.Web.Binders;

namespace Fido.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            string l4net = Server.MapPath("~/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(l4net));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Remove(typeof(byte[]));
            ModelBinders.Binders.Add(typeof(byte[]), new CustomByteArrayModelBinder());
            ModelBinders.Binders.Add(typeof(ListOptions), new IndexParamsModelBinder());
        }
    }
}
