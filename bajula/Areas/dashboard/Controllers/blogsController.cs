using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class blogsController : baseController
    {
        [HttpPost]
        public ActionResult ArticleEdit(int id)
        {
            var a = MASTERdomain.blogs.SelectMany(x => x.articles).SingleOrDefault(x => x.id == id);
            if (a == null)
            {
                return RedirectToAction("NotFound", "Error", new { area = "" });
            }

            var viewmodel = new ArticleViewModel()
            {
                article = a.ToModel(true),
                blogList = MASTERdomain.blogs.Select(x => new SelectListItem()
                {
                    Text = x.title,
                    Value = x.id.ToString(),
                    Selected = x.id == a.blogid
                }),
                editMode = true
            };
            return View("ArticleEdit", viewmodel);
        }

        [HttpPost]
        public ActionResult ArticleNew(int blogid)
        {
            var viewmodel = new ArticleViewModel()
                                {
                                    article = new Article(),
                                    blogList = MASTERdomain.blogs.Select(x => new SelectListItem()
                                                                                  {
                                                                                      Text = x.title,
                                                                                      Value = x.id.ToString(),
                                                                                      Selected = x.id == blogid
                                                                                  })
                                };
            return View("ArticleEdit", viewmodel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ArticleSave(long? id, string title, string content, string tags, bool publish, int target_blog)
        {
            var a = new article();
            if (id.HasValue)
            {
                a = MASTERdomain.blogs.SelectMany(x => x.articles).SingleOrDefault(x => x.id == id.Value);
                if (a == null)
                {
                    return Json("Article not found".ToJsonFail());
                }
                db.article_tags.DeleteAllOnSubmit(a.article_tags);
            }
            else
            {
                a.created = DateTime.UtcNow;
                a.creator = sessionid.Value;
                var blog = MASTERdomain.blogs.SingleOrDefault(x => x.id == target_blog);
                if (blog == null)
                {
                    return Json("Blog not found".ToJsonFail());
                }
                blog.articles.Add(a);
            }
            a.title = title;
            a.content = content;
            a.blogid = target_blog;
            a.published = publish? DateTime.UtcNow: (DateTime?) null;

            var taglist = tags.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var tag in taglist)
            {
                var t = new article_tag();
                t.name = tag;
                t.handle = tag.ToPerma();
                t.subdomainid = subdomainid.Value;
                a.article_tags.Add(t);
            }

            repository.Save();

            return Json("Article saved successfully".ToJsonOKMessage());
        }

        public ActionResult Delete(int id)
        {
            var blog = MASTERdomain.blogs.SingleOrDefault(x => x.id == id);
            if (blog == null)
            {
                return Json("Blog not found".ToJsonFail());
            }

            var tags = blog.articles.SelectMany(x => x.article_tags);
            var comments = blog.articles.SelectMany(x => x.article_comments);

            db.article_tags.DeleteAllOnSubmit(tags);
            db.article_comments.DeleteAllOnSubmit(comments);
            db.articles.DeleteAllOnSubmit(blog.articles);
            db.blogs.DeleteOnSubmit(blog);
            repository.Save();

            return Json("Blog successfully deleted".ToJsonOKMessage());
        }

        public ActionResult Edit(int id)
        {
            var blog = MASTERdomain.blogs.SingleOrDefault(x => x.id == id);
            if (blog == null)
            {
                return RedirectToAction("NotFound", "Error", new { area = "" });
            }

            var viewmodel = blog.ToModel(true);

            return View(viewmodel);
        }

        public ActionResult New()
        {
            var viewmodel = new Blog
                                {
                                    commentsList = Enum.GetValues(typeof (Commenting)).Cast<Commenting>()
                                        .Select(x => new SelectListItem()
                                                         {
                                                             Text = x.ToDescriptionString(),
                                                             Value = x.ToInt().ToString()
                                                         })
                                };
            return View("edit", viewmodel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(int? id, Commenting comments, string title)
        {
            var blog = new blog();
            if (id.HasValue)
            {
                blog = MASTERdomain.blogs.SingleOrDefault(x => x.id == id.Value);
                if (blog == null)
                {
                    return RedirectToAction("NotFound", "Error", new { area = "" });
                }
            }
            else
            {
                // TODO: allow blog handle to be editable
                var perma = title.ToPerma();
                string perma1 = perma;
                if (db.blogs.Count(x => x.permalink == perma1 &&
                                        x.subdomainid == subdomainid.Value) != 0)
                {
                    perma = string.Format("{0}-{1}", db.blogs.Max(x => x.id) + 1, perma);
                }
                blog.permalink = perma.ToMaxLength(100);
                blog.subdomainid = subdomainid.Value;
                MASTERdomain.blogs.Add(blog);
            }
            
            blog.comments = (short) comments;
            blog.title = title;
            blog.updated = DateTime.UtcNow;

            repository.Save();

            return Json("Blog save successfully".ToJsonOKMessage());

        }



    }
}
