using System;
using System.Net;
using Google.GData.Client;
using Google.GData.WebmasterTools;
using clearpixels.Logging;
using HttpUtility = System.Web.HttpUtility;

namespace tradelr.Models.google
{
    public class Webmaster
    {
        private const string SiteQueryUrl = "https://www.google.com/webmasters/tools/feeds/sites/";
        private const string sitemapQueryUrl = "https://www.google.com/webmasters/tools/feeds/{0}/sitemaps/";
        WebmasterToolsService service { get; set; }
        public Webmaster()
        {
            service = new WebmasterToolsService("tradelr");
            service.setUserCredentials("tradelr.com@gmail.com", "tuaki1976");
        }

        private static string CreateSiteID(string url)
        {
            return HttpUtility.UrlEncode(url).Replace(".","%2e").Replace("_","%5f");
        }

        private SitesEntry GetSite(string url)
        {
            var siteid = CreateSiteID(url);
            var query = new SitesQuery(string.Concat(SiteQueryUrl,siteid));
            SitesEntry entry = null;
            try
            {
                var result = service.Query(query);
                if (result.Entries.Count == 0)
                {
                    return null;
                }
                entry = (SitesEntry)result.Entries[0];
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }

            return entry;
        }

        /// <summary>
        /// not needed but just leave it here for reference
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetVerificationMetaTag(string url)
        {
            var site = GetSite(url);
            if (site == null)
            {
                return null;
            }
            return site.VerificationMethod.Value;
        }

        public SitesEntry AddSite(string url)
        {
            var entry = new AtomEntry();
            var content = new AtomContent();
            content.Src = new AtomUri(url);
            entry.Content = content;
            entry.Content.Type = "";
            try
            {
                return service.Insert(new Uri(SiteQueryUrl), entry) as SitesEntry;
            }
            catch (Exception ex)
            {
                var status = ((WebException) (ex.InnerException)).Status;
                if (status != WebExceptionStatus.ProtocolError)
                {
                    Syslog.Write(ex);
                }
            }
            return null;
        }

        public void AddSitemap(string url, string sitemapurl)
        {
            var sitemap = new SitemapsEntry();
            sitemap.Id = new AtomId(sitemapurl);
            sitemap.SitemapType = "WEB";
            var category = new AtomCategory("http://schemas.google.com/webmasters/tools/2007#sitemap-regular",
                                            "http://schemas.google.com/g/2005#kind");
            sitemap.Categories.Add(category);
            var siteid = CreateSiteID(url);
            var queryurl = string.Format(sitemapQueryUrl, siteid);
            try
            {
                service.Insert(new Uri(queryurl), sitemap);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
        }

        public SitesEntry VerifySite(string url, SitesEntry entry = null)
        {
            if (entry == null)
            {
                entry = GetSite(url);
            }

            VerificationMethod method = new VerificationMethod("metatag","true");
            var siteid = CreateSiteID(url);
            entry.VerificationMethod = method;
            var category = new AtomCategory("http://schemas.google.com/webmasters/tools/2007#site-info",
                                            "http://schemas.google.com/g/2005#kind");
            entry.Categories.Add(category);
            entry.Id = new AtomId(string.Concat(SiteQueryUrl,siteid));
            entry.Content.Type = "";
            try
            {
                return service.Update(entry);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return null;
        }
    }
}