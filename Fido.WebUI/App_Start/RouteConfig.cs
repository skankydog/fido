using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Fido.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection Routes)
        {
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      //      Routes.MapRoute(
      //          name: "CompleteRegistration",
      //          url: "Account/CompleteRegistration/ConfirmationId");

      //      Routes.MapRoute(
      //          name: "CompleteForgottenPassword",
      //          url: "Account/CompleteForgottenPassword/ConfirmationId");

            Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
