using System.Linq;
using tradelr.Library.Caching;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public long AddProductCategory(productCategory category, long subdomain)
        {
            productCategory result;
            if (category.parentID.HasValue)
            {
                result = db.productCategories.Where(x => x.masterID == category.masterID && 
                                x.subdomain == subdomain && x.parentID == category.parentID).SingleOrDefault();
            }
            else
            {
                result = db.productCategories.Where(x => x.masterID == category.masterID &&
                                x.subdomain == subdomain && !x.parentID.HasValue).SingleOrDefault();
            }
            if (result != null)
            {
                return result.id;
            }
            db.productCategories.InsertOnSubmit(category);
            db.SubmitChanges();
            CacheHelper.Instance.invalidate_dependency(DependencyType.categories, subdomain.ToString());
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomain.ToString());
            return category.id;
        }

        public void UpdateProductCategories(long subdomain, long catid, string[] allPIDs, string[] updatePIDs)
        {
            // get all products that need to be updated
            var affected = db.products.Where(x => x.subdomainid == subdomain && allPIDs.Contains(x.id.ToString()));
            foreach (var a in affected)
            {
                a.category = null;
            }

            var toupdate = db.products.Where(x => x.subdomainid == subdomain && updatePIDs.Contains(x.id.ToString()));

            // add new ones
            foreach (var entry in toupdate)
            {
                entry.category = catid;
            }

            db.SubmitChanges();
            CacheHelper.Instance.invalidate_dependency(DependencyType.categories, subdomain.ToString());
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomain.ToString());
        }

        /// <summary>
        /// Gets all top level product categories or sub level categories if a category ID is being specified
        /// </summary>
        /// <param name="maincategoryID"></param>
        /// <param name="subdomain"></param>
        /// <returns></returns>
        public IQueryable<productCategory> GetProductCategories(long? maincategoryID, long subdomain)
        {
            if (maincategoryID.HasValue)
            {
                return db.productCategories.Where(x => x.subdomain == subdomain && x.parentID == maincategoryID);
            }
            return db.productCategories.Where(x => x.subdomain == subdomain && !x.parentID.HasValue);
        }

        public IQueryable<productCategory> GetProductCategoriesOfContactsButMine(long subdomainid)
        {
            var subdomainids = GetPublicContacts(subdomainid).Select(x => x.organisation1.subdomain).ToArray();
            return db.productCategories.Where(x => subdomainids.Contains(x.subdomain));
        }

        public void DeleteProductCategories(long subdomain, string[] strings)
        {
            IQueryable<productCategory> results;
            if (strings == null)
            {
                results = db.productCategories.Where(x => x.subdomain == subdomain);
            }
            else
            {
                results = db.productCategories.Where(x => x.subdomain == subdomain &&
                                                        (strings.Contains(x.id.ToString()) ||
                                                            strings.Contains(x.parentID.Value.ToString()))
                                                     );
            }

            // clear product category in products
            var productsUsingTheseCategories =
                db.products.Where(x => x.category.HasValue && results.ToList().Select(y => y.id).Contains(x.category.Value));
            foreach (var product in productsUsingTheseCategories)
            {
                product.category = null;
            }
            db.productCategories.DeleteAllOnSubmit(results);
            db.SubmitChanges();
            CacheHelper.Instance.invalidate_dependency(DependencyType.categories, subdomain.ToString());
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomain.ToString());
        }

        public productCategory GetProductCategory(long id)
        {
            return db.productCategories.Where(x => x.id == id).SingleOrDefault();
        }

        public IQueryable<productCategory> GetProductCategories(long subdomain)
        {
            return db.productCategories.Where(x => x.subdomain == subdomain);
        }

        public IQueryable<MASTERproductCategory> FindMASTERProductCategories(string query, long owner)
        {
            return db.MASTERproductCategories.Where(x => x.name.StartsWith(query));
        }

        public MASTERproductCategory AddMasterProductCategory(string name)
        {
            var exist =
                db.MASTERproductCategories.Where(x => x.name == name)
                    .SingleOrDefault();
            if (exist == null)
            {
                exist = new MASTERproductCategory();
                exist.name = name;
                db.MASTERproductCategories.InsertOnSubmit(exist);
                db.SubmitChanges();
            }
            return exist;
        }

    }
}