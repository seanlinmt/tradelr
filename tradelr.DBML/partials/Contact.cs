using System.Collections.Generic;
using System.Linq;
using tradelr.DBML.Models;
using tradelr.Library;
using tradelr.Models.contacts;
using tradelr.Models.history;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public IQueryable<user> GetAllContacts(long subdomain)
        {
            return db.users.Where(x => x.organisation1.subdomain == subdomain);
        }

        public IQueryable<user> GetPrivateContacts(long subdomainid)
        {
            // this excludes staff members (admin users on the same domain)
            // exclude self too since ppl who can get contact lists are admins on the same subdomain
            return db.users.Where(x => (x.role & (int)UserRole.CREATOR) == 0 && x.organisation1.subdomain == subdomainid);
        }

        public IQueryable<user> GetPublicContacts(long requesterSubdomain)
        {
            var friend1 = db.friends.Where(x => x.subdomainid == requesterSubdomain)
                .Select(x => x.MASTERsubdomain1.id);
            var friend2 = db.friends.Where(x => x.friendsubdomainid == requesterSubdomain)
                .Select(x => x.MASTERsubdomain.id);
            var domainids = friend1.Union(friend2).ToList();
            return db.users.Where(x => (x.role & (int)UserRole.CREATOR) != 0 &&
                                       domainids.Contains(x.organisation1.subdomain));
        }

        public IQueryable<user> GetContacts(long subdomain, long requester, string filterList, string sidx, string sord,
            ContactType? type, string letter)
        {
            if (!string.IsNullOrEmpty(filterList))
            {
                var filterid = long.Parse(filterList);
                var results1 =
                    db.contactGroupMembers.Where(
                        x => x.groupid == filterid && x.contactGroup.subdomainid == requester).Select(x => x.user);

                if (!string.IsNullOrEmpty(letter))
                {
                    results1 = results1.Where(x => x.firstName.StartsWith(letter) || x.firstName.StartsWith(letter.ToLower()));
                }

                if (type.HasValue)
                {
                    switch (type.Value)
                    {
                        case ContactType.PRIVATE:
                            results1 = results1.Where(x => x.organisation1.subdomain == subdomain);
                            break;
                        case ContactType.NETWORK:
                            results1 = results1.Where(x => x.organisation1.subdomain != subdomain);
                            break;
                    }
                }

                IOrderedQueryable<user> ordered1 = null;
                if (!string.IsNullOrEmpty(sord) && !string.IsNullOrEmpty(sidx))
                {
                    if (sord == "asc")
                    {
                        ordered1 = results1.OrderBy(sidx);

                    }
                    else if (sord == "desc")
                    {
                        ordered1 = results1.OrderByDescending(sidx);
                    }
                }
                return (ordered1 ?? results1);
            }

            IQueryable<ContactQueryResult> results = null;
            var privatecontacts = GetPrivateContacts(subdomain).Select(x => new ContactQueryResult { contact = x, name = x.organisation1.name });
            var publiccontacts = GetPublicContacts(subdomain).Select(x => new ContactQueryResult { contact = x, name = x.organisation1.name });
            if (type.HasValue)
            {
                switch (type.Value)
                {
                    case ContactType.PRIVATE:
                        results = privatecontacts;
                        break;
                    case ContactType.NETWORK:
                        results = publiccontacts;
                        break;
                }
            }
            else
            {
                results = privatecontacts.Union(publiccontacts);
            }

            if (!string.IsNullOrEmpty(letter))
            {
                results = results.Where(x => x.contact.firstName.StartsWith(letter) || x.contact.firstName.StartsWith(letter.ToLower()));
            }

            IOrderedQueryable<user> ordered = null;
            if (!string.IsNullOrEmpty(sord) && !string.IsNullOrEmpty(sidx))
            {
                if (sord == "asc")
                {
                    ordered = results.Select(x => x.contact).OrderBy(sidx);

                }
                else if (sord == "desc")
                {
                    ordered = results.Select(x => x.contact).OrderByDescending(sidx);
                }
            }
            return (ordered ?? results.Select(x => x.contact));
        }

        public IQueryable<user> GetContacts(long subdomain, long requester, List<string> ids, string sidx, string sord)
        {
            IQueryable<user> results = db.users.Where(x => ids.Contains(x.id.ToString()) && x.id != requester);

            IOrderedQueryable<user> ordered = null;
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

        public void AddContactGroupMember(contactGroupMember member)
        {
            // check that member doesn't already exist
            contactGroupMember m = member;
            var exists =
                db.contactGroupMembers.Where(x => x.groupid == m.groupid && 
                                                x.userid == m.userid)
                                                .SingleOrDefault();
            if (exists == null)
            {
                db.contactGroupMembers.InsertOnSubmit(member);
                db.SubmitChanges();
            }
            else
            {
                member = exists;
            }
        }

        private void DeleteChangeHistory(long contextid, ChangeHistoryType type)
        {
            var items = db.changeHistoryItems.Where(x => x.changeID == contextid);
            db.changeHistoryItems.DeleteAllOnSubmit(items);
            var entry = db.changeHistories.Where(x => x.contextID == contextid && x.historyType == type.ToString()).SingleOrDefault();
            if (entry == null)
            {
                return;
            }
            db.changeHistories.DeleteOnSubmit(entry);
        }

        public contactGroup GetContactGroup(long groupid, long subdomainid)
        {
            return db.contactGroups.Where(x => x.id == groupid && x.subdomainid == subdomainid).SingleOrDefault();
        }

        public IQueryable<contactGroup> GetContactGroups(long subdomainid)
        {
            return db.contactGroups.Where(x => x.subdomainid == subdomainid);
        }

        public bool IsContactInUse(long contactid)
        {
            // if user is receiver or creator of an order
            if (db.orders.Where(x => x.owner == contactid || x.receiverUserid == contactid).Count() != 0)
            {
                return true;
            }

            return false;
        }

        public void UpdateContactGroupMembers(long subdomainid, long groupid, string[] userids)
        {
            // get existing ones first in filter
            var result = db.contactGroupMembers
                .Where(x => x.contactGroup.subdomainid == subdomainid &&
                            x.groupid == groupid);

            db.contactGroupMembers.DeleteAllOnSubmit(result);

            // add new ones
            foreach (var userid in userids)
            {
                var cf = new contactGroupMember();
                cf.userid = long.Parse(userid);
                cf.groupid = groupid;
                db.contactGroupMembers.InsertOnSubmit(cf);
            }

            db.SubmitChanges();
        }

        public void DeleteContactGroup(long subdomainid, long groupid)
        {
            // children set to DELETE CASCADE in db
            var result = db.contactGroups.Where(x => x.subdomainid == subdomainid && x.id == groupid);
            db.contactGroups.DeleteAllOnSubmit(result);
            db.SubmitChanges();
        }

        public user GetContact(long subdomain, long contactid)
        {
            return db.users.Where(x => x.organisation1.subdomain == subdomain && x.id == contactid).SingleOrDefault();
        }

    }
}