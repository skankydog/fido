using System.Web;
using System.Web.Mvc;
using Fido.WebUI.Filters;

namespace Fido.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection Filters)
        {
            Filters.Add(new HandleErrorAttribute());
            Filters.Add(new AntiForgeryTokenFilter());
        }
    }
}
