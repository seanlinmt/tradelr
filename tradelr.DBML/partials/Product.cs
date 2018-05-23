using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using clearpixels.Logging;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.DBML.Models;
using tradelr.Library.Caching;
using tradelr.Models.counter;
using tradelr.Models.photos;
using tradelr.Models.products;
using tradelr.Library;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddProduct(ProductInfo productInfo, long subdomainid)
        {
            // update total products for us
            UpdateCounters(subdomainid, 1, CounterType.PRODUCTS_MINE);

            productInfo.p.created = DateTime.UtcNow;
            productInfo.p.updated = productInfo.p.created;
            db.products.InsertOnSubmit(productInfo.p);

            Save("AddProduct");
            
            Debug.Assert(productInfo.p.id != 0);

            // insert images
            foreach (var photoUrl in productInfo.photo_urls)
            {
                var url = photoUrl;
                new Thread(() => url.ReadAndSaveProductImageFromUrl(subdomainid, subdomainid, productInfo.p.id)).Start();
            }

            db.SubmitChanges();
#if LUCENE
            // index product
            var indexer = new LuceneWorker(db, GetSubDomain(subdomainid).ToIdName());
            indexer.AddToIndex(LuceneIndexType.PRODUCTS, productInfo.p);
#endif
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.ToString());
        }

        public void AddProducts(IEnumerable<ProductInfo> products, long subdomainid)
        {
            // used for adding supplier products
            foreach (var p in products)
            {
                AddProduct(p, subdomainid);
            }
        }

        public void DeleteProduct(long id, long subdomainid)
        {
            var p = db.products.SingleOrDefault(x => x.id == id && x.subdomainid == subdomainid);
            if (p == null)
            {
                Syslog.Write(string.Concat("can't find product to delete: ", id, ",", subdomainid));
                return;
            }

            // break circular reference
            p.thumb = null;
            Save();

            // remove gbase
            if (p.gbase_product != null)
            {
                db.gbase_products.DeleteOnSubmit(p.gbase_product);
            }

            // remove any wordpress posts
            if (p.wordpressPosts != null)
            {
                db.wordpressPosts.DeleteOnSubmit(p.wordpressPosts);
            }

            // remove any tumblr posts
            if (p.tumblrPosts != null)
            {
                db.tumblrPosts.DeleteOnSubmit(p.tumblrPosts);
            }

            // remove product collections
            db.productCollectionMembers.DeleteAllOnSubmit(p.productCollectionMembers);

            // remove group pricing
            var grouppricings = p.contactGroupPricings;
            db.contactGroupPricings.DeleteAllOnSubmit(grouppricings);

            // remove facebook imports
            var fbImports = p.facebook_imports;
            db.facebook_imports.DeleteAllOnSubmit(fbImports);

            // remove ebay entries
            if (p.ebayID.HasValue)
            {
                db.ebay_products.DeleteOnSubmit(p.ebay_product);
            }

            // remove product images
            if (p.product_images != null)
            {
                db.product_images.DeleteAllOnSubmit(p.product_images);
            }

            // update counters
            UpdateCounters(subdomainid, -1, CounterType.PRODUCTS_MINE);

            // delete tags
            if (p.tags1 != null)
            {
                db.tags.DeleteAllOnSubmit(p.tags1);
            }

            // update other friends counters
            var domainids = new List<long>();
            var excludedomainids = new HashSet<long>();
            
            
            var friendids = GetFriends(subdomainid).Select(x => x.id).ToList();
            foreach (var friendid in friendids)
            {
                if (!excludedomainids.Contains(friendid))
                {
                    domainids.Add(friendid);
                }
            }

            // delete variants
            db.product_variants.DeleteAllOnSubmit(p.product_variants);

            db.products.DeleteOnSubmit(p);
            try
            {
                // because thumbnail is set to NULL
                Save();
            }
            catch (Exception ex)
            {
                var resultString = "DeleteProduct:" + ex.Message;
                foreach (var conflict in db.ChangeConflicts)
                {
                    var cstr = "";
                    foreach (var mc in conflict.MemberConflicts)
                    {
                        cstr += string.Concat("name:", mc.Member.Name, ",");
                    }
                    resultString += cstr;
                }
                Syslog.Write(resultString);
            }

            CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, subdomainid.ToString());
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, id.ToString());
        }

        public void DeleteProductVariant(product_variant variant)
        {
            db.inventoryLocationItems.DeleteAllOnSubmit(variant.inventoryLocationItems);

            var histories = variant.inventoryLocationItems.SelectMany(x => x.inventoryHistories);
            db.inventoryHistories.DeleteAllOnSubmit(histories);

            db.product_variants.DeleteOnSubmit(variant);
            CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, variant.productid.ToString());
            db.SubmitChanges();
        }

        public product GetProduct(long id)
        {
            return db.products.Where(x => x.id == id).SingleOrDefault();
        }

        public product GetProduct(long id, long subdomain)
        {
            return db.products.Where(x => x.id == id && x.subdomainid == subdomain).SingleOrDefault();
        }

        public IQueryable<product> GetProducts(long subdomainid)
        {
            return db.products.Where(x => x.subdomainid == subdomainid && (x.flags & (int)ProductFlag.ARCHIVED) == 0);
        }

        public IQueryable<product_variant> GetProductVariants(long subdomain, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return db.product_variants.Where(x => x.product.subdomainid == subdomain);
            }

            return db.product_variants.Where(x => x.product.subdomainid == subdomain && x.sku.StartsWith(query));
        }

        public IQueryable<product> GetProducts(long subdomain, string categoryID,
                                                string sidx, string sord, string alarm, ProductFlag flag, long? collection)
        {
            IQueryable<product> results = db.products.Where(x => x.subdomainid == subdomain);
            if (!string.IsNullOrEmpty(categoryID))
            {
                var id = long.Parse(categoryID);
                results = results.Where(x => x.category == id || x.productCategory.parentID == id);
            }

            switch (flag)
            {
                case ProductFlag.NONE:
                    results = results.Where(x => (x.flags & (int)(ProductFlag.INACTIVE | ProductFlag.ARCHIVED)) == 0);
                    break;
                case ProductFlag.INACTIVE:
                    results = results.Where(x => (x.flags & (int)ProductFlag.INACTIVE) != 0);
                    break;
                case ProductFlag.ARCHIVED:
                    results = results.Where(x => (x.flags & (int)ProductFlag.ARCHIVED) != 0);
                    break;
            }

            if (collection.HasValue)
            {
                results =
                    results.Where(x => x.productCollectionMembers.Count(y => y.collectionid == collection.Value) != 0);
            }

            IOrderedQueryable<product> ordered = null;
            if (!string.IsNullOrEmpty(sord) && !string.IsNullOrEmpty(sidx))
            {
                if (sord == "asc")
                {
                    ordered = results.OrderBy(sidx);

                }
                else if (sord == "desc")
                {
                    ordered = results.OrderByDescending(sidx);
                }
            }

            return (ordered ?? results);
        }

        public IQueryable<product> GetProducts(long subdomain, IEnumerable<string> ids, string sidx, string sord)
        {
            IQueryable<product> results = db.products.Where(x => x.subdomainid == subdomain &&
                (x.flags & (int)ProductFlag.ARCHIVED) == 0);

            if (ids != null)
            {
                results = results.Where(x => ids.Contains(x.id.ToString()));
            }

            IOrderedQueryable<product> ordered = null;
            if (!string.IsNullOrEmpty(sord) && !string.IsNullOrEmpty(sidx))
            {
                if (sord == "asc")
                {
                    ordered = results.OrderBy(sidx);

                }
                else if (sord == "desc")
                {
                    ordered = results.OrderByDescending(sidx);
                }
            }

            return (ordered ?? results);
        }

        public IQueryable<product_variant> GetProductVariants(long subdomainid)
        {
            return db.product_variants.Where(x => x.product.subdomainid == subdomainid);
        }

        public IQueryable<product> GetStoreProducts(long subdomainid)
        {
            return
                db.products.Where(
                    x => x.subdomainid == subdomainid).IsActive();
        }

        public IQueryable<product> GetSupplierProducts(long subdomainid)
        {
            // get other stores
            var subdomainids = GetPublicContacts(subdomainid).Select(x => x.organisation1.MASTERsubdomain.id).ToList();

            return db.products.Where(x => subdomainids.Contains(x.subdomainid) &&
                                          (x.flags & (int) (ProductFlag.INACTIVE | ProductFlag.ARCHIVED)) == 0);
        }

        public IQueryable<product> GetSupplierProducts(long subdomainid, long orgid)
        {
            var usr = GetPublicContacts(subdomainid).Where(x => x.organisation.Value == orgid).FirstOrDefault();
            if (usr == null)
            {
                return null;
            }

            return db.products.Where(x => x.subdomainid == usr.organisation1.MASTERsubdomain.id &&
                                        (x.flags & (int)(ProductFlag.INACTIVE | ProductFlag.ARCHIVED)) == 0);
        }

        public IQueryable<product> GetSupplierProducts(long subdomainid, string categoryID, string sidx, string sord, string supplierID)
        {
            // get all supplier products
            var results = GetSupplierProducts(subdomainid);

            if (!string.IsNullOrEmpty(categoryID))
            {
                var catid = long.Parse(categoryID);
                results = results.Where(x => x.category == catid ||
                        (x.category.HasValue &&
                         x.productCategory.parentID == catid));
            }

            if (!string.IsNullOrEmpty(supplierID))
            {
                results = results.Where(x => x.subdomainid.ToString() == supplierID);
            }

            IOrderedQueryable<product> ordered = null;
            if (!string.IsNullOrEmpty(sord) && !string.IsNullOrEmpty(sidx))
            {
                var indexesInProduct = new[] {"title", "sellingPrice"};
                if (indexesInProduct.Contains(sidx))
                {
                    if (sord == "asc")
                    {
                        ordered = results.OrderBy(sidx);

                    }
                    else if (sord == "desc")
                    {
                        ordered = results.OrderByDescending(sidx);
                    }
                }
                else
                {
                    if (sord == "asc")
                    {
                        ordered = results.OrderBy(sidx);

                    }
                    else if (sord == "desc")
                    {
                        ordered = results.OrderByDescending(sidx);
                    }
                }
            }

            return (ordered ?? results);
        }

        public bool IsProductVariantInUse(long variantid, long subdomainid)
        {
            var variant = GetProductVariants(subdomainid).Where(x => x.id == variantid).SingleOrDefault();
            if (variant != null)
            {
                var count = db.orderItems.Where(x => x.variantid == variant.id).Count();
                if (count != 0)
                {
                    return true;
                }
            }
            return false;
        }

        

        public void UpdateProductMainThumbnail(long productid, long owner, string photoid)
        {
            var product = db.products.SingleOrDefault(x => x.subdomainid == owner && x.id == productid);
            if (product != null)
            {
                if (string.IsNullOrEmpty(photoid))
                {
                    product.thumb = null;
                }
                else
                {
                    product.thumb = long.Parse(photoid);
                }
                db.SubmitChanges();
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_subdomain, product.subdomainid.ToString());
                CacheHelper.Instance.invalidate_dependency(DependencyType.products_single, productid.ToString());
            }
            else
            {
                Syslog.Write(string.Concat("UpdateProductMainThumbnail: ", productid, ",", owner, ",", photoid));
            }
        }

        public bool IsProductInUse(long productid)
        {
            var product = db.products.SingleOrDefault(x => x.id == productid);
            if (product != null)
            {
                foreach (var variant in product.product_variants)
                {
                    if (db.orderItems.Count(x => x.variantid == variant.id) != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public product_variant GetProductVariant(string sku, long subdomainid, ProductFlag? excludeFlag)
        {
            var variant = db.product_variants.Where(x => x.sku == sku &&
                                                         x.product.subdomainid == subdomainid);

            if (excludeFlag.HasValue)
            {
                variant = variant.Where(x => (x.product.flags & (int)excludeFlag) == 0);
            }

            return variant.FirstOrDefault();
        }

        public product_variant GetProductVariant(long variantid, long subdomainid)
        {
            return db.product_variants.SingleOrDefault(x => x.id == variantid && x.product.subdomainid == subdomainid);
        }

        /// <summary>
        /// Get product suppliers by supplier name
        /// </summary>
        /// <param name="supplierName"></param>
        /// <returns></returns>
        public organisation GetProductSupplier(string supplierName)
        {
            return db.organisations.SingleOrDefault(x => x.name == supplierName);
        }
        
        public void UpdateProductsOutOfStock(long subdomain)
        {
            // find all products with reorder level <= inStock
            var count = db.inventoryLocationItems.Where(x => x.inventoryLocation.subdomain == subdomain &&
                                                             (x.available <= x.alarmLevel || x.available < 0)).Select(
                                                                 x => x.variantid).Distinct().Count();
            var dom = db.MASTERsubdomains.Single(x => x.organisation.subdomain == subdomain);
            dom.total_outofstock = count;
        }

        public void AddInventoryLocationItem(inventoryLocationItem list, long subdomainid)
        {
            var existing =
                db.inventoryLocationItems.SingleOrDefault(x => x.locationid == list.locationid &&
                                                               x.variantid == list.variantid);
            if (existing != null)
            {
                throw new Exception("Inventory entry already exist: " + existing);
            }
            db.inventoryLocationItems.InsertOnSubmit(list);
            db.SubmitChanges();
        }

        public long AddInventoryLocation(inventoryLocation location, long subdomainid)
        {
            var exist = db.inventoryLocations.SingleOrDefault(x => x.name == location.name && x.subdomain == subdomainid);
            if (exist != null)
            {
                return exist.id;
            }
            db.inventoryLocations.InsertOnSubmit(location);
            db.SubmitChanges();
            return location.id;
        }
    }
}