using System;
using System.Diagnostics;
using System.Linq;
using clearpixels.Logging;
using tradelr.Library;
using tradelr.Models.history;
using tradelr.Models.time;
using tradelr.Models.transactions;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void DeleteOrder(long id)
        {
            var o = db.orders.Single(x => x.id == id);
            DeleteOrder(o);
        }

        public IQueryable<order> GetOrders()
        {
            return db.orders;
        }

        public IQueryable<order> GetOrders(long subdomainid, TransactionType type, long viewerid, TimeLine? interval, string sidx, string sord, bool meIsReceiver)
        {
            IQueryable<order> results = db.orders.AsQueryable();

            if (type != TransactionType.ALL)
            {
                results = results.Where(x => x.type == type.ToString());
            }
            if (interval.HasValue)
            {
                results = results.Where(x => x.user1.organisation1.MASTERsubdomain.id == subdomainid &&
                                x.orderDate > interval.Value.ToPointInTime());
            }
            else
            {
                results = results.Where(x => x.user1.organisation1.MASTERsubdomain.id == subdomainid);
            }

            if (meIsReceiver)
            {
                // we also don't want receiver to see transactions that have not been sent
                results = results.Where(x => x.receiverUserid == viewerid && x.status != OrderStatus.DRAFT.ToString());
            }

            IOrderedQueryable<order> ordered = null;
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

        public void AddOrder(order order)
        {
            db.orders.InsertOnSubmit(order);
            db.SubmitChanges();
        }

        public void DeleteOrder(order o)
        {

            // delete addresses
            if (o.address != null)
            {
                db.addresses.DeleteOnSubmit(o.address);
            }

            if (o.address1 != null)
            {
                db.addresses.DeleteOnSubmit(o.address1);
            }
            
            // delete change history
            DeleteChangeHistory(o.id,
                                o.type == TransactionType.INVOICE.ToString() ? ChangeHistoryType.INVOICE : ChangeHistoryType.ORDERS);

            if (o.transactions != null)
            {
                // delete comments too
                db.comments.DeleteAllOnSubmit(o.transactions.comments);

                // this must be a new order
                db.transactions.DeleteOnSubmit(o.transactions);
            }
            else
            {
                // this must be a reference order
                if (o.transactions1 != null && o.transactions1.Count != 0)
                {
                    o.transactions1[0].secondaryOrderID = null;
                }
            }

            // delete payments
            db.payments.DeleteAllOnSubmit(o.payments);

            // delete ebay order
            if (o.ebayID.HasValue)
            {
                db.ebay_orderitems.DeleteAllOnSubmit(o.ebay_order.ebay_orderitems);
                db.ebay_orders.DeleteOnSubmit(o.ebay_order);
            }

            db.orders.DeleteOnSubmit(o);
            Save();
        }

        public order GetOrder(long orderid)
        {
            return db.orders.SingleOrDefault(x => x.id == orderid);
        }

        public order GetOrder(long subdomainid, long id)
        {
            return db.orders.SingleOrDefault(x => (x.user1.organisation1.subdomain == subdomainid || 
                                                   (x.receiverUserid.HasValue && x.user.organisation1.subdomain == subdomainid)) && 
                                                  x.id == id);
        }

        public order GetOrder(long orgid, TransactionType type, long orderNumber)
        {
            return db.orders.SingleOrDefault(x => x.user1.organisation.Value == orgid &&
                                                  x.type == type.ToString() &&
                                                  x.orderNumber == orderNumber);
        }

        public IQueryable<order> GetAllPurchaseOrders(long subdomain)
        {
            return db.orders.Where(x => x.receiverUserid.HasValue &&  
                                        x.user.organisation1.subdomain == subdomain &&
                                        x.type == TransactionType.ORDER.ToString());
        }

        public IQueryable<order> GetAllInvoices(long subdomain)
        {
            return db.orders.Where(x => x.receiverUserid.HasValue && 
                                        x.user.organisation1.subdomain == subdomain &&
                                        x.type == TransactionType.INVOICE.ToString());
        }

        public int GetMonthlyInvoiceCount(long sessionid)
        {
            var time = DateTime.UtcNow;
            var month = time.Month;
            var year = time.Year;
            return db.orders.Count(x => x.owner == sessionid &&
                                        x.type == TransactionType.INVOICE.ToString() &&
                                        x.created.Month == month &&
                                        x.created.Year == year);
        }

        public void UpdateOrderStatus(long orderid, TransactionType type, long? userId, long? receiverId, OrderStatus status)
        {
            Debug.Assert(userId.HasValue || receiverId.HasValue);
            order exist;
            if (userId.HasValue)
            {
                exist = db.orders.SingleOrDefault(x => x.id == orderid &&
                                                       x.owner == userId.Value &&
                                                       x.type == type.ToString());
            }
            else
            {
                exist = db.orders.SingleOrDefault(x => x.id == orderid &&
                                                       x.receiverUserid == receiverId.Value &&
                                                       x.type == type.ToString());
            }

            if (exist != null)
            {
                // only update to viewed status if previous status is sent
                if (exist.status != OrderStatus.SENT.ToString() &&
                    status == OrderStatus.VIEWED)
                {
                    return;
                }
                exist.status = status.ToString();
            }
            db.SubmitChanges();
        }

        public void UpdateOrderStatus(long id, long owner, OrderStatus status)
        {
            var exist = db.orders.SingleOrDefault(x => x.id == id &&
                                                       x.owner == owner);
            if (exist != null)
            {
                exist.status = status.ToString();
                db.SubmitChanges();
            }
        }

        public long GetNewOrderNumber(long subdomain, TransactionType type)
        {
            var orders = db.orders.Where(x => x.user1.organisation1.subdomain == subdomain && x.type == type.ToString());
            if (!orders.Any())
            {
                return 1;
            }
            var maxorderNumber = orders.Max( x => x.orderNumber);
            return maxorderNumber + 1;
        }

        public void UpdateOrderViewID(order order)
        {
            if (string.IsNullOrEmpty(order.viewid))
            {
                var guid = Guid.NewGuid().ToString("N");
                // check if exist
                bool duplicate = true;
                while (duplicate)
                {
                    var exist = GetOrderByViewID(guid);
                    if (exist == null)
                    {
                        duplicate = false;
                    }
                    else
                    {
                        guid = Guid.NewGuid().ToString("N");
                        // log this
                        Syslog.Write("Duplicate ViewID");
                    }
                }
                order.viewid = guid;
                db.SubmitChanges();
            }
        }

        public order GetOrderByViewID(string viewid)
        {
            return db.orders.SingleOrDefault(x => x.viewid == viewid);
        }

        public order GetOrderByOrderNumber(long subdomain, TransactionType type, long ordernumber)
        {
            return db.orders.SingleOrDefault(x => x.user1.organisation1.MASTERsubdomain.id == subdomain && x.orderNumber == ordernumber && x.type == type.ToString());
        }

        public transaction GetTransaction(long id)
        {
            return db.transactions.SingleOrDefault(x => x.id == id);
        }
    }
}