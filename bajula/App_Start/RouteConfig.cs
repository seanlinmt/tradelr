using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.Libraries.DomainRoute;
using tradelr.Library.Constants;

namespace tradelr.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon(\.ico)?" });
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
                                                     new { controller = "login", action = "find" } // Parameter defaults
                                                     ));

            routes.Add("SignUpRoute", new DomainRoute(
                                          "secure." + GeneralConstants.SUBDOMAIN_HOST_NOPORT, // Domain with parameters
                                          "register/{action}/{id}", // URL with parameters
                                          new { controller = "Register", action = "Index", id = "" } // Parameter defaults
                                          ));

            routes.MapRoute(
                "LiquidProduct", // Route name
                "products/{id}/{title}", // URL with parameters
                new { controller = "Products", action = "Single", title = UrlParameter.Optional }, // Parameter defaults
                new { id = @"\d+" },
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
                new { controller = "photos", action = "Upload", id = "", type = "" } // Parameter defaults
                );

            routes.MapRoute(
                "PhotosDelete", // Route name
                "photos/delete/{type}/{id}", // URL with parameters
                new { controller = "photos", action = "Delete", id = "", type = "" } // Parameter defaults
                );

            routes.MapRoute(
                "Browse", // Route name
                "browse/{id}", // URL with parameters
                new { controller = "browse", action = "Index", id = "" } // Parameter defaults
                );

            routes.MapRoute(
                "Liquid_Pages", // Route name
                "pages/{id}", // URL with parameters
                new { controller = "Pages", action = "Index", id = "" },
                new[] { "tradelr.Controllers.liquid" }
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
                new { controller = "Blogs", action = "Atom" },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Liquid_Articles", // Route name
                "blogs/{blogid}/{aid}/{action}", // URL with parameters
                new { controller = "Articles", action = "Index" },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Liquid_Blogs", // Route name
                "blogs/{id}", // URL with parameters
                new { controller = "Blogs", action = "Index", id = "" },
                new[] { "tradelr.Controllers.liquid" }
                );

            routes.MapRoute(
                "Liquid_Cart", // Route name
                "cart/{action}/{id}", // URL with parameters
                new { controller = "Cart", action = "Index", id = "" },
                new[] { "tradelr.Controllers.liquid" }
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
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                GetSubNamespace("tradelr.Controllers").ToArray()
                ).DataTokens["UseNamespaceFallback"] = false;

            // it will never get here until we add constraints to the default route
            routes.MapRoute("NotFound", "{*url}", new { area = "", controller = "Error", action = "NotFound", id = UrlParameter.Optional });

        }


        private static IEnumerable<string> GetSubNamespace(string parentNamespace)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var groups = assembly.GetTypes().Where(t => t.IsClass).GroupBy(t => t.Namespace);

            return groups.Where(entry => !string.IsNullOrEmpty(entry.Key) && entry.Key.StartsWith(parentNamespace))
                .Select(entry => entry.Key);
        }
    }
}