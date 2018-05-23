using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.google;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.users;

namespace tradelr.Controllers.google
{
    /// <summary>
    /// this is special so we can handle no-SSL google map connection
    /// </summary>
    
    //[ElmahHandleError]
    public class googleController : baseController
    {

        [HttpGet]
        [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
        public ActionResult map(long id)
        {
            GoogleMapData viewdata;
            var org = MASTERdomain.organisations.Single(x => x.id == id);
            viewdata = org.ToGoogleMap();

            var serializer = new JavaScriptSerializer();
            var countryData = serializer.Serialize(Country.Values.Values);
            if (viewdata.country.HasValue)
            {
                viewdata.countryList = Country.GetCountries().ToSelectList(viewdata.country.Value);
            }
            else
            {
                viewdata.countryList = Country.GetCountries().ToSelectList("", "None", "");
            }
            viewdata.countryData = countryData;

            return View(viewdata);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
        public ActionResult map(long id, string latitude, string longtitude, string mapzoom)
        {
            var org = MASTERdomain.organisations.Single(x => x.id == id);

            if (!string.IsNullOrEmpty(latitude))
            {
                org.latitude = decimal.Parse(latitude);
                org.longtitude = decimal.Parse(longtitude);
                org.zoom = int.Parse(mapzoom);
            }
            repository.Save();

            return Json("Location saved".ToJsonOKMessage());    
        }

        public ActionResult Sitemap()
        {
            if (!subdomainid.HasValue)
            {
                return RedirectToAction("NotFound", "Error");
            }
            Response.ContentType = "text/xml";
            var ms = new MemoryStream();
            var objX = new XmlTextWriter(ms, Encoding.UTF8);
            objX.WriteStartDocument();
            objX.WriteStartElement("urlset");
            objX.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

            objX.WriteStartElement("url");
            objX.WriteElementString("loc", accountHostname.ToDomainUrl("", true));
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.8");
            objX.WriteEndElement();

            if (MASTERdomain.IsStoreEnabled())
            {
                var products = repository.GetStoreProducts(subdomainid.Value);

                foreach (var product in products)
                {
                    string url = product.ToLiquidProductUrl();
                    objX.WriteStartElement("url");
                    objX.WriteElementString("loc", accountHostname.ToDomainUrl(url, true));
                    if (product.updated.HasValue)
                    {
                        objX.WriteElementString("lastmod", product.updated.Value.ToString("yyyy-MM-dd"));
                    }
                    objX.WriteElementString("changefreq", "daily");
                    objX.WriteElementString("priority", "0.7");
                    objX.WriteEndElement();
                }    
            }
            objX.WriteEndElement(); // urlset
            objX.WriteEndDocument();
            objX.Flush();
            objX.Close();
            return Content(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}