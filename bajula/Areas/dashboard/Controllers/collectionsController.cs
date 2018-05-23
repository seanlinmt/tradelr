using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.collections;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class collectionsController : baseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add()
        {
            return View();
        }

        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string title, string details, bool visible)
        {
            title = title.Trim();
            details = details.Trim();

            // if it's empty then
            if (string.IsNullOrEmpty(title))
            {
                return SendJsonErrorResponse("Please specify a collection name");
            }

            if (string.Compare(title, "categories", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return SendJsonErrorResponse("Create FAIL. Name is RESERVED. Choose another name.");
            }

            var collection = new product_collection
            {
                name = title,
                details = details,
                subdomainid = subdomainid.Value
            };

            var perma = title.ToPerma();
            string perma1 = perma;
            if (db.product_collections.Count(x => x.permalink == perma1 &&
                                                  x.subdomainid == subdomainid.Value) != 0)
            {
                perma = string.Format("{0}-{1}", db.product_collections.Max(x => x.id) + 1, perma);
            }
            collection.permalink = perma.ToMaxLength(100);

            if (visible)
            {
                collection.settings |= (int)CollectionSettings.VISIBLE;
            }
            else
            {
                collection.settings &= ~(int)CollectionSettings.VISIBLE;
            }

            try
            {
                repository.AddProductCollection(collection, subdomainid.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(new { collection.id, collection.name}.ToJsonOKData());
        }

        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(long? id)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Collection ID not specified");
            }
            try
            {
                var collection = repository.GetProductCollection(id.Value, subdomainid.Value);
                if (collection == null)
                {
                    return Json("Cannot find collection".ToJsonFail());

                }

                if (((CollectionSettings)collection.settings).HasFlag(CollectionSettings.PERMANENT))
                {
                    return Json("This is a DEFAULT collection. It cannot be deleted".ToJsonFail());
                }

                // start deleting
                db.productCollectionMembers.DeleteAllOnSubmit(collection.productCollectionMembers);
                db.product_collections.DeleteOnSubmit(collection);

                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(id.ToJsonOKData());
        }

        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(long? id)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Collection ID was not specified");
            }
            var collection = repository.GetProductCollection(id.Value, subdomainid.Value);
            if (collection == null)
            {
                return SendJsonErrorResponse("Invalid Collection ID specified");
            }

            var products = repository.GetProducts(subdomainid.Value);
            var productList = collection.productCollectionMembers.Select(x => x.productid).ToArray();

            var viewmodel = new CollectionViewModel
                               {
                                   id = collection.id,
                                   permalink = collection.permalink,
                                   visible = ((CollectionSettings) collection.settings).HasFlag(CollectionSettings.VISIBLE),
                                   details = collection.details,
                                   title = collection.name,
                                   fullUrl = accountHostname.ToDomainUrl("/collections/" + collection.permalink),
                                   products = products.ToBaseModel(),
                                   productids = string.Join(",", productList)
                               };
            return View(viewmodel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Update(long id, string ids, string selectedIDs, string title, string details, bool visible, string handle)
        {
            var collection = repository.GetProductCollection(id, subdomainid.Value);
            if (collection == null)
            {
                return Json("Collection not found".ToJsonFail());
            }

            if (string.Compare(title, "categories", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return SendJsonErrorResponse("Create FAIL. Name is RESERVED. Choose another name.");
            }

            collection.name = title;
            collection.details = details;

            var perma = handle.ToPerma();
            string perma1 = perma;
            if (db.product_collections.Count(x => x.permalink == perma1 &&
                                                  x.subdomainid == subdomainid.Value &&
                                                  x.id != id) != 0)
            {
                perma = string.Format("{0}-{1}", db.product_collections.Max(x => x.id) + 1, perma);
            }
            collection.permalink = perma.ToMaxLength(100);

            if (visible)
            {
                collection.settings |= (int) CollectionSettings.VISIBLE;
            }
            else
            {
                collection.settings &= ~(int)CollectionSettings.VISIBLE;
            }

            var oldselectedids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var selectedids = selectedIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            repository.UpdateProductCollection(subdomainid.Value, 
                                                id,
                                               Array.ConvertAll(oldselectedids, long.Parse),
                                               Array.ConvertAll(selectedids, long.Parse));


            repository.Save();
            /*
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.Value.ToString());

            var resetids = new HashSet<string>(oldselectedids);
            resetids.UnionWith(selectedids);
            foreach (var resetid in resetids)
            {
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, resetid);
            }
             * */
            return Json(new { collection.id, collection.name}.ToJsonOKData());
        }


    }
}
