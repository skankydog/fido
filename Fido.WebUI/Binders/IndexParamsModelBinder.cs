using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fido.Action;

namespace Fido.WebUI.Binders
{
    public class IndexParamsModelBinder : DefaultModelBinder
    {
        private HttpRequestBase Request;

        public override object BindModel(ControllerContext ControllerContext, ModelBindingContext BindingContext)
        {
            if (BindingContext.ModelType == typeof(IndexParams))
            {
                Request = ControllerContext.HttpContext.Request;
                IndexParams Model = new IndexParams();

                Model.Echo = Request["sEcho"];
                Model.Filter = Request["sSearch"];
                Model.Take = Convert.ToInt32(Request["iDisplayLength"]);
                Model.Skip = Convert.ToInt32(Request["iDisplayStart"]);
                Model.Columns = Request["sColumns"].Split(',').ToList();

                Model.SortColumns = GetList("iSortCol_");
                Model.SortOrders = GetList("sSortDir_");

                return Model;
            }

            return base.BindModel(ControllerContext, BindingContext);
        }

        private IList<string> GetList(string Name)
        {
            var Index = 0;
            var Items = new List<string>();
            var Item = Request[Name + Index.ToString()];

            while (Item != null)
            {
                Items.Add(Item);
                Item = Request[Name + ++Index];
            }

            return Items;
        }
    }
}
