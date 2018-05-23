
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using tradelr.Common;

namespace tradelr.Controllers
{
    public class sitemapController : Controller
    {
        // returns main sitemap
        public ActionResult Index()
        {
            Response.ContentType = "text/xml";
            var ms = new MemoryStream();
            XmlTextWriter objX = new XmlTextWriter(ms, Encoding.UTF8);
            objX.WriteStartDocument();
            objX.WriteStartElement("urlset");
            objX.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");

            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "1.0");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/contacts");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/transactions");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/store");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/engage");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/web");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            /*
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/tour/security");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            objX.WriteStartElement("url");
            objX.WriteElementString("loc", "http://www.tradelr.com/pricing");
            objX.WriteElementString("lastmod", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"));
            objX.WriteElementString("changefreq", "daily");
            objX.WriteElementString("priority", "0.9");
            objX.WriteEndElement();
            */
            objX.WriteEndElement(); // urlset
            objX.WriteEndDocument();
            objX.Flush();
            objX.Close();
            return Content(Encoding.UTF8.GetString(ms.ToArray()));
        }

    }
}
