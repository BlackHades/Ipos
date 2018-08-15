using System.Web.Optimization;

namespace SmartWr.Ipos.Core.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            ScriptBundle jsScripts = new ScriptBundle("~/Scripts/jsScripts");
            jsScripts.Include("~/Scripts/jquery-1.10.2.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/detect.js",
                "~/Scripts/fastclick.js",
                "~/Scripts/jquery.slimscroll.js",
                "~/Scripts/waves.js",
                "~/Scripts/wow.min.js",
                "~/Scripts/jquery.nicescroll.js",
                "~/Scripts/jquery.scrollTo.min.js",
                "~/Scripts/jquery.peity.min.js",
                "~/Scripts/jquery.waypoints.js",
                "~/Scripts/jquery.counterup.min.js",
                 "~/Scripts/angular.js",
                "~/Scripts/angular-block-ui.js ",
                "~/Scripts/angular-ui-router.js",
                "~/Scripts/angular-resource.min.js",
              "~/Scripts/angular-sanitize.min.js",
                "~/Scripts/dirPagination.js",
                "~/Scripts/smartwr/serialize2json.js",
                "~/Scripts/loading-bar.js",
                "~/Scripts/jquery.core.js",
                "~/Scripts/jquery.app.js",
                 "~/Scripts/smartwr/serialize2json.js",
                 "~/Content/plugins/notifyjs/dist/notify.min.js",
               "~/Content/plugins/notifyjs/dist/styles/metro/notify-metro.js",
                "~/Scripts/modernizr.min.js");

            bundles.Add(jsScripts);

            //Bundling for styles

            var cssStyles = new StyleBundle("~/Styles/cssStyles");
            cssStyles.Include("~/Content/plugins/morris/morris.css",
                "~/Content/css/bootstrap.min.css",
                "~/Content/css/components.css",
                //"~/Content/css/icons.css",
                "~/Content/css/pages.css",
                "~/Content/css/responsive.css",
                "~/Content/loading-bar.css",
                "~/Content/angular-block-ui.css",
                "~/Content/css/styles.css"
                );

            bundles.Add(cssStyles);
            //BundleTable.EnableOptimizations = true;

        }
    }
}
