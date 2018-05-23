using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using JoeBlogs;
using tradelr.Common.Models.photos;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.products;

namespace tradelr.Models.export.wordpress
{
    public class WordpressExporter : ExportItem
    {
        private Post post;
        private WordPressWrapper wrapper;

        private WordpressExporter(string hostname):base(hostname)
        {
            post = new Post();
        }

        public WordpressExporter(string hostname, long sessionid) 
            : this(hostname)
        {
            ownerid = sessionid;
        }

        public bool AddCredentials(wordpressSite site, long sdid)
        {
            if (site == null)
            {
                return false;
            }
            var cryptor = new AESCrypt();
            var decryptedpwd = cryptor.Decrypt(site.password, sdid.ToString());
            wrapper = new WordPressWrapper(site.url + "/xmlrpc.php", site.email, decryptedpwd);

            return true;
        }

        public void AddPhotos(IEnumerable<Photo> productPhotos)
        {
            Debug.Assert(post != null);
            var sb = new StringBuilder();
            
            foreach (var photo in productPhotos)
            {
                sb.Append("<p>");
                sb.Append(hostName.ToDomainUrl(photo.url, true).ToHtmlImage());
                sb.Append("</p>");
            }
            sb.Append(post.Body);
            
            post.Body = sb.ToString();
        }

        public void FillBlogEntry(product p)
        {
            base.InitValues(p, 1);

            post.Title = productname;

            var productpage = hostName.ToDomainUrl(p.ToProductUrl());
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<h3><a target='_blank' href='{0}'>{1}</a></h3>", productpage,
                            HttpUtility.HtmlEncode(productname));

            if (SellingPrice.HasValue)
            {
                if (SpecialPrice.HasValue)
                {
                    // if has special price then original (strike-through) + special price
                    sb.AppendFormat(
                        "<h3><span style='text-decoration:line-through;'>{2}{0}</span><span style='margin-left:10px;'>{2}{1}</span></h3>",
                        SellingPrice.Value.ToString("n" + Currency.decimalCount), SpecialPrice.Value.ToString("n" + Currency.decimalCount), Currency.symbol);
                }
                else
                {
                    sb.AppendFormat("<h3><span>{1}{0}</span></h3>", SellingPrice.Value, Currency.symbol);
                }
            }

            sb.Append("<p>");
            sb.Append(HttpUtility.HtmlEncode(Description).ToHtmlBreak());
            sb.Append("</p>");
            sb.Append("<p><a target='_blank' href='");
            sb.Append(productpage);
            sb.Append("'>Go to product page</a></p>");

            post.Body = sb.ToString();
            if (p.tags1 != null)
            {
                post.Tags = p.tags1.Select(x => x.name.Replace("_", " ")).ToArray();
            }
            post.DateCreated = DateTime.UtcNow;
        }

        public void GetBlogEntry(product p, int postid)
        {
            bool hasError = false;
            try
            {
                post = wrapper.GetPost(postid);
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();

                        Syslog.Write(string.Concat("Wordpress Get entry: ", err, " ", hostName, " "));
                    }
                }
                hasError = true;
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("Wordpress Get entry: ", ex.Message, " ", hostName, " "));
            }

            if (hasError)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(ownerid.Value,
                                   new ActivityMessage(ProductId, ownerid,
                                               ActivityMessageType.AUTOPOST_WORDPRESS_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                    repository.Save();
                }
            }

            FillBlogEntry(p);
        }

        public int? PostEntry()
        {
            Debug.Assert(post != null);
            bool hasError = false;
            int? postid = null;
            try
            {
#if DEBUG
                postid = wrapper.NewPost(post, false);
#else
                postid = wrapper.NewPost(post, true);
#endif
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
                        Syslog.Write(string.Concat("Wordpress Post: ", err, " ", hostName, " ", postid));
                    }
                }
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("Wordpress Post: ", ex.Message, " ", hostName, " ", postid));
            }
            if (hasError)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(ownerid.Value,
                                   new ActivityMessage(ProductId, ownerid,
                                               ActivityMessageType.AUTOPOST_WORDPRESS_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                    repository.Save();
                }
                
            }
            return postid;
        }

        public void UpdateBlogEntry()
        {
            Debug.Assert(post != null);
            bool hasError = false;
            try
            {
#if DEBUG
                wrapper.EditPost(post.PostID, post, false);
#else
                wrapper.EditPost(post.PostID, post, true);
#endif
            }
            catch (WebException ex)
            {
                var resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var err = sr.ReadToEnd();
                        Syslog.Write(string.Concat("Wordpress Blog Update: ", err, " ", hostName, " ", post.PostID));
                    }
                }
                hasError = true;
            }
            catch (Exception ex)
            {
                hasError = true;
                Syslog.Write(string.Concat("Wordpress Update: ", ex.Message, " ", hostName, " ", post.PostID));

            }
            if (hasError)
            {
                using (var repository = new TradelrRepository())
                {
                    repository.AddActivity(ownerid.Value,
                                   new ActivityMessage(ProductId, ownerid,
                                               ActivityMessageType.AUTOPOST_WORDPRESS_FAIL,
                                               string.Format("<a href='{0}'>{1}</a>", producturl, productname)),
                                               subdomainid);
                    repository.Save();
                }
            }
        }

        public void UpdateID(int postid)
        {
            Debug.Assert(ProductId != 0);
            using (var repository = new TradelrRepository())
            {
                var w = repository.GetProduct(ProductId).wordpressPosts;
                if (w == null)
                {
                    w = new wordpressPost();
                    repository.AddWordpressPost(w);
                }
                w.productid = ProductId;
                w.postid = postid;
                repository.Save();
            }
        }

        public void DeletePost(int postid)
        {
            wrapper.DeletePost(postid);
        }
    }

    public class WordpressExporterWorker
    {
        private readonly WordpressExporter item;

        public WordpressExporterWorker(WordpressExporter item)
        {
            this.item = item;
        }

        public void Delete(int postid)
        {
            item.DeletePost(postid);
        }

        public void Post()
        {
            var postid = item.PostEntry();
            if (postid.HasValue)
            {
                item.UpdateID(postid.Value);
            }
        }

        public void Update()
        {
            item.UpdateBlogEntry();
        }
    }
}