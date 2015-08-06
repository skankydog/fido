using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Fido.WebUI.Flash;
using Fido.WebUI.Flash.Messages;

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
    }
}
