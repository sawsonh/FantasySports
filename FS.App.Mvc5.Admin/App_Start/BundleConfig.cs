using System.Web.Optimization;

namespace FS.App.Mvc5.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"
                      , "~/Scripts/bootstrap-datepicker.js"
                      //,"~/Scripts/plugins/morris/raphael.min.js"
                      //,"~/Scripts/plugins/morris/morris.min.js"
                      //,"~/Scripts/plugins/morris/morris-data.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/gridmvc").Include(
                      "~/Scripts/gridmvc.min.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/game").Include(
                      "~/Scripts/Game.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/game/addperiod").Include(
                      "~/Scripts/GameAddPeriod.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/sb-admin.css",
                      "~/Content/plugins/morris.css",
                      "~/Content/plugins/anytime.5.0.5.css"));

            bundles.Add(new StyleBundle("~/Content/css/Gridmvc").Include(
                      "~/Content/Gridmvc.css"
                      ,"~/Content/Gridmvc.datepicker.min.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/css/font-awesome").Include(
                      "~/Content/font-awesome/css/font-awesome.min.css"));
        }
    }
}
