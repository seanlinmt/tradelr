using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public IQueryable<mail> GetMails()
        {
            return db.mails;
        }

        public void DeleteMail(mail entry)
        {
            db.mails.DeleteOnSubmit(entry);
        }
    }
}