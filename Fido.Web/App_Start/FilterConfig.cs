using System.Web;
using System.Web.Mvc;
using Fido.Web.Filters;

namespace Fido.Web
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
