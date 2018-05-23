using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tradelr.Areas.dashboard.Models.store.navigation;
using tradelr.Common;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [TradelrHttps]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    public class linksController : baseController
    {
        [HttpGet]
        public ActionResult New()
        {
            var viewmodel = new LinkList();
            return View("ListContent", viewmodel);
        }

        [HttpPost]
        public ActionResult Delete(long id, long linkid)
        {
            var list = MASTERdomain.linklists.Where(x => x.id == id).SingleOrDefault();
            if (list == null)
            {
                return Json("List not found".ToJsonFail());
            }

            var link = list.links.Where(x => x.id == linkid).SingleOrDefault();
            if (link == null)
            {
                return Json("Link not found".ToJsonFail());
            }

            db.links.DeleteOnSubmit(link);

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("Link deleted successfully".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult DeleteList(long id)
        {
            var list = MASTERdomain.linklists.Where(x => x.id == id).SingleOrDefault();
            if (list == null)
            {
                return Json("List not found".ToJsonFail());
            }
            if (list.permanent)
            {
                return Json("This is a DEFAULT link list and cannot be deleted".ToJsonFail());
            }

            db.linklists.DeleteOnSubmit(list);

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("List deleted successfully".ToJsonFail());
        }

        public ActionResult Index()
        {
            var viewmodel = new LinksViewModel();
            viewmodel.linklists = MASTERdomain.linklists.ToModel();

            // handle linktype options
            var selectOptions = new Dictionary<string, IEnumerable<string[]>>();
            foreach (LinkType value in Enum.GetValues(typeof(LinkType)))
            {
                switch (value)
                {
                    case LinkType.BLOG:
                        var blogs = MASTERdomain.blogs.Select(x => new[] {x.title, x.id.ToString()});
                        selectOptions.Add(LinkType.BLOG.ToString(),blogs);
                        break;
                    case LinkType.FRONTPAGE:
                        // nothing
                        break;
                    case LinkType.COLLECTION:
                        var collections = MASTERdomain.product_collections.Select(x => new[] {x.name, x.id.ToString()});
                        selectOptions.Add(LinkType.COLLECTION.ToString(),collections);
                        break;
                    case LinkType.PAGE:
                        var pages = MASTERdomain.pages.Select(x => new[] {x.name, x.id.ToString()});
                        selectOptions.Add(LinkType.PAGE.ToString(),pages);
                        break;
                    case LinkType.PRODUCT:
                         var products = MASTERdomain.products
                             .OrderByDescending(x => x.id)
                             .Take(100)
                             .Select(x => new[] {x.title, x.id.ToString()});
                        selectOptions.Add(LinkType.PRODUCT.ToString(),products);
                        break;
                    case LinkType.SEARCHPAGE:
                        break;
                    case LinkType.WEB:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            var serializer = new JavaScriptSerializer();
            var jsonstring = serializer.Serialize(selectOptions);
            viewmodel.selectablesString = jsonstring;
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Save(string liquid_linklists)
        {
            // we need to use this serializer as we use enum strings
            var serializer = new JavaScriptSerializer();
            LinkList[] lists;
            try
            {
                lists = serializer.Deserialize<LinkList[]>(liquid_linklists);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            foreach (var entry in lists)
            {
                // new entry
                var list = new linklist();
                if (entry.id == 0)
                {
                    list.permanent = false;
                    MASTERdomain.linklists.Add(list);
                }
                else
                {
                    list = MASTERdomain.linklists.Where(x => x.id == entry.id).SingleOrDefault();
                    if (list == null)
                    {
                        return SendJsonErrorResponse("Unable to find link list: " + entry.id);
                    }

                }
                string perma = entry.handle;
                if (string.IsNullOrEmpty(perma))
                {
                    // this is a new entry or someone deleted the perma
                    perma = entry.title.ToPerma();
                }

                string perma1 = perma;
                var _linklistid = entry.id;
                if (db.linklists.Where(x => x.permalink == perma1 &&
                                x.subdomainid == subdomainid.Value &&
                                x.id != _linklistid).Count() != 0)
                {
                    perma = string.Format("{0}-{1}", db.linklists.Max(x => x.id) + 1, perma);
                }
                
                list.permalink = perma.ToMaxLength(100);

                list.title = entry.title;

                foreach (var entryLink in entry.links)
                {
                    var link = new link();
                    if (string.IsNullOrEmpty(entryLink.id))
                    {
                        list.links.Add(link);
                    }
                    else
                    {
                        link = list.links.Where(x => x.id.ToString() == entryLink.id).SingleOrDefault();
                        if (link == null)
                        {
                            return SendJsonErrorResponse("Unable to find link:" + entryLink.id);
                        }
                    }

                    link.title = entryLink.title;
                    link.type = (short)entryLink.link_type;
                    switch (entryLink.link_type)
                    {
                        case LinkType.BLOG:
                            var blog =
                                MASTERdomain.blogs.Where(x => x.id.ToString() == entryLink.url_selected).SingleOrDefault();
                            if (blog != null)
                            {
                                link.url = "/blogs/" + blog.permalink;
                            }
                            break;
                        case LinkType.FRONTPAGE:
                            link.url = Link.DEFAULT_FRONTPAGE;
                            break;
                        case LinkType.COLLECTION:
                            var collection =
                                MASTERdomain.product_collections.Where(x => x.id.ToString() == entryLink.url_selected).
                                    SingleOrDefault();
                            if (collection != null)
                            {
                                link.url = "/collections/" + collection.permalink;
                                if (!string.IsNullOrEmpty(entryLink.url_filter))
                                {
                                    link.url += ("/" + entryLink.url_filter.Replace(",", "+"));
                                }
                            }
                            break;
                        case LinkType.PAGE:
                            var page =
                                MASTERdomain.pages.Where(x => x.id.ToString() == entryLink.url_selected).SingleOrDefault();
                            if (page != null)
                            {
                                link.url = "/pages/" + page.permalink;
                            }
                            break;
                        case LinkType.PRODUCT:
                            var product =
                                MASTERdomain.products.Where(x => x.id.ToString() == entryLink.url_selected).SingleOrDefault();
                            if (product != null)
                            {
                                link.url = product.ToLiquidProductUrl();
                            }
                            break;
                        case LinkType.SEARCHPAGE:
                            link.url = Link.DEFAULT_SEARCHPAGE;
                            break;
                        case LinkType.WEB:
                            if (!string.IsNullOrEmpty(entryLink.url_raw))
                            {
                                link.url = entryLink.url_raw;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            repository.Save();

            return Json("Link lists saved successfully".ToJsonOKMessage());
        }
    }
}
