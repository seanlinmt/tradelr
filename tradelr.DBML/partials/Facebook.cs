using System;
using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddUpdateFacebookToken(facebook_token ftoken)
        {
            var existing =
                db.facebook_tokens.SingleOrDefault(x => x.pageid == ftoken.pageid && x.subdomainid == ftoken.subdomainid);
            if (existing != null)
            {
                existing.accesstoken = ftoken.accesstoken;
            }
            else
            {
                db.facebook_tokens.InsertOnSubmit(ftoken);
            }
        }

        public void DeleteFacebookTokens(long subdomainid)
        {
            var existing = db.facebook_tokens.Where(x => x.subdomainid == subdomainid);
            db.facebook_tokens.DeleteAllOnSubmit(existing);
            db.SubmitChanges();
        }

        public void DeleteFacebookPage(facebookPage fbpage)
        {
            db.facebookPages.DeleteOnSubmit(fbpage);
            db.SubmitChanges();
        }

        public void AddFacebookPage(facebookPage facebookPage)
        {
            db.facebookPages.InsertOnSubmit(facebookPage);
            db.SubmitChanges();
        }

        public IQueryable<facebookPage> GetFacebookPage(string pageid)
        {
            return db.facebookPages.Where(x => x.pageid == pageid);
        }
    }
}