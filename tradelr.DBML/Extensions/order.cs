using System.Linq;
using tradelr.Library;
using tradelr.Models.payment;
using tradelr.Models.transactions;

namespace tradelr.DBML
{
    public partial class order
    {
        public string status_old { get; set; }
        partial void  OnstatusChanging(string value)
        {
            status_old = status;
        }

        public bool hasDigitalOrderItem()
        {
            return orderItems.Any(x => x.orderItems_digitals != null);
        }

        public bool allDigitalOrderItems()
        {
            return orderItems.All(x => x.orderItems_digitals != null);
        }

        public string GetStatusStringForGrid(bool isReceiver)
        {
            var s = status.ToEnum<OrderStatus>();
            string result = "";
            switch (s)
            {
                case OrderStatus.PAID:
                    if (isReceiver)
                    {
                        result = "<span class='orderstatus_paid'>paid</span>";
                    }
                    else
                    {
                        if (payments.Any(x => x.status == PaymentStatus.New.ToString()))
                        {
                            result = string.Format("<a class='orderstatus_reviewpayment' href='/dashboard/{0}/{1}'>review payment</a>",
                                type == TransactionType.INVOICE.ToString() ? "invoices" : "orders",
                                id);
                        }
                        else
                        {
                            result = "<span class='orderstatus_paid'>paid</span>";
                        }
                    }
                    
                    break;
                case OrderStatus.PARTIAL:
                    result = "<span class='orderstatus_partial'>partial</span>";
                    break;
                case OrderStatus.SENT:
                    result = isReceiver
                                 ? "<span class='orderstatus_outstanding'>outstanding</span>"
                                 : "<span class='orderstatus_sent'>sent</span>";
                    break;
                case OrderStatus.SHIPPED:
                    result = "<span class='orderstatus_shipped'>shipped</span>";
                    break;
                case OrderStatus.VIEWED:
                    result = isReceiver
                                 ? "<span class='orderstatus_outstanding'>outstanding</span>"
                                 : "<span class='orderstatus_viewed'>viewed</span>";
                    break;
            }
            return result;
        }
    }
}
