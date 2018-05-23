using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using DotLiquid;
using FiftyOne.Foundation.Mobile.Detection;
using Lucene.Net.Index;
using tradelr.Common.Library;
using tradelr.Controllers.error;
using tradelr.Controllers.liquid;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.Libraries.DomainRoute;
using tradelr.Libraries.scheduler;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.liquid.extend;

namespace tradelr
{
    public class MvcApplication : HttpApplication
    {
        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon(\.ico)?" });
            routes.IgnoreRoute("{*wpaddat}", new { wpaddat = @"(.*/)?wpad.dat(/.*)?" });
            routes.IgnoreRoute("{*php}", new { php = @".*\.php(/.*)?" });

            routes.Add("LoginDomainRoute", new DomainRoute(
                                               "secure." + GeneralConstants.SUBDOMAIN_HOST_NOPORT,
                                               // Domain with parameters
                                               "", // URL with parameters
                                               new { controller = "login", action = "find" } // Parameter defaults
                                               ));

            routes.Add("FindAccountDomainRoute", new DomainRoute(
                                                     "secure." + GeneralConstants.SUBDOMAIN_HOST_NOPORT,
                                                     // Domain with parameters
                                                     "login/find", // URL with parameters
                                                     new {controller = "login", action = "find"} // Parameter defaults
                                                     ));

            routes.Add("SignUpRoute", new DomainRoute(
                                          "secure." + GeneralConstants.SUBDOMAIN_HOST_NOPORT, // Domain with parameters
                                          "register/{action}/{id}", // URL with parameters
                                          new {controller = "Register", action = "Index", id = ""} // Parameter defaults
                                          ));

            routes.MapRoute(
                "LiquidProduct", // Route name
                "products/{id}/{title}", // URL with parameters
                new { controller = "Products", action = "Single", title = UrlParameter.Optional }, // Parameter defaults
                new {id = @"\d+"},
                new[] { "tradelr.Controllers.liquid" }
                );

            // mvc3 dun like 2 UrlParameter.Optional in a row
            /*
            routes.MapRoute(
                "LiquidProductFix", // Route name
                "products", // URL with parameters
                new { controller = "Products", action = "Index"}, // Parameter defaults
                new[] { "tradelr.Controllers.liquid" }
                );*/

#region liquid json results
                routes.MapRoute(
                    "LiquidProduct JSON", // Route name
                    "products/{id}.json", // URL with parameters
                    new { controller = "Products", action = "Single", isJson = true }
                );

                routes.MapRoute(
                    "CartAdd JSON", // Route name
                    "cart/add.json", // URL with parameters
                    new { controller = "Cart", action = "Add", isJson = true },
                    new[] { "tradelr.Controllers.liquid" }
                );

                routes.MapRoute(
                        "Cart JSON", // Route name
                        "cart.json", // URL with parameters
                        new { controller = "Cart", action = "GetJson", isJson = true },
                        new[] { "tradelr.Controllers.liquid" }
                    );

                routes.MapRoute(
                        "CartChange JSON", // Route name
                        "cart/change.json", // URL with parameters
                        new { controller = "Cart", action = "Change", isJson = true },
                        new[] { "tradelr.Controllers.liquid" }
                    );
#endregion

            routes.MapRoute(
                "Photos", // Route name
                "photos/upload/{type}/{id}", // URL with parameters
                new {controller = "photos", action = "Upload", id = "", type = ""} // Parameter defaults
                );

            routes.MapRoute(
                "PhotosDelete", // Route name
                "photos/delete/{type}/{id}", // URL with parameters
                new {controller = "photos", action = "Delete", id = "", type = ""} // Parameter defaults
                );

            routes.MapRoute(
                "Browse", // Route name
                "browse/{id}", // URL with parameters
                new {controller = "browse", action = "Index", id = ""} // Parameter defaults
                );

            routes.MapRoute(
                "Liquid_Pages", // Route name
                "pages/{id}", // URL with parameters
                new {controller = "Pages", action = "Index", id = ""},
                new[] {"tradelr.Controllers.liquid"}
                );

            routes.MapRoute(
                "Liquid_collections_products", // Route name
                "collections/{collectionid}/products/{productid}/{producttitle}", // URL with parameters
                new { controller = "collections", action = "Products", producttitle = UrlParameter.Optional },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Liquid_collections", // Route name
                "collections/{id}/{tags}", // URL with parameters
                new { controller = "collections", action = "Index", id = UrlParameter.Optional, tags = UrlParameter.Optional },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Blog_Feed", // Route name
                "blogs/{blogid}.atom", // URL with parameters
                new {controller = "Blogs", action = "Atom"},
                new[] {"tradelr.Controllers.liquid"}
                );

            routes.MapRoute(
                "Liquid_Articles", // Route name
                "blogs/{blogid}/{aid}/{action}", // URL with parameters
                new {controller = "Articles", action = "Index"},
                new[] {"tradelr.Controllers.liquid"}
                );

            routes.MapRoute(
                "Liquid_Blogs", // Route name
                "blogs/{id}", // URL with parameters
                new {controller = "Blogs", action = "Index", id = ""},
                new[] {"tradelr.Controllers.liquid"}
                );

            routes.MapRoute(
                "Liquid_Cart", // Route name
                "cart/{action}/{id}", // URL with parameters
                new {controller = "Cart", action = "Index", id = ""},
                new[] {"tradelr.Controllers.liquid"}
                );

