using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.users;

namespace tradelr.Models.transactions
{
    public class OrderView
    {
        public OrderView(order o, organisation s, string receiverAddress, long sessionid, TransactionType type)
        {
            order = o.ToModel(type, sessionid);
            sender = s.ToFullOrganisationAddress();
            banner =
                s.logo.HasValue
                    ? s.image.ToModel(Imgsize.BANNER).url.ToHtmlImage()
                    : "";
            receiver = receiverAddress;
            currency = o.currency.ToCurrency();

            // handle addresses
            if (!o.allDigitalOrderItems())
            {
                order.billingAddress = o.address1.ToHtmlString();
                order.shippingAddress = o.address.ToHtmlString();
            }

            if (o.shipwireTransaction != null)
            {
                submittedToShipwire = true;
            }

            transactionID = (o.transactions ?? o.transactions1.First()).id;

            // init commentrs for new inline format
            comments = Enumerable.Empty<OrderComment>();
        }

        private const string ButtonEdit = "<button onclick=\"window.location = '/dashboard/{0}/edit/{1}';\" class=\"icon_edit\" type=\"button\"> edit</button>";
        private const string ButtonEditShipping = "<button onclick=\"dialogBox_open('/dashboard/orders/editshipping/{0}', 500);\" type=\"button\"> update shipping cost</button>";
        private const string ButtonEmail = "<button onclick=\"dialogBox_open('/dashboard/orders/email/{0}?t={1}', 500);\" class=\"icon_email\" type=\"button\"> send</button>";
        private const string ButtonResendEmail = "<button onclick=\"dialogBox_open('/dashboard/orders/email/{0}?t={1}', 500);\" class=\"icon_email\" type=\"button\"> resend</button>";
        private const string ButtonShipOrder = "<button onclick=\"dialogBox_open('/dashboard/orders/ship/{0}', 500);\" class=\"icon_shipped\" title=\"Ships order and notifies buyer\" type=\"button\"> ship order</button>";
        private const string ButtonPrint = "<button class='icon_print' onclick='window.print();' type=\"button\"> print</button>";
        private const string ButtonMarkReceived = "<button id=\"markReceived\" onclick=\"dialogBox_open('/dashboard/orders/markreceived/{0}', 500);\" class=\"icon_received\" title=\"Enter the date the order was received\" type=\"button\"> order received</button>";

        // payment
        private const string ButtonEnterPayment = "<button onclick=\"dialogBox_open('/dashboard/payment/new/{0}', 550);\" class=\"green\" title=\"Enter amount paid by buyer\" type=\"button\"> enter payment</button>";
        private const string ButtonMakePayment = "<button onclick=\"dialogBox_open('/dashboard/payment/method/{0}', 500);\" class=\"green\" type=\"button\"> make payment</button>";

        private const string ButtonReviewPayment = "<button onclick=\"dialogBox_open('/dashboard/payment/review/{0}', 500);\" class=\"green\" type=\"button\"> review payment</button>";

        public Order order { get; private set; }
        public long transactionID { get; set; }
        public IEnumerable<OrderComment> comments { get; set; }
        public string banner { get; private set; }
        public string sender { get; private set; }
        public string receiver { get; private set; }
        public string bannerStatus { get; private set; }
        public string buttons { get; private set; }
        public Currency currency { get; private set; }
        private bool submittedToShipwire { get; set; }
        

        public void SetStatusRibbon(bool isReceiver)
        {
            const string template = "<div class='ribbon_status' style='background-color:{0}'><h2>{1}</h2></div><div class='ribbon_status_triangle' style='border-left-color:{2}'></div>";
            switch (order.orderStatus)
            {
                case OrderStatus.DRAFT:
                    bannerStatus = string.Format(template, "#999999", "SAVED", "#666666");
                    break;
                case OrderStatus.PAID:
                    bannerStatus = string.Format(template, "#33cc00", "PAID", "#006600");
                    break;
                case OrderStatus.SENT:
                    if (isReceiver)
                    {
                        bannerStatus = string.Format(template, "#e01f00", "OUTSTANDING", "#9E0B0F");
                    }
                    else
                    {
                        bannerStatus = string.Format(template, "#33cc00", "SENT", "#006600");
                    }
                    break;
                case OrderStatus.VIEWED:
                    if (isReceiver)
                    {
                        bannerStatus = string.Format(template, "#e01f00", "OUTSTANDING", "#9E0B0F");
                    }
                    else
                    {
                        bannerStatus = string.Format(template, "#999999", "VIEWED", "#666666");
                    }
                    break;
                //case OrderStatus.DISPUTED:
                //    bannerStatus = string.Format(template, "#e01f00", "DISPUTED", "#9E0B0F");
                //    break;
                case OrderStatus.PARTIAL:
                    bannerStatus = string.Format(template, "#ED8A04", "PARTIAL", "#9a5b05");
                    break;
                case OrderStatus.SHIPPED:
                    bannerStatus = string.Format(template, "#33cc00", "SHIPPED", "#006600");
                    break;
                default:
                    break;

            }
        }

