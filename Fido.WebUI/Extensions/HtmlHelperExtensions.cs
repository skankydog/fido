using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Fido.WebUI.Flash;
using Fido.WebUI.Flash.Messages;
using Fido.Action.Implementation;

namespace Fido.WebUI.Extensions
{
    public enum LinkType
    {
        Normal = 0,
        Modal
    }

    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Flash<TYPE>(this HtmlHelper<TYPE> Helper)
        {
            var Flasher = new Flasher(); // *Think* I can get from (BaseController)(Helper.ViewContext.Controller).Flasher
            var Builder = new StringBuilder();

            while (Flasher.Count > 0)
            {
                FlashMessage Message = Flasher.Pop();
                
                var Name = Message.Data == null
                    ? Message.GetType().Name.ToString()
                    : Message.Data.GetType().Name.ToString();

                Builder.AppendLine(Helper.DisplayFor(m => Message, Name).ToString());
            }

            return MvcHtmlString.Create(Builder.ToString());
        }

        public static TYPE Cast<TYPE>(this object Value)
        {
            if (Value == null)
                return default(TYPE);

            return (TYPE)Value;
        }

        // <li><a href="@Url.Action("Index", "AdHome", new { Area = "Administration" })"><i class="fa fa-2x fa-cog fa-fw"></i></a></li>
        public static string MyUrl<TMODEL>(this HtmlHelper<TMODEL> Html, string Display, string Action, string Controller, string Area = "")
            where TMODEL : IDataModel
        {
            TMODEL Model = Html.ViewData.Model;

            // TO DO: check permissions

            var Url = new UrlHelper(Html.ViewContext.RequestContext);
            var y = Url.Action(Action, Controller, new { Area = Area });

            return y.ToString();
        }

        public static MvcHtmlString li<TMODEL>(this HtmlHelper<TMODEL> Html, LinkType LinkType, string Display, string Action, string Controller, string Area = "")
            where TMODEL : IDataModel
        {
            TMODEL Model = Html.ViewData.Model;

            // TO DO: check permissions

            var Url = new UrlHelper(Html.ViewContext.RequestContext);
            var Link = Url.Action(Action, Controller, new { Area = Area });
            var AnchorClass = LinkType == LinkType.Modal ? "class = modal-link" : "";

            var LI = string.Concat("<li><a ", AnchorClass, " href=\"", Link.ToString(), "\">", Display, "</a></li>");

            return new MvcHtmlString(LI);
        }
    }
}
