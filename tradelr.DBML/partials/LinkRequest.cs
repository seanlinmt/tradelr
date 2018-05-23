using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddLinkRequest(linkRequest request)
        {
            db.linkRequests.InsertOnSubmit(request);
            db.SubmitChanges();
        }

        public void DeleteLinkRequest(linkRequest request)
        {
            db.linkRequests.DeleteOnSubmit(request);
        }

        public void DeleteLinkRequest(long userid, long friendid)
        {
            var req = GetLinkRequest(userid, friendid);
            if (req != null)
            {
                DeleteLinkRequest(req);
                db.SubmitChanges();
            }
        }

        public linkRequest GetLinkRequest(long userid, long friendid)
        {
            return db.linkRequests.Where(x => x.friendid == friendid && x.userid == userid).SingleOrDefault();
        }

        public linkRequest GetLinkRequest(long id)
        {
            return db.linkRequests.Where(x => x.id == id).SingleOrDefault();
        }
    }
}