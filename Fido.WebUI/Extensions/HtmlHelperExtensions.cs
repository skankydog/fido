using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Fido.WebUI.Flash;
using Fido.WebUI.Flash.Messages;
using Fido.Action.Implementation;

namespace Fido.WebUI.Extensions
{
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

        public static MvcHtmlString MyAchor<TMODEL>(this HtmlHelper<TMODEL> Helper, string Action, string Controller, string Area, string Output)
            where TMODEL : IDataModel
        {
            TMODEL Model = Helper.ViewData.Model;
            MvcHtmlString Return = MvcHtmlString.Create("");

        //    if (Model.HasReadPermission("AdHome", "Administration"))
                Return = MvcHtmlString.Create("");

            //MvcHtmlString x = MvcHtmlString.Create("<li><a href=www.skankydog.com></a></li>");
            MvcHtmlString x = MvcHtmlString.Create("<div>" + Model.GetType().ToString() + "</div>");
            // Ok - I should have the model here

            return x;
        }

        // <li><a href="@Url.Action("Index", "AdHome", new { Area = "Administration" })"><i class="fa fa-2x fa-cog fa-fw"></i></a></li>
        public static MvcHtmlString ListItem<TMODEL>(this HtmlHelper<TMODEL> Helper, string Action, string Controller, string Area)
            where TMODEL : IDataModel
        {
            TMODEL Model = Helper.ViewData.Model;
            MvcHtmlString Return = MvcHtmlString.Create("hello");// @Url.Action("Index", "AdHome", new { Area = "Administration" });

       //     if (Model.HasReadPermission("AdHome", "Administration"))
                Return = MvcHtmlString.Create("");

            //MvcHtmlString x = MvcHtmlString.Create("<li><a href=www.skankydog.com></a></li>");
            MvcHtmlString x = MvcHtmlString.Create("<div>" + Model.GetType().ToString() + "</div>");
            // Ok - I should have the model here

            return x;
        }
    }
}
