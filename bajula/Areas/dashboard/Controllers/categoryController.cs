using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Library.Imaging;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Caching;
using tradelr.Library.JSON;
using tradelr.Models.category;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [TradelrHttps]
    public class categoryController : baseController
    {
        [HttpGet]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Add()
        {
            return View();
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [HttpPost]
        public ActionResult Add(string categoryTitle, string categoryTitleSelected)
        {
            categoryTitle = categoryTitle.Trim();

            // if it's empty then
            if (string.IsNullOrEmpty(categoryTitle))
            {
                return SendJsonErrorResponse("Empty category name");
            }

            var category = new productCategory
                                {
                                    masterID = string.IsNullOrEmpty(categoryTitleSelected) ? repository.AddMasterProductCategory(categoryTitle).id : long.Parse(categoryTitleSelected),
                                    details = "",
                                    subdomain = subdomainid.Value
                                };


            try
            {
                repository.AddProductCategory(category, subdomainid.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            if (category.id == 0)
            {
                return Json(string.Format("Category <strong>{0}</strong> already exist", categoryTitle).ToJsonFail());
            }
                
            return Json(category.ToModel().ToJsonOKData());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult AddSub(string id)
        {
            // check that catid is valid
            if (string.IsNullOrEmpty(id))
            {
                return View(new ProductCategory());
            }
            var maincategory = repository.GetProductCategory(long.Parse(id));
            if (maincategory == null)
            {
                return SendJsonErrorResponse("Invalid category ID");
            }
            return View(maincategory.ToModel());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AddSub(string id, string categoryTitle, string categoryTitleSelected)
        {
            categoryTitle = categoryTitle.Trim();

            // if it's empty then
            if (string.IsNullOrEmpty(categoryTitle))
            {
                return SendJsonErrorResponse("Empty category name");
            }

            // check if subcategory already exist
            if (string.IsNullOrEmpty(id))
            {
                return SendJsonErrorResponse("Missing category id");
            }
            var parentid = long.Parse(id);
            var subcategories = repository.GetProductCategories(parentid, subdomainid.Value);
            if (subcategories.Where(x => x.MASTERproductCategory.name == categoryTitle).Count() != 0)
            {
                return SendJsonErrorResponse(string.Format("Subcategory <strong>{0}</strong> already exists", categoryTitle));
            }

            var category = new productCategory
            {
                masterID = string.IsNullOrEmpty(categoryTitleSelected) ? repository.AddMasterProductCategory(categoryTitle).id : long.Parse(categoryTitleSelected),
                details = "",
                subdomain = subdomainid.Value,
                parentID = parentid
            };

            try
            {
                repository.AddProductCategory(category, subdomainid.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            if (category.id == 0)
            {
                return SendJsonErrorResponse(string.Format("Subcategory <strong></strong> already exists",categoryTitle));
            }

            return Json(category.ToModel().ToJsonOKData());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string id)
        {
            string[] categoryIDs = null;
            if (!string.IsNullOrEmpty(id))
            {
                categoryIDs = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                repository.DeleteProductCategories(subdomainid.Value, categoryIDs);
            }
            // TODO: should only return successful deletes
            return Json(categoryIDs.ToJsonOKData());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Edit(long? id)
        {
            if (!id.HasValue)
            {
                return SendJsonErrorResponse("Category ID was not specified");
            }
            string categoryTitle = repository.GetProductCategory(id.Value).MASTERproductCategory.name;
            var products = repository.GetProducts(subdomainid.Value).OrderByDescending(x => x.category == id).ThenBy(x => x.id);
            var productsInCategory = products.Where(x => x.category == id).Select(x => x.id).ToArray();
            var takecount = Math.Max(20, productsInCategory.Length);
            var viewdata = new CategoryViewModel
                               {
                                   categoryTitle = categoryTitle,
                                   products = products.Take(takecount).ToBaseModel(),
                                   catid = id.Value,
                                   productIdsInCategory = string.Join(",", productsInCategory)
                               };
            return View(viewdata);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Products(long id)
        {
            var products =
                repository.GetProducts(subdomainid.Value).Where(x => x.id > id).OrderBy(x => x.id).Take(20);
            return Json(products.ToBaseModel().ToJsonOKData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Update(string id, string ids, string selectedIDs)
        {
            // need to trim /n from values
            var oldselectedids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            var newselectedids = selectedIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();

            repository.UpdateProductCategories(subdomainid.Value, long.Parse(id), oldselectedids, newselectedids);

            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.Value.ToString());

            var resetids = new HashSet<string>(oldselectedids);
            resetids.UnionWith(newselectedids);
            foreach (var resetid in resetids)
            {
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, resetid);
            }

            return Json(id.ToJsonOKMessage());
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult Find(string q, string limit)
        {
            var result = repository.FindMASTERProductCategories(q, sessionid.Value);
            var data = result.Select(x => new {title = x.name});
            return Json(data.ToList().ToJsonOKData(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">parent category ID</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetSub(long id)
        {
            var subcategory = repository.GetProductCategories(id, subdomainid.Value);
            return Json(subcategory.ToModel().ToJsonOKData());
        }

    }
}