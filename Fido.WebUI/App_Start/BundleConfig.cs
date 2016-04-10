﻿// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
using System.Web;
using System.Web.Optimization;

namespace Fido.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection Bundles)
        {
            Bundles.Add(new StyleBundle("~/nolayout.styles")
                .Include("~/Content/css/bootstrap.css")
                .Include("~/Content/css/nolayout.css"));

            Bundles.Add(new StyleBundle("~/layout.styles")
                .Include("~/Content/css/bootstrap.css")
                .Include("~/Content/css/layout.css"));

            Bundles.Add(new StyleBundle("~/administration.styles")
                .Include("~/Content/css/bootstrap.css")
                .Include("~/Content/css/administration.css")
                .Include("~/Content/bootstrap-chosen.css")
                .Include("~/Content/css/plugins/metisMenu/metisMenu.min.css")
                .Include("~/Content/css/plugins/dataTables.bootstrap.css")
                .Include("~/Content/css/gridforms.css")); // Not sure I am going to use this one. Consider removing

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            Bundles.Add(new ScriptBundle("~/page.top.scripts")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/chosen.jquery.js")
                .Include("~/Scripts/jquery.unobtrusive-ajax.js")
                .Include("~/Scripts/plugins/dataTables/jquery.dataTables.js")
                .Include("~/Scripts/plugins/dataTables/dataTables.bootstrap.js")
                .Include("~/Scripts/gridforms.js")
                .Include("~/Scripts/modernizr-*"));

            Bundles.Add(new ScriptBundle("~/page.bottom.scripts")
                .Include("~/Scripts/jquery.validate*")
                .Include("~/Scripts/bootstrap.js")
                .Include("~/Scripts/site.js")
                .Include("~/Scripts/plugins/metisMenu/metisMenu.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            // BundleTable.EnableOptimizations = true;
        }
    }
}
