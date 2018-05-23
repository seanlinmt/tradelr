using System;
using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.jqgrid;
using tradelr.Models.payment;
using tradelr.Models.users;

namespace tradelr.Models.transactions
{
    public class Order
    {
        public long? id { get; set; }

        public bool isOwner { get; set; }
        public bool isNew { get; set; }

        public long orderNumber { get; set; }  
        public TransactionType TransactionType { get; set; }
        // needs to be nullable because we want to be able to set empty string when creating new order
        public DateTime? orderDate { get; set; } 
        public string location { get; set; }
        public OrderStatus orderStatus { get; set; }
        public long receiverOrgID { get; set; }
        public string receiverName { get; set; }
        public string terms { get; set; }
        public Currency currency { get; set; }
        public string discount { get; set; }
        public string discountType { get; set; }
        public string discountCode { get; set; }

        public List<OrderItem> orderItems { get; set; }

        // for viewing invoice
        public string subTotal { get; set; }
        public string orderTotal { get; set; }
        public string totalPaid { get; set; }
        public decimal amountDue { get; set; }

        // shipping
        public string shippingMethod { get; set; }
        public string shippingCost { get; set; }
        public string shipwireShippingID { get; set; }
        public string shippingNotes { get; set; }

        // tax
        public string totalTax { get; set; }
        
        // payment
        public string paymentMethod { get; set; }
        public string paymentInstructions { get; set; }
        public bool hasUnreviewedCustomPayment { get; set; }

        // addresses
        public string shippingAddress { get; set; }
        public string billingAddress { get; set; }

        public Order()
        {
            orderItems = new List<OrderItem>();
        }

        public void CreateEmptyOrder(long onumber, organisation org, TransactionType type)
        {
            orderNumber = onumber;
            isNew = true;
            orderDate = DateTime.UtcNow;
            terms = org.MASTERsubdomain.paymentTerms;
            TransactionType = type;
            currency = org.MASTERsubdomain.currency.ToCurrency();
        }
    }

    public static class OrderHelper
    {
        public static void UpdateOrderTotal(this order o, long subdomainid, int currency_decimalplaces, string couponCode, ITradelrRepository repository)
        {
            // we need to update invoice total
            decimal total = 0;
            foreach (var orderItem in o.orderItems)
            {
                if (orderItem.unitPrice.HasValue)
                {
                    var subtotal = orderItem.quantity * orderItem.unitPrice.Value;
                    if (orderItem.tax.HasValue)
                    {
                        subtotal = (1 + orderItem.tax.Value / 100) * subtotal;
                    }
                    total += subtotal;
                }
            }

            o.total = total;
            
            // apply any discount
            if (!string.IsNullOrEmpty(couponCode))
            {
                var coupon =
                    repository.GetCoupons(subdomainid)
                    .SingleOrDefault(x => x.code == couponCode && !x.expired);
                if (coupon != null)
                {
                    if (!coupon.minimumPurchase.HasValue ||
                    (total >= coupon.minimumPurchase.Value))
                    {
                        o.discountPercentage = coupon.couponPercentage;
                        o.discountValue = coupon.couponValue;
                        o.discountCouponCode = coupon.code;
                    }
                    coupon.impressions++;
                }
            }

            if (o.discountPercentage.HasValue)
            {
                o.total = o.total * (1 - o.discountPercentage.Value / 100);
            }
            else if (o.discountValue.HasValue)
            {
                o.total = Math.Max(0, o.total - o.discountValue.Value);
            }

            // shipping cost not included in discount
            if (o.shippingCost.HasValue)
            {
                o.total = total + o.shippingCost.Value;
            }

            // add any tax
            if (o.taxAmount.HasValue)
            {
                o.total += o.taxAmount.Value;
            }

            // round it to the proper currency
            o.total = Math.Round(o.total, currency_decimalplaces);
        }

        public static TransactionType? GetQueryType(string type)
        {
            if (type == "p")
            {
                return TransactionType.ORDER;
            }

            if (type == "s")
            {
                return TransactionType.INVOICE;
            }

            return null;
        }

