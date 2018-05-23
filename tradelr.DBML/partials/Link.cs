using System.Linq;
using tradelr.Models.counter;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddFriend(long subdomainid, long friendsubdomainid)
        {
            //check if friend exists
            if (!IsFriend(subdomainid, friendsubdomainid))
            {
                friend f = new friend { subdomainid = subdomainid, friendsubdomainid = friendsubdomainid };
                db.friends.InsertOnSubmit(f);

                // update total contacts count for both ppl
                UpdateCounters(subdomainid, 1, CounterType.CONTACTS_PUBLIC);
                UpdateCounters(friendsubdomainid, 1, CounterType.CONTACTS_PUBLIC);
            }
        }

        private IQueryable<MASTERsubdomain> GetFriends(long subdomainid)
        {
            var friend1 = db.friends.Where(x => x.subdomainid == subdomainid).Select(x => x.MASTERsubdomain1);
            var friend2 = db.friends.Where(x => x.friendsubdomainid == subdomainid).Select(x => x.MASTERsubdomain);
            return friend1.Union(friend2);
        }

        public bool IsFriend(long subdomainid, long friendsubdomainid)
        {
            var f = db.friends.Where(x => (x.friendsubdomainid == friendsubdomainid && x.subdomainid == subdomainid) ||
                                          (x.friendsubdomainid == subdomainid && x.subdomainid == friendsubdomainid));
            if (f.Count() == 0)
            {
                return false;
            }
            return true;
        }

        
    }
}