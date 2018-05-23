using System;
using System.Data.Linq;
using System.Linq;

namespace tradelr.DBML
{
    public static class QueriesCompiled
    {
        private static readonly Func<tradelrDataContext, string, MASTERsubdomain> GetSubDomainQuery;
        private static readonly Func<tradelrDataContext, long, MASTERsubdomain> GetSubDomainByIDQuery;
        private static readonly Func<tradelrDataContext, long, long, user> GetUserByIdQuery;
        private static readonly Func<tradelrDataContext, string, long, user> GetUserByPasswordHashQuery;

        private static readonly Func<tradelrDataContext, string, MASTERsubdomain> GetCustomHostnameQuery;

        // this is used in situations where we need traverse tables using relationships to obtain data from other tables
        // as well
        private static DataLoadOptions dl;

        static QueriesCompiled()
        {
            GetCustomHostnameQuery = CompiledQuery.Compile<tradelrDataContext, string, MASTERsubdomain>(
                (db, hostname) => db.MASTERsubdomains.SingleOrDefault(x => x.customDomain == hostname)
                );

            GetSubDomainQuery = CompiledQuery.Compile<tradelrDataContext, string, MASTERsubdomain>(
                (db, name) => db.MASTERsubdomains.SingleOrDefault(x => x.name == name)
                );

            GetSubDomainByIDQuery = CompiledQuery.Compile<tradelrDataContext, long, MASTERsubdomain>(
                (db, id) => db.MASTERsubdomains.SingleOrDefault(x => x.id == id)
                );

            GetUserByIdQuery = CompiledQuery.Compile<tradelrDataContext, long, long, user>(
                (db, id, subdomain) =>
                db.users.SingleOrDefault(x => x.id == id && x.organisation1.subdomain == subdomain)
                );
            GetUserByPasswordHashQuery = CompiledQuery.Compile<tradelrDataContext, string, long, user>(
                (db, hash, subdomainid) =>
                db.users.SingleOrDefault(x => x.passwordHash == hash && x.organisation1.subdomain == subdomainid)
                );
        }

        public static MASTERsubdomain GetCustomHostname(this tradelrDataContext db, string hostname)
        {
            return GetCustomHostnameQuery(db, hostname);
        }

        public static MASTERsubdomain GetSubDomain(this tradelrDataContext db, string name)
        {
            return GetSubDomainQuery(db, name);
        }

        public static MASTERsubdomain GetSubDomainByID(this tradelrDataContext db, long id)
        {
            return GetSubDomainByIDQuery(db, id);
        }

        public static user GetUserById(this tradelrDataContext db, long sessionid, long subdomainid)
        {
            return GetUserByIdQuery(db, sessionid, subdomainid);
        }

        public static user GetUserByPasswordHash(this tradelrDataContext db, string hash, long subdomainid)
        {
            return GetUserByPasswordHashQuery(db, hash, subdomainid);
        }
    }
}