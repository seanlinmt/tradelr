using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;

using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.google.gears;
using tradelr.Models.users;

namespace tradelr.Controllers.offline
{
    //[ElmahHandleError]
    [CompressFilter]
    [RoleFilter(role = UserRole.CREATOR)]
    public class manifestController : baseController
    {
        /// <summary>
        /// for HTML5 offline access
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            StringBuilder sb = new StringBuilder();
            bool notfound = false;
            sb.AppendLine("CACHE MANIFEST");
            sb.AppendLine("# " + GeneralConstants.TIMESTAMP);
            sb.AppendLine("FALLBACK:");
            sb.AppendLine("/dashboard/product/edit /dashboard/product/edit");
            sb.AppendLine("/dashboard/category/addsub /dashboard/category/addsub");
            sb.AppendLine("NETWORK:");
            sb.AppendLine("*");
            sb.AppendLine("CACHE:");
            foreach (var entry in URLS_TO_CACHE)
            {
#if DEBUG
                // ignore invalid certificates
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate { return true; };
                // verify that url is valid as it is a pita to debug applicationCache errors
                using (var client = new HeadClient())
                {
                    try
                    {
                        var head = client.DownloadString(accountHostname.ToDomainUrl(entry));
                    }
                    catch (Exception ex)
                    {
                        Syslog.Write(ex);
                        notfound = true;
                    }
                }
#endif
                sb.AppendLine(entry);
            }
            Debug.Assert(!notfound);
            // cannot cache all images as we're limited to 5mb offline cache
            /*
            var images = db.images.Where(x => x.subdomain == subdomainid.Value);
            var sizes = Enum.GetValues(typeof (Imgsize));
            foreach (var image in images)
            {
                foreach (Imgsize size in sizes)
                {
                    try
                    {
                        var url = Img.by_size(image.url, size);
                        sb.AppendLine(url);
                    }
                    catch (Exception ex)
                    {
                        Syslog.Write(ex);
                    }
                }
            }
            */

            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetValidUntilExpires(false);
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.ContentType = "text/cache-manifest";
            
            return Content(sb.ToString());
        }

        /// <summary>
        /// for Google Gears
        /// </summary>
        /// <returns></returns>
        public ActionResult Gears()
        {
            var manifest = new Manifest("1");
            //manifest.LoadManifestEntries(URLS_TO_CACHE, subdomain);

            Response.Cache.SetMaxAge(new TimeSpan(0));
            Response.Cache.SetCacheability(HttpCacheability.Private);
            
            return Json(manifest, JsonRequestBehavior.AllowGet);
        }

        private readonly string[] URLS_TO_CACHE = new[]
                                     {
                                         "/dashboard/account",
                                         "/dashboard/category/add",
                                         "/dashboard/category/addsub",
                                         "/dashboard/contacts",
                                         "/dashboard/contacts/add",
                                         "/Error",
                                         "/Error/NotFound",
                                         "/dashboard",
                                         "/dashboard/inventory",
                                         "/dashboard/invoices/add",
                                         "/dashboard/transactions",
                                         "/dashboard/orders/add",
                                         "/dashboard/payment/history",
                                         "/dashboard/product/add",
                                         "/dashboard/product/edit",
                                         "/dashboard/stockUnit/add",
                                         "/css/inside",
                                         "/Content/css/print.css",
                                         "/Scripts/jquery-1.4.2.min.js",
                                         "/Scripts/navigation/adminnav.js",
                                         GeneralConstants.DEBUG?"/Scripts/ajaxUpload.js":"/Scripts/ajaxUpload.min.js",
                                         GeneralConstants.DEBUG?"/Scripts/core.js":"/Scripts/core.min.js",
                                         GeneralConstants.DEBUG?"/Scripts/extend.js":"/Scripts/extend.min.js",
                                         GeneralConstants.DEBUG?"/Scripts/jqueryui.js":"/Scripts/jqueryui.min.js",
                                         "/Content/img/thumbs_none.png",
                                         "/Content/img/thumbs_none_medium.gif",
                                         "/Content/img/arrow_right.png",
                                         "/Content/img/button_add.png",
                                         "/Content/img/button50px.png",
                                         "/Content/img/date.png",
                                         "/Content/img/hover_delete.png",
                                         "/Content/img/divider-side.gif",
                                         "/Content/img/divider-side-top.gif",
                                         "/Content/img/divider-side-bottom.gif",
                                         "/Content/img/hover_edit.png",
                                         "/Content/img/favicon.png",
                                         "/Content/img/header_gradient.gif",
                                         "/Content/img/home.png",
                                         "/Content/img/loading.gif",
                                         "/Content/img/loading_blue.gif",
                                         "/Content/img/orb_tick_grey.png",
                                         "/Content/img/plus_circle.png",
                                         "/Content/img/plus_grey.png",
                                         "/Content/img/plus_square.png",
                                         "/Content/img/power.png",
                                         "/Content/img/required.png",
                                         "/Content/img/sel.gif",
                                         "/Content/img/subnav_bg.png",
                                         "/Content/img/small_newphoto.png",
                                         "/Content/img/social/icons/blogger_16.png",
                                         "/Content/img/social/icons/facebook_16.png",
                                         "/Content/img/social/icons/google_16.png",
                                         "/Content/img/thumbs_none.png",
                                         "/Content/img/tradelr.png",
                                         "/Content/img/hover_delete.png",
                                         "/Content/img/bullets/customize.png",
                                         "/Content/img/bullets/mail.png",
                                         "/Content/img/bullets/password.png",
                                         "/Content/img/bullets/server.png",
                                         "/Content/img/google/analytics.png",
                                         "/Content/img/google/adsense.png",
                                         "/Content/img/google/marker.png",
                                         "/Content/img/headings/heading_add.png",
                                         "/Content/img/headings/heading_add_med.png",
                                         "/Content/img/headings/heading_inventory.png",
                                         "/Content/img/headings/heading_setup.png",
                                         "/Content/img/headings/heading_terms.png",
                                         "/Content/images/ui-icons_0073ea_256x240.png",
                                         "/Content/images/ui-icons_ff6600_256x240.png",
                                         "/Content/img/loader/prettyLoader.png",
                                         "/Content/img/nav/arrow_right.gif",
                                         "/Content/img/nav/button_begins.gif",
                                         "/Content/img/nav/button_prvs.gif",
                                         "/Content/img/nav/button_nexts.gif",
                                         "/Content/img/nav/button_ends.gif",
                                         "/Content/img/nav/arrow_right.gif",
                                         "/Content/img/nav/arrow_down.gif",
                                         "/Content/img/payments/paypal.jpg",
                                         "/Content/img/payments/googleCheckout.png"
                                     };
    }
}