        public static Order ToModel(this order o, TransactionType type, long sessionid)
        {
            var order = new Order();
            order.currency = o.currency.ToCurrency(); // discount and total dependent on this being initialised first
            
            if (o.discountPercentage.HasValue)
            {
                order.discount = o.discountPercentage.Value.ToString("n2");
                order.discountType = "%";
            }
            else if (o.discountValue.HasValue)
            {
                order.discount = o.discountValue.Value.ToString("n" + order.currency.decimalCount);
            }
            order.discountCode = o.discountCouponCode;

            order.id = o.id;
            order.isOwner = o.owner == sessionid;
            order.terms = o.terms;
            order.orderStatus = o.status.ToEnum<OrderStatus>();
            order.TransactionType = type;
            order.orderNumber = o.orderNumber;
            order.orderDate = o.orderDate.ToLocalTime();
            order.location = o.inventoryLocation.HasValue ? o.inventoryLocation1.name : GeneralConstants.INVENTORY_LOCATION_DEFAULT;
            if (o.user.organisation.HasValue)
            {
                order.receiverOrgID = o.user.organisation.Value;
                order.receiverName = o.user.organisation1.name;
            }
            else
            {
                throw new Exception();
            }

            // handle tax
            order.totalTax = o.taxAmount.HasValue ? o.taxAmount.Value.ToString("n" + order.currency.decimalCount) : "";

            // handle shipping
            order.shippingCost = o.shippingCost.HasValue ? o.shippingCost.Value.ToString("n" + order.currency.decimalCount) : "";
            order.shippingMethod = o.shippingMethod;
            order.shipwireShippingID = o.shipwireShippingid;

            if (string.IsNullOrEmpty(order.shippingMethod))
            {
                if (o.allDigitalOrderItems())
                {
                    order.shippingNotes = "Digital product. Not applicable.";
                }
                else
                {
                    //order.shippingNotes = "Shipping method not specified.";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(order.shippingCost))
                {
                    order.shippingNotes = string.Format("{0}. Shipping cost has not been determined.", order.shippingMethod);
                }
                else
                {
                    order.shippingNotes = string.Format("{0}. Shipping cost is {1}{2}.", order.shippingMethod, order.currency.symbol, order.shippingCost);
                }
            }

            // handle payment
            var cart = o.carts.SingleOrDefault();
            if (cart != null)
            {
                var methodType = cart.paymentMethod.ToEnum<PaymentMethodType>();
                var method = "";
                var instructions = "";
                switch (methodType)
                {
                    case PaymentMethodType.Custom:
                        if (cart.paymentCustomId.HasValue)
                        {
                            method = cart.paymentMethod1.name;
                            instructions = cart.paymentMethod1.instructions;
                        }
                        break;
                    case PaymentMethodType.Paypal:
                        method = "Paypal";
                        break;
                    default:
                        // customer will get here when shipping method not found so no payment method specified
                        break;
                }
                order.paymentMethod = method;
                order.paymentInstructions = instructions;
            }

            // have unreviewed payment?
            if (o.payments.Any(x => x.status == PaymentStatus.New.ToString()))
            {
                order.hasUnreviewedCustomPayment = true;
            }

            // shipping costs already included in order total so there is no need to check
            order.orderTotal = o.total.ToString("n" + order.currency.decimalCount);
            order.totalPaid = o.totalPaid.ToString("n" + order.currency.decimalCount);
            order.amountDue = (o.total - o.totalPaid);
            order.orderItems = o.orderItems.ToModel(order.currency);
            order.subTotal = order.orderItems.Sum(x => x.subTotal).ToString("n" + order.currency.decimalCount);
           
            return order;
        }

        private static string GetJqGridActionString(this order row, long viewerid)
        {
            string actionString = "";
            if (row.receiverUserid == viewerid &&
                row.transactions != null &&
                row.transactions.reviewid.HasValue &&
                row.transactions.review.pending)
            {
                actionString =  string.Format(
                        "<span class='jqreview block' href='/dashboard/orders/review/{0}'>review</span>", row.id);
            }
            else
            {
                if (row.owner == viewerid && row.status == OrderStatus.DRAFT.ToString())
                {
                    actionString = string.Format(
                        "<a class='jqedit block' href='/dashboard/{0}/edit/{1}'>edit</a>",
                        row.type == TransactionType.INVOICE.ToString() ? "invoices" : "orders",
                        row.id);
                }
            }

            return actionString;
        }

        private static string ToOrderDetails(this order row, long viewerid)
        {
            string contactParameters;

            if (row.receiverUserid == viewerid)
            {
                contactParameters = row.owner.ToString();
            }
            else
            {
                contactParameters = row.receiverUserid.ToString();
            }

            if (row.receiverUserid.HasValue)
            {
                if (row.user.organisation1.subdomain != row.user1.organisation1.subdomain)
                {
                    if (row.receiverUserid.Value == viewerid)
                    {
                        contactParameters = string.Concat(contactParameters, "/", row.user1.organisation1.subdomain);
                    }
                    else
                    {
                        contactParameters = string.Concat(contactParameters, "/", row.user.organisation1.subdomain);
                    }
                }
            }
            else
            {
                return string.Format(
                    "<a href='#'>{0}</a> <br/><span class='tip_inline'>{1} {2}</span>",
                    row.receiverUserid == viewerid ? row.user1.ToFullName() : row.receiverFullName,
                    row.ebayID.HasValue? string.Format("[ebayid:{0}]", row.ebay_order.orderid):"",
                    row.orderItems.ToDescriptionString());
            }
            

            return
                string.Format(
                    "<a href='/dashboard/contacts/{0}'>{1}</a> <br/><span class='tip_inline'>{2} {3}</span>",
                    contactParameters,
                    row.receiverUserid == viewerid ? row.user1.ToFullName() : row.user.ToFullName(),
                    row.ebayID.HasValue ? string.Format("[ebayid:{0}]", row.ebay_order.orderid) : "",
                    row.orderItems.ToDescriptionString());
        }

        public static string ToOrderLink(this order value, string section = "")
        {
            if (value.type == TransactionType.INVOICE.ToString())
            {
                return string.Concat(GeneralConstants.URL_SINGLE_INVOICE, value.id, section);
            }
            return string.Concat(GeneralConstants.URL_SINGLE_ORDER, value.id, section);
        }

        public static JqgridTable ToTransactionJqGrid(this IEnumerable<order> rows, long viewerid)
        {
            var grid = new JqgridTable();
            foreach (var row in rows)
            {
                var currency = row.currency.ToCurrency();
                var sellingPrice = row.total.ToString("n" + currency.decimalCount);

                var transactionType = row.type.ToEnum<TransactionType>();
                
                var entry = new JqgridRow
                {
                    id = row.id.ToString(),
                    cell = new object[] 
                                           {
                                               string.Format("<div class='pl10'><span class='jqview' href='/dashboard/{0}/{1}'><strong>{2}</strong> #{3}</span><br/>{4}</div>", 
                                                                      transactionType == TransactionType.INVOICE?"invoices":"orders",
                                                                      row.id, 
                                                                      transactionType == TransactionType.INVOICE?"invoice":"order",
                                                                      row.orderNumber.ToString("D8"),
                                                                      row.orderDate.ToLocalTime().ToShortDateString()),
                                               row.ToOrderDetails(viewerid),
                                               currency.symbol + sellingPrice,
                                               string.Format("{0}{1}",row.GetStatusStringForGrid(row.receiverUserid == viewerid), 
                                               row.GetJqGridActionString(viewerid))
                                           }
                };
                grid.rows.Add(entry);
                grid.records++;
            }
            return grid;
        }

        public static IEnumerable<Shipwire.order.OrderItem> ToShipwireItems(this order order)
        {
            var shipwireitems = new List<Shipwire.order.OrderItem>();
            foreach (var item in order.orderItems)
            {
                var shipwireitem = new Shipwire.order.OrderItem
                                       {
                                           Sku = item.product_variant.sku, 
                                           Quantity = item.quantity
                                       };
                shipwireitems.Add(shipwireitem);
            }
            return shipwireitems;
        }

        public static transaction ToTransaction(this order value)
        {
            if (value.transactions != null)
            {
                return value.transactions;
            }
            return value.transactions1[0];
        }

        public static readonly JqgridTable EMPTY_JQGRID_TABLE = new JqgridTable
        {
            records = 0,
            page = 0,
            total = 0
        };
    }
}
