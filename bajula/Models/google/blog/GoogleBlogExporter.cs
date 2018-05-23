using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Google.GData.Client;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.products;
using tradelr.Models.users;
using HttpUtility = System.Web.HttpUtility;

namespace tradelr.Models.google.blog
{
    public class GoogleBlogExporter
    {
        public AtomEntry blogEntry { get; set;}
        public string hostName { get; set; }
        private Service service { get; set;}
        private ITradelrRepository repository { get; set; }
        private long ownerid { get; set; }
        private long productid { get; set; }
        private string productname { get; set; }
        private string producturl { get; set; }
        private long subdomainid { get; set; }

        public GoogleBlogExporter(string sessionKey, string hostName, ITradelrRepository repository, long sesssionid)
        {
            var authFactory = new GAuthSubRequestFactory("blogger", "tradelr") {Token = sessionKey};
            service = new Service(authFactory.ApplicationName) {RequestFactory = authFactory};
            this.hostName = hostName;
            this.repository = repository;
            this.ownerid = sesssionid;

            blogEntry = new AtomEntry();
        }

        public void AddPhotos(IEnumerable<Photo> productPhotos)
        {
            Debug.Assert(blogEntry != null);
            var regex = new Regex("^<div.+?>(.+?)</div>$");
            var match = regex.Match(blogEntry.Content.Content);
            string content = "";
            if (match.Success)
            {
                content = match.Groups[1].Value;
            }
            StringBuilder sb= new StringBuilder();
            sb.Append("<div xmlns='http://www.w3.org/1999/xhtml'>");
            
            foreach (var photo in productPhotos)
            {
                sb.Append("<p>");
                sb.Append(hostName.ToDomainUrl(photo.url, true).ToHtmlImage());
                sb.Append("</p>");
            }
            sb.Append("<p>");
            sb.Append(content);
            sb.Append("</p>");
            sb.Append("</div>");
            
            blogEntry.Content.Content = sb.ToString();
        }

        public void FillBlogEntry(product p)
        {
            // init values for activity message
            productid = p.id;
            productname = p.title;
            subdomainid = p.subdomainid;
            producturl = p.ToProductUrl();

            blogEntry.Title.Text = productname;
            blogEntry.Content = new AtomContent();
            var productpage =
                hostName.ToDomainUrl(p.ToProductUrl());
            StringBuilder sb = new StringBuilder();
            sb.Append("<div xmlns='http://www.w3.org/1999/xhtml'>");
            sb.AppendFormat("<h3><a target='_blank' href='{0}'>{1}</a></h3>", productpage,
                            HttpUtility.HtmlEncode(productname));

            if (p.sellingPrice.HasValue)
            {
                var currency = p.MASTERsubdomain.currency.ToCurrency();

                var sellingPrice = p.tax.HasValue
                                           ? (p.sellingPrice.Value * (p.tax.Value / 100 + 1)).ToString("n" +
                                                                                                   currency.
                                                                                                       decimalCount)
                                           : p.sellingPrice.Value.ToString("n" + currency.decimalCount);

                if (p.specialPrice.HasValue)
                {
                    // if has special price then original (strike-through) + special price
                    var specialPrice = p.tax.HasValue
                                           ? (p.specialPrice.Value*(p.tax.Value/100 + 1)).ToString("n" +
                                                                                                   currency.
                                                                                                       decimalCount)
                                           : p.specialPrice.Value.ToString("n" + currency.decimalCount);
                    sb.AppendFormat(
                        "<h3><span style='text-decoration:line-through;'>{2}{0}</span><span style='margin-left:10px;'>{2}{1}</span></h3>",
                        sellingPrice, specialPrice, currency.symbol);
                }
                else
                {
                    sb.AppendFormat("<h3><span>{1}{0}</span></h3>", sellingPrice, currency.symbol);
                }
            }

            sb.Append("<p>");
            sb.Append(HttpUtility.HtmlEncode(p.details).ToHtmlBreak());
            sb.Append("</p>");
            sb.Append("<p><a target='_blank' href='");
            sb.Append(productpage);
            sb.Append("'>Go to product page</a></p>");
            sb.Append("</div>");

            blogEntry.Content.Content =  sb.ToString();
            blogEntry.Content.Type = "xhtml";
            blogEntry.Authors.Add(new AtomPerson());
            blogEntry.Authors[0].Name = p.MASTERsubdomain.organisation.users.First().ToFullName();
            blogEntry.Authors[0].Email = p.MASTERsubdomain.organisation.users.First().email;
        }

        public void GetBlogEntry(product p, string postUrl)
        {
            bool hasError = false;
            try
            {
                blogEntry = service.Get(postUrl.ToAtomUri());
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        
                        Syslog.Write(string.Concat("GBlog Get entry: ", err, " ", hostName, " "));
                    }
                }
                hasError = true;
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("GBlog Get entry: ", ex.Message, " ", hostName, " "));
            }

            if (hasError)
            {
                repository.AddActivity(ownerid,
                                   new ActivityMessage(productid, ownerid,
                                               ActivityMessageType.AUTOPOST_GOOGLEBLOG_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                repository.Save();
            }
            
            FillBlogEntry(p);
        }

        public string PostBlogEntry(Uri destinationBlog)
        {
            Debug.Assert(blogEntry != null);
            AtomEntry created = null;
            bool hasError = false;
            try
            {
                created = service.Insert(destinationBlog, blogEntry);
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        hasError = true;
                        Syslog.Write(string.Concat("GBlog Post: ", err, " ", hostName, " ", destinationBlog.AbsoluteUri));
                    }
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("GBlog Post: ", ex.Message, " ", hostName, " ", destinationBlog.AbsoluteUri));
            }
            if (hasError)
            {
                repository.AddActivity(ownerid,
                                   new ActivityMessage(productid, ownerid,
                                               ActivityMessageType.AUTOPOST_GOOGLEBLOG_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>",producturl,productname)),
                                               subdomainid);
                repository.Save();
            }
            return created == null ? null : created.Id.AbsoluteUri;
        }

        public void UpdateBlogEntry()
        {
            Debug.Assert(blogEntry != null);
            bool hasError = false;
            try
            {
                service.Update(blogEntry);
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        Syslog.Write(string.Concat("GBlog Update: ", err, " ", hostName, " ", blogEntry.FeedUri));
                    }
                }
                hasError = true;
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("GBlog Update: ", ex.Message, " ", hostName, " ", blogEntry.FeedUri));
                        
            }
            if (hasError)
            {
                repository.AddActivity(ownerid,
                                   new ActivityMessage(productid, ownerid,
                                               ActivityMessageType.AUTOPOST_GOOGLEBLOG_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                repository.Save();
            }
        }
    }

    public static class GoogleBlogExporterHelper
    {
        public static string ToAtomUri(this string id)
        {
            var regex = new Regex("blog-(.+?)\\.post-(.+?)$");
            var match = regex.Match(id);
            if (!match.Success)
            {
                return "";
            }
            var blogid = match.Groups[1].Value;
            var postid = match.Groups[2].Value;
            return string.Concat("http://www.blogger.com/feeds/", blogid, "/posts/default/", postid);
        }

        public static IEnumerable<GoogleBlogData> ToModel(this IEnumerable<googleBlog> values)
        {
            foreach (var value in values)
            {
                yield return new GoogleBlogData
                                 {
                                     name = value.title,
                                     blogHref = value.blogHref
                                 };
            }
        }
    }
}