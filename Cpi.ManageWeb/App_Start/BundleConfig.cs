﻿using System.Web.Optimization;

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
                        "~/Scripts/angular/angular-base.js"));

            bundles.Add(new ScriptBundle("~/bundles/javascript").Include(
                        "~/Scripts/javascript/javascript-base.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/site.css",
                    "~/Content/MaterialIcons/material-icons.css",
                    "~/Content/Fonts/fonts.css"));

            // angular apps
            bundles.Add(new ScriptBundle("~/bundles/call/call").Include(
                        "~/Areas/Call/Views/Call/callApp.js"));
        }
    }
}