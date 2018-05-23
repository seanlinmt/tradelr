using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.Areas.dashboard.Models.store.page;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.store.viewmodel;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class pagesController : baseController
    {
        [HttpPost]
        public ActionResult Delete(long id)
        {
            var page = MASTERdomain.pages.Where(x => x.id == id).SingleOrDefault();
            if (page == null)
            {
                return Json("Cannot find page".ToJsonFail());
            }
            
            db.pages.DeleteOnSubmit(page);
            repository.Save();

            return Json("Page deleted successfully".ToJsonOKMessage());
        }

        [HttpGet]
        public ActionResult Index()
        {
            // we return a list of everything
            var viewmodel = new PagesViewModel();
            viewmodel.pages =
                MASTERdomain.pages.Select(
                    x =>
                    new Page()
                    {
                        id = x.id.ToString(),
                        title = x.name,
                        updated = x.updated.ToString(GeneralConstants.DATEFORMAT_STANDARD),
                        visible = ((PageSettings)x.settings).HasFlag(PageSettings.VISIBLE)
                    });
            viewmodel.blogs = MASTERdomain.blogs.Select(x => new Blog()
            {
                id = x.id.ToString(),
                title = x.title,
                articles = x.articles.ToModel(),
                commentType = (Commenting)x.comments
            });
            return View(viewmodel);
        }

        public ActionResult New()
        {
            var themeHandler = new ThemeHandler(MASTERdomain, false);
            var page = new Page();
            page.templateList = themeHandler.GetTemplateNamesStartingWith("page")
                .Select(x => new SelectListItem(){Text = x, Value = x})
                .OrderBy(x => x.Text.Length);
            return View("edit", page);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(long? id, string title, string content, bool visible, string handle, string template)
        {
            var page = new page();

            if (id.HasValue)
            {
                page = MASTERdomain.pages.SingleOrDefault(x => x.id == id.Value);
                if (page == null)
                {
                    return Json("Could not find page".ToJsonFail());
                }
                if (!string.IsNullOrEmpty(handle))
                {
                    page.permalink = handle.ToPerma();
                }
            }
            else
            {
                page.settings = (int)PageSettings.NONE;
                MASTERdomain.pages.Add(page);
                page.creator = sessionid.Value;
            }

            page.permalink = title.ToPerma();
            if (db.pages.Count(x => x.permalink == page.permalink && 
                                    x.subdomainid == subdomainid.Value &&
                                    x.id != page.id) != 0)
            {
                page.permalink = string.Format("{0}-{1}", db.pages.Max(x => x.id) + 1, page.permalink);
            }
            page.permalink = page.permalink.ToMaxLength(100);

            page.name = title;
            page.content = content;
            page.updated = DateTime.UtcNow;
            if (visible)
            {
                page.settings |= (int) PageSettings.VISIBLE;
            }
            else
            {
                page.settings &= ~(int) PageSettings.VISIBLE;
            }

            page.templatename = template;

            repository.Save();

            return Json("Page saved successfully".ToJsonOKMessage());
        }

        public ActionResult View(long id)
        {
            var page = MASTERdomain.pages.Where(x => x.id == id).SingleOrDefault();
            if (page == null)
            {
                return RedirectToAction("NotFound", "Error", new { area = "" });
            }
            var themeHandler = new ThemeHandler(MASTERdomain, false);
            return View("edit", new Page()
            {
                id = page.id.ToString(),
                title = page.name,
                pageUrl = accountHostname.ToDomainUrl("/pages/"),
                permalink = page.permalink,
                content = page.content,
                templateList = themeHandler.GetTemplateNamesStartingWith("page")
                                            .Select(x => new SelectListItem()
                                                             {
                                                                 Text = x, 
                                                                 Value = x,
                                                                 Selected = x == page.templatename
                                                             }),
                updated = page.updated.ToString(GeneralConstants.DATEFORMAT_STANDARD),
                visible = ((PageSettings)page.settings).HasFlag(PageSettings.VISIBLE),
                editMode = true
            });
        }

#if DEBUG
        public ActionResult Test()
        {
            return View(baseviewmodel);
        }
#endif
    }
}
