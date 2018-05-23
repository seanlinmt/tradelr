using System.Collections.Generic;
using System.Linq;
using tradelr.Library.Caching;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public long AddProductCollection(product_collection collection, long subdomainid)
        {
            var existing =
                db.product_collections.Where(
                    x =>
                    x.name == collection.name && x.subdomainid == subdomainid &&
                    x.etsy_sectionid == collection.etsy_sectionid && 
                    x.etsy_shopid == collection.etsy_shopid).
                    SingleOrDefault();

            if (existing != null)
            {
                return existing.id;
            }

            db.product_collections.InsertOnSubmit(collection);
            db.SubmitChanges();
            return collection.id;
        }

        public void DeleteProductCollection(long collectionid, long subdomainid)
        {
            var collection = GetProductCollection(collectionid, subdomainid);
            if (collection != null)
            {
                db.productCollectionMembers.DeleteAllOnSubmit(collection.productCollectionMembers);
                db.product_collections.DeleteOnSubmit(collection);
                db.SubmitChanges();
            }
        }

        public product_collection GetProductCollection(long collectionid, long subdomainid)
        {
            return GetProductCollections(subdomainid).Where(x => x.id == collectionid).SingleOrDefault();
        }

        public IQueryable<product_collection> GetProductCollections(long subdomainid)
        {
            return db.product_collections.Where(x => x.subdomainid == subdomainid);
        }

        public void UpdateProductCollection(long subdomain, long collectionid, IEnumerable<long> oldSelectedPIDs, IEnumerable<long> selectedPIDs)
        {
            var oldSelectedProducts =
                db.productCollectionMembers.Where(x => x.collectionid == collectionid && oldSelectedPIDs.Contains(x.productid) && x.product.subdomainid == subdomain);
            db.productCollectionMembers.DeleteAllOnSubmit(oldSelectedProducts);

            Save();

            var newSelectedProducts = db.products.Where(x => x.subdomainid == subdomain && selectedPIDs.Contains(x.id));

            // add new ones
            foreach (var entry in newSelectedProducts)
            {
                var newentry = new productCollectionMember();
                newentry.collectionid = collectionid;
                newentry.productid = entry.id;
                db.productCollectionMembers.InsertOnSubmit(newentry);
            }

            //CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomain.ToString());
        }
    }
}