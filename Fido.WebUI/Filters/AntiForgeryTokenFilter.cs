using System.Net.Http;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Fido.WebUI.Filters
{
    public class SkipCSRFCheckAttribute : ActionFilterAttribute
    {
    }

    public class AntiForgeryTokenFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext FilterContext)
        {
            if (FilterContext.RequestContext.HttpContext.Request.HttpMethod == HttpMethod.Post.ToString() &&
                FilterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipCSRFCheckAttribute), false).Length == 0)
            {
                AntiForgery.Validate();
            }
        }
    }
}
