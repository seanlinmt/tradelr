using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Google.GData.Client;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Library;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.google;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class bloggerController : baseController
    {
        public ActionResult clear()
        {
            var sd = repository.GetSubDomains().Where(x => x.id == subdomainid.Value).Single();
            sd.bloggerSessionKey = "";
            if (sd.googleBlogs != null)
            {
                db.googleBlogs.DeleteAllOnSubmit(sd.googleBlogs);
            }
            repository.Save();
            return new EmptyResult();
        }
    
        public ActionResult haveToken()
        {
            if (!string.IsNullOrEmpty(MASTERdomain.bloggerSessionKey))
            {
                return Json(true.ToJsonOKData());
            }
            return Json(false.ToJsonOKData());
        }

        /// <summary>
        /// get list of blogs
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            var sd = repository.GetSubDomains().Where(x => x.id == subdomainid.Value).Single();
            if (string.IsNullOrEmpty(sd.bloggerSessionKey))
            {
                return Json("No permission".ToJsonFail());
            }

            var authFactory = new GAuthSubRequestFactory("blogger", "tradelr");
            authFactory.Token = sd.bloggerSessionKey;
            var service = new Service(authFactory.ApplicationName);
            service.RequestFactory = authFactory;

            var query = new FeedQuery();
            query.Uri = new Uri("http://www.blogger.com/feeds/default/blogs");
            AtomFeed feed = null;
            var blogs = new List<GoogleBlogData>();
            try
            {
                feed = service.Query(query);
                
                foreach (AtomEntry entry in feed.Entries)
                {
                    var blog = new GoogleBlogData();
                    blog.name = entry.Title.Text;
                    blog.blogHref = entry.AlternateUri.Content;
                    foreach (AtomLink t in entry.Links)
                    {
                        if (t.Rel.Equals("http://schemas.google.com/g/2005#post"))
                        {
                            blog.blogUri = t.AbsoluteUri;
                        }
                    }
                    blogs.Add(blog);
                }
            }
            catch(Exception ex)
            {
                Syslog.Write(ex);
                return Json("No blogs found".ToJsonFail());
            }
            var view = this.RenderViewToString("~/Areas/dashboard/Views/blogger/list.ascx", blogs);
            return Json(view.ToJsonOKData());
        }

        [JsonFilter(Param = "blogs", RootType = typeof(GoogleBlogData[]))]
        public ActionResult saveList(IEnumerable<GoogleBlogData> blogs)
        {
            foreach (var entry in blogs)
            {
                var blog = new googleBlog
                               {
                                   subdomainid = subdomainid.Value,
                                   title = entry.name,
                                   blogUri = entry.blogUri,
                                   blogHref = entry.blogHref
                               };
                db.googleBlogs.InsertOnSubmit(blog);
            }
            db.SubmitChanges();

            // return view
            var view = this.RenderViewToString("~/Areas/dashboard/Views/blogger/display.ascx", blogs);
            return Json(view.ToJsonOKData());
        }

        public ActionResult saveToken(string token)
        {
            var sd = repository.GetSubDomains().Where(x => x.id == subdomainid.Value).Single();
            var sessionToken = AuthSubUtil.exchangeForSessionToken(token, null);
            sd.bloggerSessionKey = sessionToken;
            repository.Save();
            return View("close");
        }
    }
}
