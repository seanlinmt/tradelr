using System;
using System.Linq;
using clearpixels.Logging;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.photos;
using tradelr.Models.users;
using tradelr.DBML.Lucene;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeleteUser(long userid, long subdomainid)
        {
            var user = db.users.SingleOrDefault(x => x.id == userid && x.organisation1.subdomain == subdomainid);
            if (user == null)
            {
                Syslog.Write(string.Concat("cannot delete user:", userid, ",", subdomainid));
                return;
            }
            var activities = db.activities.Where(x => x.owner == userid);
            db.activities.DeleteAllOnSubmit(activities);
            DeleteChangeHistory(userid, ChangeHistoryType.CONTACT);

            var contactFilters = db.contactGroupMembers.Where(x => x.userid == userid);
            db.contactGroupMembers.DeleteAllOnSubmit(contactFilters);

            var messages = db.messages.Where(x => x.recipient == userid || x.sender == userid);
            db.messages.DeleteAllOnSubmit(messages);

            var images = db.images.Where(x => x.subdomain == subdomainid &&
                                              (x.imageType == PhotoType.PROFILE.ToString() && x.contextID == userid));
            db.images.DeleteAllOnSubmit(images);

            if ((user.role & (int)UserRole.CREATOR) != 0)
            {
                // have to handle case where user is the creator
                // so unset as creator first
                var MASTERsubdomain = db.MASTERsubdomains.SingleOrDefault(x => x.creator == user.organisation);
                if (MASTERsubdomain != null)
                {
                    MASTERsubdomain.creator = null;
                }
            }

            // delete organisation
            var org = user.organisation1;

            // delete ebay user if any
            if (user.ebayID.HasValue)
            {
                db.ebay_users.DeleteOnSubmit(user.ebay_user);
            }

            db.users.DeleteOnSubmit(user);

            if (db.users.Count(x => x.organisation.Value == org.id) == 1)
            {
                // delete addresses
                if (org.address1 != null)
                {
                    db.addresses.DeleteOnSubmit(org.address1);
                }
                if (org.address2 != null)
                {
                    db.addresses.DeleteOnSubmit(org.address2);
                }
                db.organisations.DeleteOnSubmit(org);
            }

            // update totals
            UpdateCounters(subdomainid, -1, CounterType.CONTACTS_PRIVATE);
            db.SubmitChanges();
        }

        public user GetPrimaryUser(long orgid)
        {
            return db.users.FirstOrDefault(x => x.organisation.Value == orgid);
        }

        public user GetUserByEmailAndPassword(string emailpassword, long subdomain)
        {
            var users = db.users.Where(x => x.passwordHash != null && x.organisation1.subdomain == subdomain);
            foreach (var user in users)
            {
                if (BCrypt.CheckPassword(emailpassword, user.passwordHash))
                {
                    return user;
                }
            }
            return null;
        }

        public user GetUserByEbayID(string ebayuserid)
        {
            var usr = db.ebay_users.SingleOrDefault(x => x.ebayuserid == ebayuserid);

            if (usr == null)
            {
                return null;
            }

            return usr.users.First();
        }

        public IQueryable<user> GetUserByFBID(string fbid)
        {
            return db.users.Where(x => x.FBID == fbid);
        }

        public user GetUserByFBID(string fbid, long subdomainid)
        {
            return
                db.users.SingleOrDefault(x => x.FBID == fbid && x.organisation1.subdomain == subdomainid);
        }

        public user GetUserByTwitterID(string screenName, long subdomainid)
        {
            return
                db.users.SingleOrDefault(x => x.twitterID == screenName && x.organisation1.subdomain == subdomainid);
        }

        public user GetUserById(long id, long subdomain)
        {
            return db.users.SingleOrDefault(x => x.id == id && x.organisation1.subdomain == subdomain);
        }

        public user GetUserById(long id)
        {
            return db.users.SingleOrDefault(x => x.id == id);
        }

        public IQueryable<user> GetUsersByEmail(string email)
        {
            return db.users.Where(x => x.email == email);
        }

        public IQueryable<user> GetUsersByEmail(string email, long subdomainid)
        {
            return GetUsersByEmail(email)
                .Where(x => x.organisation1.subdomain == subdomainid);
        }

        public user GetUserByViewId(string viewid)
        {
            return db.users.SingleOrDefault(x => x.viewid == viewid);
        }

        public long AddUser(user u)
        {
            db.users.InsertOnSubmit(u);
            db.SubmitChanges();
#if LUCENE
            // index for search
            var indexer = new LuceneWorker(db, u.organisation1.MASTERsubdomain.ToIdName());
            indexer.AddToIndex(LuceneIndexType.CONTACTS, u);
#endif
            return u.id;
        }

    }
}