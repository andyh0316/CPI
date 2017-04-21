using System.Web;
using System.Web.Optimization;

namespace Cpi.StoreWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular/angular.js",
                        "~/Scripts/angular/angular-animate.js",
                        "~/Scripts/angular/angular-ui-router.js",
                        "~/Scripts/angular/angular-base.js"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                        "~/Scripts/javascript/javascript-base.js"));

            // angular apps
            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                        "~/Views/Home/HomeApp.js"));
        }
    }
}
