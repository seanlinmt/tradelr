using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeleteFavourite(long owner, long productid)
        {
            var fav = db.favourites.Where(x => x.owner == owner && x.productid == productid).SingleOrDefault();
            db.favourites.DeleteOnSubmit(fav);
            db.SubmitChanges();
        }

        public IQueryable<product> GetFavourites(long subdomainid, string categoryid, string sidx, string sord, long? owner)
        {
            IQueryable<product> results;

            // only return owner favourites
            if (owner.HasValue)
            {
                results = db.favourites.Where(x => x.owner == owner).Select(x => x.product);
            }
            else
            {
                results = db.favourites.Where(x => x.user.organisation1.subdomain == subdomainid).Select(x => x.product);
            }

            if (!string.IsNullOrEmpty(categoryid))
            {
                var id = long.Parse(categoryid);
                results = results.Where(x => x.category == id || x.productCategory.parentID == id);
            }

            return results;
        }

        public void AddFavourite(favourite fav)
        {
            db.favourites.InsertOnSubmit(fav);
            db.SubmitChanges();
        }

        public bool IsFavourite(long productid, long sessionid)
        {
            if (db.favourites.Where(x => x.productid == productid && x.owner == sessionid).Count() == 0)
            {
                return false;
            }
            return true;
        }
    }
}