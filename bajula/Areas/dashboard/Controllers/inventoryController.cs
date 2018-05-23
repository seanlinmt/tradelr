using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.collections;
using tradelr.Models.inventory;
using tradelr.Models.networks;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class inventoryController : baseController
    {
        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Alarm(long id)
        {
            var ilocitem = repository.GetInventoryLocationItem(id, subdomainid.Value).ToModel();
            return View(ilocitem);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Alarm(long id, int quantity)
        {
            var ilocitem = repository.GetInventoryLocationItem(id, subdomainid.Value);
            if (ilocitem == null)
            {
                return Json("Could not find item".ToJsonFail());
            }
            ilocitem.alarmLevel = quantity;
            repository.Save();
            return Json("Alarm level updated".ToJsonOKMessage());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Available(long id)
        {
            var ilocitem = repository.GetInventoryLocationItem(id, subdomainid.Value).ToModel();
            return View(ilocitem);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult Available(long id, string description, int quantity)
        {
            var ilocitem = repository.GetInventoryLocationItem(id, subdomainid.Value);
            if (ilocitem == null)
            {
                return Json("Could not find item".ToJsonFail());
            }

            // get difference
            int diff = quantity;
            if (ilocitem.available.HasValue)
            {
                diff = quantity - ilocitem.available.Value;
            }

            var invWorker = new InventoryWorker(ilocitem, subdomainid.Value, ilocitem.product_variant.product.trackInventory, ilocitem.product_variant.IsDigital());
            invWorker.SetValues(description, diff, null, null, null);

            repository.Save();
            return Json("Inventory level updated".ToJsonOKMessage());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult Export(string ids)
        {
            try
            {
                string[] idarray = string.IsNullOrEmpty(ids) ? null : ids.Split(new[] {','});
                var ms = new MemoryStream();
                using(var fs =
                    new FileStream(
                        GeneralConstants.APP_ROOT_DIR + "/Content/templates/tradelrInventoryTemplate.xls",
                        FileMode.Open, FileAccess.Read))
                {
                    var templateWorkbook = new HSSFWorkbook(fs, true);
                    var sheet = templateWorkbook.GetSheet("Products");

                    int rowcount = 16;

                    // get products
                    var products = repository.GetProducts(subdomainid.Value, idarray, null, null);

                    var currency = MASTERdomain.currency.ToCurrency();

                    foreach (var product in products)
                    {
                        foreach (var variant in product.product_variants)
                        {
                            var row = sheet.CreateRow(rowcount);
                            row.CreateCell(0).SetCellValue(SecurityElement.Escape(variant.sku));
                            row.CreateCell(1).SetCellValue(SecurityElement.Escape(product.title));
                            row.CreateCell(2).SetCellValue(SecurityElement.Escape(product.details));
                            string mainCategory = "";
                            string subCategory = "";
                            if (product.category.HasValue)
                            {
                                if (!product.productCategory.parentID.HasValue)
                                {
                                    mainCategory = product.productCategory.MASTERproductCategory.name;
                                }
                                else
                                {
                                    subCategory = product.productCategory.MASTERproductCategory.name;
                                    mainCategory = product.productCategory.productCategory1.MASTERproductCategory.name;
                                }
                            }
                            row.CreateCell(3).SetCellValue(SecurityElement.Escape(mainCategory));
                            row.CreateCell(4).SetCellValue(SecurityElement.Escape(subCategory));
                            row.CreateCell(5).SetCellValue(SecurityElement.Escape(product.stockUnitId.HasValue ? product.stockUnit.MASTERstockUnit.name : ""));

                            string costPrice = "";
                            string sellingPrice = "";
                            if (product.costPrice.HasValue)
                            {
                                costPrice = (product.costPrice.Value).ToString("n" + currency.decimalCount);
                            }

                            if (product.sellingPrice.HasValue)
                            {
                                sellingPrice = (product.sellingPrice.Value).ToString("n" + currency.decimalCount);
                            }

                            row.CreateCell(6).SetCellValue(SecurityElement.Escape(costPrice));
                            row.CreateCell(7).SetCellValue(SecurityElement.Escape(sellingPrice));
                            // in stock
                            row.CreateCell(8).SetCellValue(SecurityElement.Escape(variant.inventoryLocationItems.Sum(x => x.available).ToString()));
                            rowcount++;

                            // get photos
                            var photos = product.product_images.Select(x => x.url);
                            row.CreateCell(9).SetCellValue(SecurityElement.Escape(string.Join(",", photos.Select(x => accountHostname.ToDomainUrl(x)).ToArray())));
                        }

                    }
                    templateWorkbook.Write(ms);
                }

                // return created file path);
                return File(ms.ToArray(), "application/vnd.ms-excel", string.Format("TradelrInventory_{0}.xls", DateTime.UtcNow.ToShortDateString()));
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult History(long id)
        {
            var location = repository.GetInventoryLocationItem(id, subdomainid.Value);
            var histories = location.inventoryHistories.ToModel();

            return View(histories);
        }

        [HttpPost]
        public ActionResult Import(string id)
        {
            try
            {
                Stream inputStream;
                if(Request.Files.Count != 0)
                {
                    inputStream = Request.Files[0].InputStream;
                }
                else
                {
                    inputStream = Request.InputStream;
                }
                inputStream.Position = 0;
                List<ProductInfo> productsList;
                using (var stream = inputStream)
                {
                    var importer = new ProductImport();
                    productsList = importer.Import(stream, sessionid.Value, subdomainid.Value);
                }

                // need to identifiy duplicates in database
                var dbduplicates = productsList.Where(x => repository.GetProductVariants(x.p.subdomainid).Select(y => y.sku).Contains(x.p.product_variants[0].sku)).ToArray();
                var duplicateSKU = dbduplicates.SelectMany(x => x.p.product_variants.Select(y => y.sku)).ToArray();
                productsList.RemoveAll(dbduplicates.Contains);

                repository.AddProducts(productsList, subdomainid.Value);

                // update total of out of stock items
                repository.UpdateProductsOutOfStock(subdomainid.Value);
                repository.Save();

                return Json(duplicateSKU.ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpGet]
        public ActionResult Import()
        {
            return View(baseviewmodel);
        }

        [PermissionFilter(permission = UserPermission.INVENTORY_VIEW)]
        public ActionResult Index(string alarm)
        {
            var viewdata = new InventoryViewData(baseviewmodel) {permission = permission};
            if (permission.HasPermission(UserPermission.INVENTORY_VIEW))
            {
                viewdata.categoryList = repository.GetProductCategories(subdomainid.Value)
                                .ToFilterModel()
                                .ToFilterList();
                viewdata.collectionsList = repository.GetProductCollections(subdomainid.Value)
                                .OrderBy(x => x.name)
                                .ToFilterList();
                viewdata.locationList = repository.GetLocations(subdomainid.Value)
                                    .ToFilterModel()
                                    .ToFilterList();
            }
            
            return View(viewdata);
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult LocationAdd(string id)
        {
            return View((object)id);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult LocationCreate(string locationName)
        {
            locationName = locationName.Trim();
            if (accountLimits.locations.HasValue)
            {
                var locationCount =
                    repository.GetSubDomains().Where(x => x.id == subdomainid.Value).SingleOrDefault().
                        inventoryLocations.Count();
                if (locationCount >= accountLimits.locations.Value)
                {
                    return Json("Number of inventory locations exceeded. Please upgrade your <a href=\"/dashboard/account/plan\">plan</a>.".ToJsonFail());
                }
            }

            if (string.IsNullOrEmpty(locationName))
            {
                return Json("Name not specified.".ToJsonFail());
            }
            // check if location exists
            if (repository.GetInventoryLocation(locationName, subdomainid.Value) != null)
            {
                return Json("Location already exist.".ToJsonFail());
            }

            // check if special location
            if (Networks.SYNC_NETWORKS.Contains(locationName))
            {
                return Json("Name specified is reserved. Please select another name.".ToJsonFail());
            }

            var loc = new inventoryLocation
                          {
                              name = locationName,
                              subdomain = subdomainid.Value,
                              lastUpdate = DateTime.UtcNow
                          };
            
            // need to add location items for each product too
            var variants = repository.GetProductVariants(subdomainid.Value);
            foreach (var variant in variants)
            {
                var locitem = new inventoryLocationItem()
                                  {
                                      variantid = variant.id
                                  };
                loc.inventoryLocationItems.Add(locitem);
            }
            
            repository.AddInventoryLocation(loc, subdomainid.Value);

            return Json(loc.ToModel().ToJsonOKData());
        }
        
        [HttpPost]
        [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
        public ActionResult LocationDelete(long id)
        {
            try
            {
                var iloc = repository.GetInventoryLocation(id, subdomainid.Value);
                switch (iloc.name)
                {
                    case Networks.LOCATIONNAME_GBASE:
                        iloc.MASTERsubdomain.googleBase = null;
                        break;
                    default:
                        break;
                }

                repository.DeleteInventoryLocation(id, subdomainid.Value);  // SUBMIT
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json(id.ToJsonOKData());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.INVENTORY_ADD)]
        public ActionResult NewLoc(long id, string name)
        {
            var locationInfo = new InventoryLocation();
            locationInfo.id = id;
            locationInfo.title = name;
            return View("inventoryInfo", locationInfo);
        }

    }
}