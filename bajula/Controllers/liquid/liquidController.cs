using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Svg;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.ActionResults;
using tradelr.Library;
using tradelr.Library.Caching;
using clearpixels.Logging;
using tradelr.Models.liquid;
using tradelr.Models.liquid.models;
using tradelr.Models.mobile;

namespace tradelr.Controllers.liquid
{
    [HandleMobileBrowser]
    public class liquidController : Controller
    {
        // handles liquid template assets
        public ActionResult Handler(string filename, string domainpath)
        {
            string uniqueid;
            if (domainpath == "facebook")
            {
                uniqueid = domainpath;
            }
            else
            {
                var pathparts = domainpath.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                uniqueid = pathparts[0];
            }

            // setup physical path first
            string liquidPhysicalPath;
            bool isSVG = false;
            if (Request.RawUrl.IndexOf(".svg.png") != -1)
            {
                liquidPhysicalPath = Request.PhysicalPath.Replace(".png", ".liquid");
                isSVG = true;
            }
            else
            {
                liquidPhysicalPath = Request.PhysicalPath + ".liquid";
            }

            // cachekey = filename
            dynamic result;
            var cachekey = ThemeHandler.GetCacheKey(uniqueid, filename, (bool)Session[BrowserCapability.IsMobileSession]);
            if (!CacheHelper.Instance.TryGetCache(CacheItemType.liquid_assets, cachekey, out result))
            {
                if (System.IO.File.Exists(liquidPhysicalPath))
                {
                    LiquidTemplateBase template;

                    var accountHostname = Request.Headers["Host"];
                    MASTERsubdomain sd;
                    using (var db = new tradelrDataContext())
                    {
#if DEBUG
                        if (accountHostname.EndsWith("localhost"))
#else
                        if (accountHostname.EndsWith("tradelr.com"))
#endif
                        {
                            ////////////// handles case for subdomains
                            string[] host = accountHostname.Split('.');
                            // not on a subdomain

                            sd = db.GetSubDomain(host[0]);
                        }
                        else
                        {
                            ////////////////// handles case for custom subdomains
                            sd = db.GetCustomHostname(accountHostname);
                        }

                        template = new LiquidTemplateBase(sd,
                            (bool)Session[BrowserCapability.IsMobileSession]);

                        var parsed_string = template.ReadTemplateFile(liquidPhysicalPath);

                        template.InitContentTemplate(parsed_string);
                        template.AddParameters("shop", sd.ToLiquidModel());
                    }
                    
                    var dirIndex = liquidPhysicalPath.LastIndexOf("\\");

                    // handle not found
                    var config_file = liquidPhysicalPath.Substring(0, dirIndex).Replace("assets", "config\\settings_data.json");
                    if (System.IO.File.Exists(config_file))
                    {
                        var current = template.ReadThemeSettings(config_file);
                        if (current != null)
                        {
                            template.AddParameters("settings", current);
                        }
                        
                        if (isSVG)
                        {
                            // convert to png
                            using (var stream = template.RenderBasicToStreamNoHeader())
                            {
                                var svg = SvgDocument.Open(stream);
                                result = svg.Draw();
                            }
                        }
                        else
                        {
                            result = template.RenderBasicNoHeader();
                        }

                        CacheHelper.Instance.Insert(CacheItemType.liquid_assets, cachekey, result);
                        if (uniqueid != "facebook")
                        {
                            CacheHelper.Instance.add_dependency(DependencyType.liquid_assets, uniqueid, CacheItemType.liquid_assets, cachekey);
                        }
                    }
                }
            }

            if (Request.RawUrl.IndexOf(".css") != -1)
            {
                Response.ContentType = "text/css";
                return Content(result);
            }

            if (Request.RawUrl.IndexOf(".js") != -1)
            {
                Response.ContentType = "application/x-javascript";
                return Content(result);
            }

            if (isSVG)
            {
                return new SvgToPngActionResult(result);
            }

            Syslog.Write(string.Format("Unknown filetype: {0}", Request.RawUrl));
            return new EmptyResult();
        }
    }
}