        // look at specs for state diagram
        public void SetButtonsToShow(TransactionType type, bool isReceiver)
        {
            var sb = new StringBuilder();
            switch (order.orderStatus)
            {
                case OrderStatus.DRAFT:
                    if (isReceiver)
                    {
                        
                    }
                    else
                    {
                        sb.AppendFormat(ButtonEdit, type == TransactionType.ORDER ? "orders" : "invoices", order.id);
                        sb.AppendFormat(ButtonEmail, order.id, type == TransactionType.ORDER ? "p" : "s");
                        if (order.totalPaid != order.orderTotal)
                        {
                            sb.AppendFormat(ButtonEnterPayment, order.id);
                        }
                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
                case OrderStatus.SENT:
                    if (isReceiver)
                    {
                        sb.AppendFormat(ButtonMakePayment, order.id);
                    }
                    else
                    {
                        if (type == TransactionType.INVOICE)
                        {
                            sb.AppendFormat(ButtonEditShipping, order.id);
                            if (!submittedToShipwire)
                            {
                                sb.AppendFormat(ButtonShipOrder, order.id);
                            }
                        }
                        else
                        {
                            sb.AppendFormat(ButtonMarkReceived, order.id);
                        }
                        sb.AppendFormat(ButtonResendEmail, order.id, type == TransactionType.ORDER ? "p" : "s");

                        if (order.totalPaid != order.orderTotal)
                        {
                            sb.AppendFormat(ButtonEnterPayment, order.id);
                        }

                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
                case OrderStatus.PAID:
                    if (isReceiver)
                    {
                        if (type == TransactionType.INVOICE)
                        {
                            sb.AppendFormat(ButtonMarkReceived, order.id);
                        }
                    }
                    else
                    {
                        //sb.AppendFormat(ButtonEdit, type == TransactionType.PURCHASE_ORDER ? "orders" : "invoices", order.id);
                        if (type == TransactionType.INVOICE)
                        {
                            if (!submittedToShipwire)
                            {
                                sb.AppendFormat(ButtonShipOrder, order.id);
                            }
                        }
                        else
                        {
                            sb.AppendFormat(ButtonMarkReceived, order.id);
                        }

                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
                case OrderStatus.VIEWED:
                    if (isReceiver)
                    {
                        if (type == TransactionType.INVOICE)
                        {
                            sb.AppendFormat(ButtonMarkReceived, order.id);
                        }
                        sb.AppendFormat(ButtonMakePayment, order.id);
                    }
                    else
                    {
                        if (type == TransactionType.INVOICE)
                        {
                            sb.AppendFormat(ButtonEditShipping, order.id);
                            if (!submittedToShipwire)
                            {
                                sb.AppendFormat(ButtonShipOrder, order.id);
                            }
                        }
                        if (order.totalPaid != order.orderTotal)
                        {
                            sb.AppendFormat(ButtonEnterPayment, order.id);
                        }

                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
                /*
                case OrderStatus.DISPUTED:
                    if (isReceiver)
                    {
                        sb.AppendFormat(ButtonMakePayment, order.id);
                    }
                    else
                    {
                        sb.AppendFormat(ButtonEdit, type == TransactionType.PURCHASE_ORDER ? "orders" : "invoices", order.id);
                    }
                    break;
                 * */
                case OrderStatus.PARTIAL:
                    if (isReceiver)
                    {
                        sb.AppendFormat(ButtonMakePayment, order.id);
                    }
                    else
                    {
                        if (order.totalPaid != order.orderTotal)
                        {
                            sb.AppendFormat(ButtonEnterPayment, order.id);
                        }

                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
                case OrderStatus.SHIPPED:
                    if (isReceiver)
                    {
                        
                    }
                    else
                    {
                        if (order.totalPaid != order.orderTotal)
                        {
                            sb.AppendFormat(ButtonEnterPayment, order.id);
                        }

                        if (order.hasUnreviewedCustomPayment)
                        {
                            sb.AppendFormat(ButtonReviewPayment, order.id);
                        }
                    }
                    break;
            }
            sb.Append(ButtonPrint);
            buttons = sb.ToString();
        }
    }
}
