using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;

using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Caching;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.offline;
using tradelr.Models.offline.tables;
using tradelr.Models.photos;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Controllers.offline
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.CREATOR)]
    public class syncController : baseController
    {
        //
        // GET: /sync/
        [AcceptVerbs(HttpVerbs.Post)]
        [JsonFilter(Param = "syncdata", RootType = typeof(RequestSync))]
        public ActionResult Index(RequestSync syncdata)
        {
            var serializer = new JavaScriptSerializer();
            IEnumerable<IColumn> receivedrows = null;
            // deserialize what we have from the client
            try
            {
                switch (syncdata.type)
                {
                    case TableName.PRODUCTS:
                        receivedrows = serializer.Deserialize<ProductColumn[]>(syncdata.data);
                        break;
                    case TableName.CATEGORY:
                        receivedrows = serializer.Deserialize<CategoryColumn[]>(syncdata.data);
                        break;
                    case TableName.PHOTOS:
                        receivedrows = serializer.Deserialize<PhotosColumn[]>(syncdata.data);
                        break;
                    case TableName.INVENTORYLOC:
                        receivedrows = serializer.Deserialize<InventoryLocColumn[]>(syncdata.data);
                        break;
                    case TableName.INVENTORYLOCITEM:
                        receivedrows = serializer.Deserialize<InventoryLocItemColumn[]>(syncdata.data);
                        break;
                    case TableName.SETTINGS:
                        receivedrows = serializer.Deserialize<SettingsColumn[]>(syncdata.data);
                        break;
                    case TableName.STOCKUNIT:
                        receivedrows = serializer.Deserialize<StockUnitColumn[]>(syncdata.data);
                        break;
                    case TableName.ORDERS:
                        receivedrows = serializer.Deserialize<OrderColumn[]>(syncdata.data);
                        break;
                    case TableName.ORGS:
                        receivedrows = serializer.Deserialize<OrgColumn[]>(syncdata.data);
                        break;
                    default:
                        return new EmptyResult();
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return new EmptyResult();
            }

            dynamic returnrows;

            #region get new rows
            // checking against maxid ensures that we return new rows or if nothing from offline db 
            // then online ids will be definately > 0
            long maxid = 0;
            if (receivedrows != null && receivedrows.Count() != 0)
            {
                // no need to check for NONE cflag as maxid from client could be update row that will have the largest id
                var maxidrows = receivedrows.Where(x => x.serverid.HasValue);
                if (maxidrows.Count() != 0)
                {
                    maxid = maxidrows.Max(x => x.serverid.Value);
                }
            }
            switch (syncdata.type)
            {
                // commented out because of implementation of product variant
               //case TableName.PRODUCTS:
               //     returnrows =
               // repository.GetProducts(subdomainid.Value, "").Where(x => x.id > maxid).ToSyncModel(CFlag.CREATE);
               //     break;
                case TableName.CATEGORY:
                    returnrows =
                repository.GetProductCategories(subdomainid.Value).Where(x => x.id > maxid).ToSyncModel(CFlag.CREATE);
                    break;
                // commented out because of new image photos
                /*
                case TableName.PHOTOS:
                    // don't include PRODUCT where contextid = 0 as these are photos that are uploaded when abandoining product creation
                    returnrows =
                        db.product_images.Where(
                            x =>
                            x.subdomainid == subdomainid.Value && x.id > maxid &&
                            x.productid != 0).ToSyncModel(CFlag.CREATE);
                    break;
                 * */
                case TableName.INVENTORYLOC:
                    returnrows =
                        repository.GetInventoryLocationsExceptSyncNetworks(subdomainid.Value).Where(x => x.id > maxid).ToSyncModel(CFlag.CREATE);
                    break;
                case TableName.INVENTORYLOCITEM:
                    returnrows =
                        repository.GetInventoryLocationsExceptSyncNetworks(subdomainid.Value)
                        .SelectMany(x => x.inventoryLocationItems).Where(x => x.id > maxid).ToSyncModel(CFlag.CREATE);
                    break;
                case TableName.SETTINGS:
                    var settings = repository.GetSubDomains().Where(x => x.id == subdomainid.Value && x.id > maxid).SingleOrDefault();
                    if (settings != null)
                    {
                        returnrows = settings.ToSyncModel(CFlag.CREATE);
                    }
                    else
                    {
                        returnrows = new List<SettingsColumn>();
                    }
                    break;
                case TableName.STOCKUNIT:
                    returnrows =
                        db.stockUnits.Where(
                            x => x.subdomainid == subdomainid.Value && x.id > maxid).
                            ToSyncModel(CFlag.CREATE);
                    break;
                case TableName.ORDERS:
                    returnrows =
                        db.orders.Where(
                            x => x.user.organisation1.MASTERsubdomain.id == subdomainid.Value && x.id > maxid).
                            ToSyncModel(CFlag.CREATE);
                    break;
                case TableName.ORGS:
                    returnrows =
                        db.organisations.Where(
                            x => x.MASTERsubdomain.id == subdomainid.Value && x.id > maxid).
                            ToSyncModel(CFlag.CREATE);
                    break;
                default:
                    return new EmptyResult();
            } 
            #endregion

            #region check for updates since our last offline check
            // only tables that allow user editing needs to be checked
            // this has to be before values added from client because lastUpdate time will be before lastOfflinkCheck
            // since the server will only be touched after update
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            if (!usr.lastOfflineCheck.HasValue)
            {
                usr.lastOfflineCheck = DateTime.UtcNow;
                repository.Save();
            }
            else
            {
                DateTime? lastCheck = usr.lastOfflineCheck;
                dynamic changedRows = null;
                switch (syncdata.type)
                {
                    case TableName.PRODUCTS:
                        changedRows =
                            db.products.Where(
                                x => x.subdomainid == subdomainid.Value && x.updated > lastCheck).
                                ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.CATEGORY:
                        changedRows =
                            db.productCategories.Where(x => x.subdomain == subdomainid.Value && x.lastUpdate > lastCheck)
                                .ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.PHOTOS:
                        // not editable
                        break;
                    case TableName.INVENTORYLOC:
                        changedRows =
                            db.inventoryLocations.Where(
                                x => x.subdomain == subdomainid.Value && x.lastUpdate > lastCheck)
                                .ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.INVENTORYLOCITEM:
                        changedRows = db.inventoryLocationItems.Where(
                            x => x.inventoryLocation.subdomain == subdomainid.Value && x.lastUpdate > lastCheck)
                            .ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.SETTINGS:
                        break;
                    case TableName.STOCKUNIT:
                        // not editable or deletable at the moment
                        break;
                    case TableName.ORDERS:
                        changedRows = db.orders.Where(
                            x => x.user1.organisation1.subdomain == subdomainid.Value && x.lastUpdate > lastCheck)
                            .ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.ORGS:
                        changedRows = db.organisations.Where(
                            x => x.subdomain == subdomainid.Value && x.lastUpdate > lastCheck)
                            .ToSyncModel(CFlag.UPDATE);
                        break;
                    case TableName.USER:
                        changedRows = repository.GetContacts(subdomainid.Value, sessionid.Value, null, null, null).Where
                            (x => x.lastUpdate > lastCheck)
                            .ToSyncModel(CFlag.UPDATE);
                        break;
                    default:
                        return new EmptyResult();
                }
                if (changedRows != null)
                {
                    foreach (var changedRow in changedRows)
                    {
                        returnrows.Add(changedRow);
                    }
                }
            }
            #endregion

            #region handle changes from client
            if (receivedrows != null && receivedrows.Count() != 0)
            {
                foreach (var receivedrow in receivedrows)
                {
                    try
                    {
                        /////////////////////////////////// UPDATE
                        // serverid is used to identify online db id
                        switch (receivedrow.cflag)
                        {
                            case CFlag.UPDATE:
                                // handle change flag
                                switch (syncdata.type)
                                {
                                    case TableName.PRODUCTS:
                                        var p = repository.GetProduct(receivedrow.serverid.Value, subdomainid.Value);
                                        if (p != null)
                                        {
                                            var col = (ProductColumn) receivedrow;
                                            p.category = col.categoryid;
                                            p.costPrice = col.costPrice;
                                            p.details = col.details;
                                            p.flags = col.flags;
                                            p.otherNotes = col.notes;
                                            p.sellingPrice = col.sellingPrice;
                                            //p.SKU = col.SKU;
                                            p.stockUnitId = col.stockunitid;
                                            p.thumb = col.thumbnailid;
                                            p.title = col.title;
                                            p.updated = DateTime.UtcNow;
                                            repository.Save();
                                            CacheHelper.Instance.invalidate_dependency(
                                                DependencyType.products_subdomain, subdomainid.Value.ToString());
                                            CacheHelper.Instance.invalidate_dependency(DependencyType.products_single,
                                                                                       receivedrow.serverid.ToString());
                                        }
                                        else
                                        {
                                            Syslog.Write("SYNC: Can't find product " + receivedrow.id);
                                        }
                                        break;
                                    case TableName.CATEGORY:
                                        // cannot update product category
                                        break;
                                    case TableName.PHOTOS:
                                        // cannot update photos
                                        break;
                                    case TableName.INVENTORYLOC:
                                        // cannot update inventory locations
                                        break;
                                    case TableName.INVENTORYLOCITEM:
                                        var ilocitem = repository.GetInventoryLocationItem(receivedrow.serverid.Value,
                                                                                           subdomainid.Value);
                                        if (ilocitem != null)
                                        {
                                            var col = (InventoryLocItemColumn) receivedrow;
                                            ilocitem.available = col.inventoryLevel;
                                            ilocitem.alarmLevel = col.alarmLevel;
                                            repository.Save();
                                        }
                                        else
                                        {
                                            Syslog.Write("SYNC: Can't find inventoryLocationItem " + receivedrow.id);
                                        }
                                        break;
                                    case TableName.STOCKUNIT:
                                        // cannot update stockunit
                                        break;
                                    case TableName.ORDERS:
                                    case TableName.ORGS:
                                    case TableName.SETTINGS:
                                    default:
                                        throw new NotImplementedException();
                                }
                                receivedrow.cflag = CFlag.CLEAR;
                                returnrows.Add(receivedrow as dynamic);
                                break;
                            case CFlag.CREATE:
                                switch (syncdata.type)
                                {
                                    // commented out due to implementation of product variants
                                        /*
                                    case TableName.PRODUCTS:
                                        var col = (ProductColumn) receivedrow;
                                        // check for duplicate rows
                                        if(repository.GetProduct(col.SKU, subdomainid.Value) == null)
                                        {
                                            var p = new product()
                                            {
                                                subdomainid = subdomainid.Value,
                                                category = col.categoryid,
                                                costPrice = col.costPrice,
                                                details = col.details ?? "",
                                                flags = col.flags,
                                                otherNotes = col.notes,
                                                paymentTerms = col.paymentterms,
                                                sellingPrice = col.sellingPrice,
                                                shippingTerms = col.shippingterms,
                                                //SKU = col.SKU,
                                                stockUnitId = col.stockunitid,
                                                thumbnail = col.thumbnailid,
                                                title = col.title
                                            };
                                            repository.AddProduct(new ProductInfo(){p = p}, subdomainid.Value);
                                            receivedrow.serverid = p.id;
                                            receivedrow.cflag = CFlag.CLEAR;
                                            returnrows.Add(receivedrow as dynamic);
                                        }
                                        else
                                        {
                                            Syslog.Write("Duplicate SKU: " + subdomainid.Value + " " + col.SKU);
                                        }
                                        break;
                                         * */
                                    case TableName.CATEGORY:
                                        var catcol = (CategoryColumn) receivedrow;
                                        var mpc = repository.AddMasterProductCategory(catcol.name);
                                        var c = new productCategory()
                                                    {
                                                        masterID = mpc.id,
                                                        parentID = catcol.parentid,
                                                        subdomain = subdomainid.Value
                                                    };
                                        repository.AddProductCategory(c, subdomainid.Value);
                                        receivedrow.serverid = c.id;
                                        receivedrow.cflag = CFlag.CLEAR;
                                        returnrows.Add(receivedrow as dynamic);
                                        break;
                                    case TableName.PHOTOS:
                                        var pcol = (PhotosColumn) receivedrow;
                                        var imageurl = pcol.url.ToSavedImageUrl(sessionid.Value, subdomainid.Value);
                                        if (!string.IsNullOrEmpty(imageurl))
                                        {
                                            var photo = new image()
                                                            {
                                                                url = imageurl,
                                                                contextID = pcol.contextid,
                                                                imageType = pcol.type,
                                                                subdomain = subdomainid.Value
                                                            };
                                            repository.AddImage(photo);
                                            receivedrow.serverid = photo.id;
                                            receivedrow.cflag = CFlag.CLEAR;
                                            returnrows.Add(receivedrow as dynamic);
                                        }
                                        break;
                                    case TableName.INVENTORYLOC:
                                        var il = (InventoryLocColumn) receivedrow;
                                        var loc = new inventoryLocation()
                                                      {
                                                          name = il.name
                                                      };
                                        repository.AddInventoryLocation(loc, subdomainid.Value);
                                        receivedrow.serverid = loc.id;
                                        receivedrow.cflag = CFlag.CLEAR;
                                        returnrows.Add(receivedrow as dynamic);
                                        break;
                                    case TableName.INVENTORYLOCITEM:
                                        var ili = (InventoryLocItemColumn) receivedrow;
                                        if (ili.productid.HasValue)
                                        {
                                            var locitem = new inventoryLocationItem()
                                            {
                                                locationid = ili.locationid,
                                                available = ili.inventoryLevel,
                                                onOrder = ili.onOrder,
                                                variantid = ili.productid.Value,
                                                alarmLevel = ili.alarmLevel
                                            };
                                            repository.AddInventoryLocationItem(locitem, subdomainid.Value);
                                            receivedrow.serverid = locitem.id;
                                            receivedrow.cflag = CFlag.CLEAR;
                                            returnrows.Add(receivedrow as dynamic);    
                                        }
                                        break;
                                    case TableName.STOCKUNIT:
                                        var scol = (StockUnitColumn) receivedrow;
                                        var msu = repository.AddMasterStockUnit(scol.name);
                                        var sunit = new stockUnit()
                                                        {
                                                            unitID = msu.id,
                                                            subdomainid = subdomainid.Value
                                                        };
                                        repository.AddStockUnit(sunit);
                                        receivedrow.serverid = sunit.id;
                                        receivedrow.cflag = CFlag.CLEAR;
                                        returnrows.Add(receivedrow as dynamic);
                                        break;
                                    case TableName.SETTINGS:
                                        break;
                                    case TableName.ORDERS:
                                    case TableName.ORGS:
                                    default:
                                        throw new NotImplementedException();
                                }
                                break;
                            case CFlag.DELETE: /////////// DELETE
                                switch (syncdata.type)
                                {
                                    case TableName.PRODUCTS:
                                        repository.DeleteProduct(receivedrow.serverid.Value, subdomainid.Value);
                                        break;
                                    case TableName.CATEGORY:
                                        repository.DeleteProductCategories(subdomainid.Value,
                                                                           new[] {receivedrow.serverid.Value.ToString()});
                                        break;
                                    case TableName.PHOTOS:
                                        repository.DeleteImage(receivedrow.serverid.Value, subdomainid.Value, PhotoType.ALL);
                                        break;
                                    case TableName.INVENTORYLOC:
                                        try
                                        {
                                            var loc = db.inventoryLocations.Where(
                                            x => x.id == receivedrow.serverid.Value && x.subdomain == subdomainid.Value).Single();
                                            db.inventoryLocations.DeleteOnSubmit(loc);
                                            db.SubmitChanges();
                                        }
                                        catch // suppress error when deleting iloc before ilocitem entry due to constraint
                                        {

                                        }
                                        break;
                                    case TableName.SETTINGS:
                                        break;
                                    case TableName.INVENTORYLOCITEM:
                                        repository.DeleteInventoryLocationItem(receivedrow.serverid.Value, subdomainid.Value);
                                        break;
                                    case TableName.STOCKUNIT:
                                        repository.DeleteStockUnit(receivedrow.serverid.Value, subdomainid.Value);
                                        break;
                                    case TableName.ORDERS:
                                    case TableName.ORGS:
                                    default:
                                        throw new NotImplementedException();
                                }
                                receivedrow.cflag = CFlag.CLEAR;
                                returnrows.Add(receivedrow as dynamic);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Syslog.Write(ex);
                    }
                }
            } 
            #endregion

            return Json(((object)returnrows).ToJsonOKData());
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Touched()
        {
            // TODO: while we were away some changes could have sneaked in
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            if (usr != null)
            {
                usr.lastOfflineCheck = DateTime.UtcNow;
                repository.Save();
            }
            return new EmptyResult();
        }
    }
}
