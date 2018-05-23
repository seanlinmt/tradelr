using System;
using System.Linq;
using tradelr.Library;

namespace tradelr.DBML
{
    /// <summary>
    /// DONT FORGET TO CHECK AND EXPIRE ENTRIES WHEN GETTING ENTRIES FROM DB
    /// </summary>
    public partial class TradelrRepository
    {
        public void AddCoupon(coupon coupon)
        {
            db.coupons.InsertOnSubmit(coupon);
            db.SubmitChanges();
        }

        public void DeleteCoupon(coupon coupon)
        {
            db.coupons.DeleteOnSubmit(coupon);
            db.SubmitChanges();
        }

        public IQueryable<coupon> GetCoupons(long subdomainid, string sidx = "", string sord = "")
        {
            IQueryable<coupon> results = db.coupons.Where(x => x.subdomainid == subdomainid);

            // expire any time dependant entries
            var today = DateTime.UtcNow;
            foreach (var result in results)
            {
                if (!result.expired &&
                    ((result.expiryDate.HasValue && today > result.expiryDate.Value) || // past expiry date
                    (result.maxImpressions.HasValue && result.impressions >= result.maxImpressions.Value))) // exceeded max impressions
                {
                    result.expired = true;
                }
                else if (result.expired &&
                    ((result.expiryDate.HasValue && today < result.expiryDate.Value && today > result.startDate) ||
                    (!result.expiryDate.HasValue && today > result.startDate)))
                {
                    result.expired = false;
                }
            }
            db.SubmitChanges();

            IOrderedQueryable<coupon> ordered = null;
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
    }
}