            routes.MapRoute(
                "Liquid_Assets", // Route name
                "Uploads/files/{domainpath}/theme/assets/{filename}", // URL with parameters
                new { controller = "Liquid", action = "Handler" },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Liquid_Assets_Mobile", // Route name
                "Uploads/files/{domainpath}/mobile_theme/assets/{filename}", // URL with parameters
                new { controller = "Liquid", action = "Handler" },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Digital checkout", // Route name
                "c/{id}", // URL with parameters
                new { controller = "c", action = "Index" }
                );

            routes.MapRoute(
                "Digital downloads", // Route name
                "d/{id}", // URL with parameters
                new { controller = "d", action = "Index" }
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = UrlParameter.Optional },
                GetSubNamespace("tradelr.Controllers").ToArray()
                ).DataTokens["UseNamespaceFallback"] = false;

            // it will never get here until we add constraints to the default route
            routes.MapRoute("NotFound", "{*url}", new { area = "", controller = "Error", action = "NotFound", id = UrlParameter.Optional });

        }

        protected void Application_Start()
        {
            //// handle areas
            AreaRegistration.RegisterAllAreas();
            RegisterRoutes(RouteTable.Routes);
            /*
             FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
             RouteConfig.RegisterRoutes(RouteTable.Routes);
             BundleConfig.RegisterBundles(BundleTable.Bundles);
             */

            // Enable the mobile detection provider.
            HttpCapabilitiesBase.BrowserCapabilitiesProvider = new MobileCapabilitiesProvider();
#if DEBUG
            // uncomment this for route debugging
            //RouteDebug.RouteDebugger.RewriteRoutesForTesting(RouteTable.Routes);
#endif
            var sessionids = new HashSet<string>();

            Application.Add("sessionidList", sessionids);

            MvcHandler.DisableMvcResponseHeader = true; 
            // start cache-based scheduler
            CacheScheduler.Instance.RegisterCacheEntry();

            // no support for this yet, need to watch SVC16

            // register Liquid custom tags
            Template.RegisterTag<PaginateBlock>("paginate");
            Template.RegisterTag<UserForm>("form");

            // add additional viewengine provider
            System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(new AssemblyResourceProvider());
        }

        protected void Session_Start()
        {
            if (!Request.Browser.Crawler && HttpContext.Current.Request.Url.ToString() != GeneralConstants.HTTP_CACHEURL)
            {
                Application.Lock();
                var idlist = Application["sessionidList"] as HashSet<string>;
                if (idlist != null)
                {
                    idlist.Add(Session.SessionID);
                    Application["sessionidList"] = idlist;
                }
                
                Application.UnLock();
            }
        }

        protected void Session_End()
        {
            Application.Lock();
            var idlist = Application["sessionidList"] as HashSet<string>;
            if (idlist != null)
            {
                idlist.Remove(Session.SessionID);
                Application["sessionidList"] = idlist;
            }
            Application.UnLock();
        }

        protected void Application_BeginRequest()
        {
            if (HttpContext.Current.Request.Url.ToString() == GeneralConstants.HTTP_CACHEURL)
            {
                // Add the item in cache and when succesful, do the work.
                CacheScheduler.Instance.RegisterCacheEntry();
            }
        }

        /*
        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode == 404)
            {
                Response.Clear();

                var rd = new RouteData();
                rd.DataTokens["area"] = ""; // In case controller is in another area
                rd.Values["controller"] = "error";
                rd.Values["action"] = "notfound";

                IController c = new errorController();
                c.Execute(new RequestContext(new HttpContextWrapper(Context), rd));
            }
        }
        */

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;

            var routeData = new RouteData();
            IController errorController = new errorController();
            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 401:
                        routeData.Values["controller"] = "Error";
                        routeData.Values["action"] = "NoAuth";
                        break;
                    case 404:
                        var area = Request.RequestContext.RouteData.DataTokens["area"] as string;
                        string[] host = Request.Headers["Host"].Split('.');
                        string hostSegment = "";
                        if (string.IsNullOrEmpty(area) && Utility.IsOnSubdomain(host, out hostSegment))
                        {
                            errorController = new liquidErrorController();
                            routeData.Values["controller"] = "liquidError";
                            routeData.Values["action"] = "NotFound";
                        }
                        else
                        {
                            routeData.Values["controller"] = "Error";
                            routeData.Values["action"] = "NotFound";
                            routeData.Values["url"] = Request.Url.OriginalString;
                        }
                        break;
                    default:
                        routeData.Values["controller"] = "Error";
                        routeData.Values["action"] = "Index";
                        break;
                }
            }

            // Clear the error on server.
            Server.ClearError();

            // Avoid IIS7 getting in the middle
            Response.TrySkipIisCustomErrors = true;

            // Call target Controller and pass the routeData.
            errorController.Execute(new RequestContext(
                 new HttpContextWrapper(Context), routeData));
        }

        private static IEnumerable<string> GetSubNamespace(string parentNamespace)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var groups = assembly.GetTypes().Where(t => t.IsClass).GroupBy(t => t.Namespace);

            return groups.Where(entry => !string.IsNullOrEmpty(entry.Key) &&  entry.Key.StartsWith(parentNamespace))
                .Select(entry => entry.Key);
        }
    }
}