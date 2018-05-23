using System.Linq;
using tradelr.Library.Caching;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public long AddOrganisation(organisation o)
        {
            db.organisations.InsertOnSubmit(o);
            db.SubmitChanges();
            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, o.subdomain.ToString());
            return o.id;
        }

        public IQueryable<organisation> GetAllOrganisationExceptOwn(long subdomainid, long ownOrgID)
        {
            var privatecontacts = GetPrivateContacts(subdomainid)
                .Where(x => x.organisation != ownOrgID)
                .Select(x => x.organisation1)
                .Distinct();

            var publiccontacts = GetPublicContacts(subdomainid)
                .Select(x => x.organisation1)
                .Distinct();

            return privatecontacts.Union(publiccontacts);
        }

        public organisation GetOrganisation(long orgid)
        {
            return db.organisations.SingleOrDefault(x => x.id == orgid);
        }

        public organisation GetOrganisation(long orgID, long subdomain)
        {
            return db.organisations.SingleOrDefault(x => x.subdomain == subdomain && x.id == orgID);
        }
    }
}