using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Models.facebook;
using tradelr.Models.liquid.models.Blog;
using tradelr.Models.liquid.models.Form;
using Blog = tradelr.Models.liquid.models.Blog.Blog;

namespace tradelr.Controllers.liquid
{
    public class articlesController : baseController
    {
        [HttpPost]
        public ActionResult Comments(string blogid, string aid, string name, string email, string body)
        {
            // verify form fields
            var form = new Form()
                           {
                               name = name,
                               email = email,
                               body = body,
                               errors = new FormErrors()  // init MUST be done here because liquid uses this to detect if there are errors
                           };
            if (string.IsNullOrEmpty(name))
            {
                form.errors.Add("author");
            }
            if (string.IsNullOrEmpty(email))
            {
                form.errors.Add("email");
            }
            if (string.IsNullOrEmpty(body))
            {
                form.errors.Add("body");
            }

            if (form.errors.Count != 0)
            {
                TempData["form"] = form;
                return RedirectToAction("Index");
            }

            
            var articleid_idx = aid.IndexOf("-");
            if (articleid_idx == -1)
            {
                return RedirectToAction("Index");
            }
            var articleid = aid.Substring(0, articleid_idx);

            var blog = MASTERdomain.blogs.Where(x => string.Equals(x.permalink, blogid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (blog == null)
            {
                return RedirectToAction("Index");
            }
            var commentType = (Commenting)blog.comments;

            var article =
                blog.articles.Where(y => y.id.ToString() == articleid).SingleOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var c = new article_comment
            {
                comment = body,
                email = email,
                name = name,
                created = DateTime.UtcNow,
                isApproved = commentType == Commenting.ON
            };
            article.article_comments.Add(c);

            repository.Save();
            form = new Form
                       {
                           posted_successfully = true
                       };

            TempData["form"] = form;

            return RedirectToAction("Index");
        }


        public ActionResult Index(string aid, string blogid)
        {
            var articleid_idx = aid.IndexOf("-");
            if (articleid_idx == -1)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var blog = MASTERdomain.blogs.Where(x => string.Equals(x.permalink, blogid, StringComparison.InvariantCultureIgnoreCase)).SingleOrDefault();
            if (blog == null)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var articleid = aid.Substring(0, articleid_idx);
            var article = blog.articles.Where(x => x.id.ToString() == articleid).SingleOrDefault();
            if (article == null)
            {
                return Content(CreatePageMissingTemplate().Render());
            }

            var template = CreateLiquidTemplate("article", article.title);
            // opengraph
            var opengraph = MASTERdomain.organisation.ToOpenGraph(null, article);
            template.AddHeaderContent(this.RenderViewToString("~/Views/store/liquid/defaultHeader.ascx", opengraph));
            
            template.InitContentTemplate("templates/article.liquid");
            template.AddParameters("article", article.ToLiquidModel());
            template.AddParameters("blog", new Blog(blog));
            template.AddParameters("comment", TempData["form"]);
            return Content(template.Render());
        }

    }
}
