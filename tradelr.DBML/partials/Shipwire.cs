using System.Linq;
using clearpixels.Logging;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddShipwireTransaction(shipwireTransaction t)
        {
            var existing = db.shipwireTransactions.Where(x => x.orderid == t.orderid).SingleOrDefault();
            if (existing == null)
            {
                db.shipwireTransactions.InsertOnSubmit(t);
            }
            else
            {
                Syslog.Write("Existing shipwiretransaction: {0} {1}", existing.transactionid,
                                           t.transactionid);
                existing.transactionid = t.transactionid;
                existing.message = t.message;
            }
            db.SubmitChanges();
        }
    }
}