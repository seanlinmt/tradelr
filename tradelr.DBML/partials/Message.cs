using System.Linq;
using tradelr.Models.message;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddMessage(message message)
        {
            db.messages.InsertOnSubmit(message);
            db.SubmitChanges();
        }

        public void DeleteMessage(long sender, long recipient, MessageType msgtype)
        {
            var msg =
                db.messages.Where(
                    x => x.type == MessageType.LINKREQUEST.ToString() && x.sender == sender && x.recipient == recipient).
                    SingleOrDefault();
            if (msg != null)
            {
                db.messages.DeleteOnSubmit(msg);
            }
        }

        public IQueryable<message> GetMessages(long owner, bool isInbox)
        {
            if (isInbox)
            {
                return db.messages.Where(x => x.recipient == owner);
            }
            return db.messages.Where(x => x.sender == owner);
        }

        public IQueryable<message> GetLinkRequestNotifications(long owner)
        {
            return db.messages.Where(x => x.type == MessageType.LINKREQUEST.ToString() && x.recipient == owner);
        }
    }
}