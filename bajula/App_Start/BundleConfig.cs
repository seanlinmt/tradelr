using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using BundleTransformer.Yui;
using BundleTransformer.Yui.Configuration;
using BundleTransformer.Yui.Minifiers;

namespace tradelr.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;

            var yuisetting = new YuiSettings();
#if DEBUG
            yuisetting.JsMinifier.DisableOptimizations = true;
            yuisetting.JsMinifier.CompressionType = CompressionType.None;
#endif
            var cssTransformer = new CssTransformer(new YuiCssMinifier());
            var jsTransformer = new JsTransformer(new YuiJsMinifier(yuisetting));
            var nullOrderer = new NullOrderer();
            
            // internal css
            var cssBundle = new Bundle("~/css/all");
            cssBundle.Include(
                "~/Content/css/Reset.css",
                "~/Content/css/common.css",
                "~/Content/css/Site.css", 
                "~/Content/css/internal.css");
            cssBundle.IncludeDirectory("~/Content/css/fileuploader", "*.css");
            cssBundle.IncludeDirectory("~/Content/css/jgrowl", "*.css");
            cssBundle.IncludeDirectory("~/Content/css/jqueryui", "*.css");
            cssBundle.IncludeDirectory("~/Content/css/prettyLoader", "*.css");
            cssBundle.Transforms.Add(cssTransformer);
            cssBundle.Orderer = nullOrderer;

            bundles.Add(cssBundle);

             // external css
            var cssextBundle = new Bundle("~/css/ext");
            cssextBundle.Include(
                "~/Content/css/Reset.css",
                "~/Content/css/common.css",
                "~/Content/css/external.css",
                "~/Content/css/Site.css",
                "~/Content/css/jgrowl/jquery.jgrowl.css",
                "~/Content/css/prettyLoader/prettyLoader.css"
                );
            cssextBundle.Transforms.Add(cssTransformer);
            cssextBundle.Orderer = nullOrderer;

            bundles.Add(cssextBundle);

            // common css
            var csscommonBundle = new Bundle("~/css/common");
            csscommonBundle.Include(
                "~/Content/css/Reset.css",
                "~/Content/css/common.css");
            csscommonBundle.Transforms.Add(cssTransformer);
            csscommonBundle.Orderer = nullOrderer;
            bundles.Add(csscommonBundle);

            // email css
            var cssemailBundle = new Bundle("~/css/email");
            cssemailBundle.Include(
                "~/Content/css/Reset.css",
                "~/Content/css/common.css",
                "~/Content/css/email/email.css");
            cssemailBundle.Transforms.Add(cssTransformer);
            cssemailBundle.Orderer = nullOrderer;
            bundles.Add(cssemailBundle);

            // jquery js
            var jqueryBundle = new Bundle("~/js/jquery");
            jqueryBundle.Include("~/Scripts/jquery-1.8.1.js");
            jqueryBundle.Transforms.Add(jsTransformer);
            jqueryBundle.Orderer = nullOrderer;
            bundles.Add(jqueryBundle);

            var jscoreBundle = new Bundle("~/js/core");
            jscoreBundle.IncludeDirectory("~/Scripts/core", "*.js");
#if DEBUG
            jscoreBundle.Include("~/Scripts/debug_true.js");
#else
            jscoreBundle.Include("~/Scripts/debug_false.js");
#endif
            jscoreBundle.Transforms.Add(jsTransformer);
            jscoreBundle.Orderer = nullOrderer;
            bundles.Add(jscoreBundle);

            var jsextBundle = new Bundle("~/js/extend");
            jsextBundle.IncludeDirectory("~/Scripts/extend", "*.js");
            jsextBundle.Transforms.Add(jsTransformer);
            jsextBundle.Orderer = nullOrderer;
            bundles.Add(jsextBundle);
        }

    }
}