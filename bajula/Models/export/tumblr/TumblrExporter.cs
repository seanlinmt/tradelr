using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using tradelr.Common.Models.photos;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using clearpixels.Logging;
using tradelr.Models.products;
using tumblr;

namespace tradelr.Models.export.tumblr
{
    public class TumblrExporter : ExportItem
    {
        private readonly global::tumblr.tumblr.Text post;
        private readonly global::tumblr.tumblr tumblr;

        private TumblrExporter(string hostname): base(hostname)
        {
            post = new global::tumblr.tumblr.Text();
            tumblr = new global::tumblr.tumblr();
        }

        public TumblrExporter(string email, string password, string hostname, long sessionid, long subdomainid, string postid = "") 
            : this(hostname)
        {
            // need to decrypt password first
            var cryptor = new AESCrypt();
            password = cryptor.Decrypt(password, subdomainid.ToString());
            ownerid = sessionid;
            post.Email = email;
            post.Password = password;
            post.Id = postid;
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
                    sb.AppendFormat("<p>{0}{1}</p>", Currency.symbol, SpecialPrice.Value.ToString("n" + Currency.decimalCount));
                }
                else
                {
                    sb.AppendFormat("<p>{0}{1}</p>", Currency.symbol, SellingPrice.Value.ToString("n" + Currency.decimalCount));
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
                post.Tags = string.Join(",", p.tags).Replace("_", " ");
            }
            post.DateOfPost = DateTime.UtcNow;
        }

        // this will edit the post if an id is set
        public string PostEntry()
        {
            Debug.Assert(post != null);
            Status status = tumblr.postText(post);
            if (status.Code != 201)
            {
                Syslog.Write(status.Msg);
            }
            return status.Id;
        }

        public void UpdateBlogEntry()
        {
            Debug.Assert(post != null);
            var status = tumblr.postText(post);
            if (status.Code != 201)
            {
                Syslog.Write(status.Msg);
            }
        }

        public void UpdateID(string postid)
        {
            Debug.Assert(ProductId != 0);

            using (var repository = new TradelrRepository())
            {
                var t = repository.GetProduct(ProductId).tumblrPosts;
                if (t == null)
                {
                    t = new tumblrPost();
                    repository.AddTumblrPost(t);
                }
                t.productid = ProductId;
                t.postid = postid;
                repository.Save();
            }
        }

        public void DeletePost()
        {
            tumblr.deleteText(post);
        }
    }

    public class TumblrExporterWorker
    {
        private readonly TumblrExporter item;

        public TumblrExporterWorker(TumblrExporter item)
        {
            this.item = item;
        }

        public void Delete()
        {
            item.DeletePost();
        }

        public void Post()
        {
            var postid = item.PostEntry();
            if (!string.IsNullOrEmpty(postid))
            {
                item.UpdateID(postid);
            }
        }
    }
}