using System;
using System.Linq;
using tradelr.Models.payment;
using tradelr.Models.time;
using tradelr.Library;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeletePayment(payment payment)
        {
            db.payments.DeleteOnSubmit(payment);
        }

        public payment GetPayment(long id)
        {
            return db.payments.SingleOrDefault(x => x.id == id);
        }

        public payment GetPaymentByReference(string reference)
        {
            return db.payments.SingleOrDefault(x => x.reference == reference);
        }

        public IQueryable<payment> GetPayments(PaymentMethodType method, PaymentStatus status)
        {
            return db.payments.Where(x => x.method == method.ToString() && x.status == status.ToString());
        }

        public IQueryable<payment> GetPayments(long owner, TimeLine? interval, string sidx, string sord)
        {
            IQueryable<payment> results;
            if (interval.HasValue)
            {
                results = db.payments
                    .Where(x => x.order.owner == owner &&
                                x.created > interval.Value.ToPointInTime());
            }
            else
            {
                results = db.payments.Where(x => x.order.owner == owner);
            }
            

            IOrderedQueryable<payment> ordered = null;
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
    }
}