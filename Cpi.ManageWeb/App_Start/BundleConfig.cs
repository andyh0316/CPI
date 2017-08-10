using System.Web.Optimization;

namespace Cpi.ManageWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular/angular.js",
                        "~/Scripts/angular/angular-animate.js",
                        "~/Scripts/angular/angular-ui-router.js",
                        "~/Scripts/angular/angular-base.js",
                        "~/Scripts/angular/ng-infinite-scroll.js"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                        "~/Scripts/javascript/javascript-base.js",
                        "~/Scripts/javascript/moment.js",
                        "~/Scripts/javascript/moment-timezone-with-data.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/site.css",
                    "~/Content/print.css",
                    "~/Content/MaterialIcons/material-icons.css",
                    "~/Content/Fonts/fonts.css"));

            string[] chartBundles = new string[] 
            {
                "~/Scripts/javascript/Chart.js",
                "~/Scripts/angular/angular-chart.js",
            };

            // angular apps
            bundles.Add(new ScriptBundle("~/bundles/public").Include(
                        "~/Views/Public/PublicApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/call/call").Include(
                        "~/Areas/Call/Views/Call/CallApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/invoice/invoice").Include(
                        "~/Areas/Invoice/Views/Invoice/InvoiceApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/finance/finance")
                .Include("~/Areas/Finance/Views/Finance/FinanceApp.js")
                .Include(chartBundles));

            bundles.Add(new ScriptBundle("~/bundles/finance/financeList")
                .Include("~/Areas/Finance/Views/FinanceList/FinanceListApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/performance/performance")
                .Include("~/Areas/Performance/Views/Performance/PerformanceApp.js")
                .Include(chartBundles));

            bundles.Add(new ScriptBundle("~/bundles/expense/expense").Include(
                        "~/Areas/Expense/Views/Expense/ExpenseApp.js"));

            bundles.Add(new ScriptBundle("~/bundles/user/user").Include(
                        "~/Areas/User/Views/User/UserApp.js"));
        }
    }
}