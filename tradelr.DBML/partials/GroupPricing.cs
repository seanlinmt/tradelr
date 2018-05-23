using System;
using System.Collections.Generic;
using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public IQueryable<contactGroupPricing> GetGroupPricings(long? groupid, long subdomainid)
        {
            // need distinct because there might be overlap
            var pricings = db.contactGroupPricings.Where(x => x.contactGroup.subdomainid == subdomainid);

            if (groupid.HasValue)
            {
                pricings = pricings.Where(x => x.groupid == groupid.Value);
            }

            return pricings;

        }

        public void DeleteGroupPricings(IEnumerable<contactGroupPricing> contactGroupPricings)
        {
            db.contactGroupPricings.DeleteAllOnSubmit(contactGroupPricings);
            db.SubmitChanges();
        }
    }
}