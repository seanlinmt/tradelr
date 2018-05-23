using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Shipwire;
using Shipwire.order;
using clearpixels.Models;
using tradelr.Common.Models.currency;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Lucene;
using tradelr.DBML.Models;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.message;
using tradelr.Models.payment;
using tradelr.Models.products;
using tradelr.Models.subdomain;
using tradelr.Models.users;
using Exception = System.Exception;

namespace tradelr.Models.transactions
{
    public class Transaction
    {
        private MASTERsubdomain senderDomain { get; set; }
        private user receiver { get; set; }
        private order order { get; set; }
        private ITradelrRepository repository { get; set; }
        private Currency currency { get; set; }
        private TransactionType transactionType { get; set; }
        private long caller_sessionid { get; set; }

        // for recording changes
        private Order original { get; set; }

        // shipping
        private ShipwireService shipwireService { get; set; }

        // used for creation
        public Transaction(MASTERsubdomain senderDomain, user to, TransactionType transactionType, ITradelrRepository repository, long sessionid)
        {
            this.senderDomain = senderDomain;
            receiver = to;
            this.repository = repository;
            this.transactionType = transactionType;
            caller_sessionid = sessionid;
        }

        // used for updates to existing order
        public Transaction(order o, ITradelrRepository repository, long? sessionid)
            :this(o.user1.organisation1.MASTERsubdomain, o.user, o.type.ToEnum<TransactionType>(), repository, sessionid.HasValue?sessionid.Value:o.owner)
        {
            order = o;
            currency = o.currency.ToCurrency();
            original = o.ToModel(transactionType, sessionid.HasValue ? sessionid.Value : o.owner);
        }

        public void CreateTransaction(long orderNumber, DateTime orderDate, string terms, int currencyid)
        {
            order = new order
            {
                owner = senderDomain.organisation.users.First().id,
                orderNumber = orderNumber,
                type = transactionType.ToString(),
                orderDate = orderDate,
                currency = currencyid,
                receiverUserid = receiver.id,
                receiverFullName = receiver.ToFullName(),
                receiverAddress = receiver.organisation1.ToFullOrganisationAddress(),
                terms = terms,
                status = OrderStatus.DRAFT.ToString(),
                created = DateTime.UtcNow,
                viewid = Crypto.Utility.GetRandomString(32)
            };

            currency = order.currency.ToCurrency();
        }

        public void AddComment(string comment, long? posterUserid = null, DateTime? created = null)
        {
            if (!created.HasValue)
            {
                created = DateTime.UtcNow;
            }

            var c = new comment
            {
                comments = comment,
                created = created.Value,
                creator = posterUserid
            };

            if (order.transactions == null)
            {
                // older deprecated transactions
                order.transactions1.First().comments.Add(c);
            }
            else
            {
                order.transactions.comments.Add(c);
            }
        }

        public void AddEbayOrderInformation(ebay_order ebayOrder)
        {
            order.ebay_order = ebayOrder;
        }

        public void AddOrderItem(orderItem oitem, products_digital digital)
        {
            order.orderItems.Add(oitem);

            // create digital entry
            if (digital != null)
            {
                var digitalOrderitem = new orderItems_digital();
                digitalOrderitem.downloadid = Crypto.Utility.GetRandomString();
                digitalOrderitem.downloadCount = 0;
                oitem.orderItems_digitals = digitalOrderitem;
            }
        }

        public void AddPaidAmount(decimal amount)
        {
            order.totalPaid += amount;

            if (order.totalPaid >= order.total)
            {
                UpdateOrderStatus(OrderStatus.PAID);
            }
            else
            {
                UpdateOrderStatus(OrderStatus.PARTIAL);
            }
        }

        public void AddPayment(DBML.payment payment, bool sendEmail)
        {
            // if enterrer is invoice owner then set status to accepted
            if (caller_sessionid == order.owner)
            {
                payment.status = PaymentStatus.Accepted.ToString();
            }
            else 
            {
                if (sendEmail)
                {
                    SendPaymentUpdateEmail(payment.status.ToEnum<PaymentStatus>(), true);
                }
            }

            order.payments.Add(payment);


            order.totalPaid += payment.paidAmount;

            if (order.total == order.totalPaid)
            {
                order.status = OrderStatus.PAID.ToString();
                repository.AddActivity(caller_sessionid,
                                new ActivityMessage(order.id, order.owner,
                                    ActivityMessageType.INVOICE_PAYMENT_RECEIVED_FULL,
                                     new HtmlLink(GetOrderNumber(), GetID()).ToTransactionString(transactionType)), senderDomain.id);
            }
            else
            {
                order.status = OrderStatus.PARTIAL.ToString();
                repository.AddActivity(caller_sessionid,
                                new ActivityMessage(order.id, order.owner,
                                    ActivityMessageType.INVOICE_PAYMENT_RECEIVED_PARTIAL,
                                     new HtmlLink(GetOrderNumber(), GetID()).ToTransactionString(transactionType)), senderDomain.id);
            }

            // need to save to get payment id
            repository.Save();

            // add comment
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(payment.method))
            {
                sb.AppendFormat("<p><strong>{4}</strong>: Payment #{0} of {1}{2} {3}</p>", payment.id, currency.symbol,
                                payment.paidAmount.ToString("n" + currency.decimalCount),
                                currency.code, payment.status);
            }
            else
            {
                sb.AppendFormat("<p><strong>{4}</strong>: Payment #{0} of {1}{2} {3} via {5}</p>", payment.id,
                                currency.symbol,
                                payment.paidAmount.ToString("n" + currency.decimalCount),
                                currency.code, payment.status, payment.method);
            }

            sb.AppendFormat("<p>{0}</p>", payment.notes);

            AddComment(sb.ToString(), caller_sessionid, payment.created);

            repository.Save();
        }

        public void AddPendingReview()
        {
            // create feedback entry
            order.transactions.review = new DBML.review()
            {
                created = DateTime.UtcNow,
                rating_overall = 3,
                rating_willshopagain = 3,
                rating_delivery = 3,
                rating_support = 3,
                pending = true,
                subdomainid = senderDomain.id
            };
        }

        public void AddShipwireTransaction(string transactionid, XElement message)
        {
            var shipwire_transaction = new shipwireTransaction
            {
                orderid = order.id,
                transactionid = transactionid,
                message = message,
                state = ShipwireState.ORDER_SUBMITTED.ToString()
            };
            repository.AddShipwireTransaction(shipwire_transaction); // submit
        }

        private void CopyBillingAndShippingAddressesFromReceiverToInvoice()
        {
            // handle billing address
            if (receiver.organisation1.address2 != null)
            {
                order.address1 = new DBML.address();
                repository.CopyDataMembers(receiver.organisation1.address2, order.address1);
            }
            
            
            // handle shipping address
            if (receiver.organisation1.address1 != null)
            {
                order.address = new DBML.address();
                repository.CopyDataMembers(receiver.organisation1.address1, order.address);
            }
        }

        public void DeleteOrder()
        {
            // handle order items first
            foreach (var item in GetOrderItems().ToArray())
            {
                DeleteOrderItem(item);
            }

            // handle order
            repository.DeleteOrder(order);
        }

        public void DeleteOrderItem(orderItem item)
        {
            var delta = -item.quantity;
            item.quantity = 0;  // set to 0 so can detect deletions
            UpdateInventoryItem(item, delta);

            order.orderItems.Remove(item);
        }

        public DBML.address GetBillingAddress()
        {
            return order.address1;
        }

        public Currency GetCurrency()
        {
            return currency;
        }

        public decimal GetDiscount()
        {
            if (order.discountPercentage.HasValue)
            {
                return order.total * order.discountPercentage.Value / 100;
            }

            if (order.discountValue.HasValue)
            {
                return Math.Max(0, order.total - order.discountValue.Value);
            }

            return 0;
        }

        public long GetID()
        {
            Debug.Assert(order.id != 0);

            return order.id;
        }

        public DateTime GetOrderDate()
        {
            return order.orderDate;
        }

        public IQueryable<orderItem> GetOrderItems()
        {
            return order.orderItems.AsQueryable();
        }

        public string GetOrderLink()
        {
            return order.ToOrderLink();
        }

        public string GetOrderNumber()
        {
            return order.orderNumber.ToString("D8");
        }

        public OrderStatus GetOrderStatus()
        {
            return order.status.ToEnum<OrderStatus>();
        }

        public user GetOwner()
        {
            return order.user1;
        }

        public IEnumerable<DBML.payment> GetPayments()
        {
            return order.payments;
        }

        public user GetReceiver()
        {
            return order.user;
        }

        public DBML.address GetShippingAddress()
        {
            return order.address;
        }

        public decimal GetShippingCost()
        {
            return order.shippingCost ?? 0;
        }

        public AddressInfo GetShipwireShippingAddress()
        {
            var addr = GetShippingAddress();

            return new AddressInfo(addr.first_name + " " + addr.last_name, addr.street_address, addr.city,
                                   addr.state, addr.country.HasValue ? Country.GetCountry(addr.country.Value).name : "",
                                   addr.postcode, addr.phone, order.user.email);
        }

        public ShipwireService GetShipWireService()
        {
            if (shipwireService != null)
            {
                return shipwireService;
            }
            var cryptor = new AESCrypt();
            // try submit order to shipwire
            shipwireService = new ShipwireService(senderDomain.shipwireEmail, cryptor.Decrypt(senderDomain.shipwirePassword, senderDomain.id.ToString()));
            return shipwireService;
        }

        public string GetShipwireTransactionID()
        {
            return order.shipwireTransaction.transactionid;
        }

        public decimal GetSubTotal()
        {
            decimal total = 0;
            foreach (var orderItem in order.orderItems)
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
            return total;
        }

        public decimal GetTotal()
        {
            return order.total;
        }
        
        public decimal GetTotalPaid()
        {
            return order.totalPaid;
        }

        public new TransactionType GetType()
        {
            return transactionType;
        }

        public bool HasDigitalOrderItems()
        {
            return order.hasDigitalOrderItem();
        }

        public bool HasShippingMethod()
        {
            return !string.IsNullOrEmpty(order.shippingMethod);
        }

        public bool HasValidShippingAddress()
        {
            var shippingAddress = GetShippingAddress();
            if (shippingAddress == null ||
                string.IsNullOrEmpty(shippingAddress.street_address) ||
                string.IsNullOrEmpty(shippingAddress.city) ||
                !shippingAddress.country.HasValue)
            {
                return false;
            }
            return true;
        }

        public void MarkReceived(DateTime datetime)
        {
            UpdateOrderStatus(OrderStatus.SHIPPED);

            // update status
            order.actualReceivedDate = datetime;

            // update inventory
            foreach (var item in GetOrderItems())
            {
                var quantity = item.quantity;
                UpdateInventoryItem(item, quantity);
            }

            // add comment
            AddComment("Order received.");
        }

        public void UpdateDiscount(string type, string discount)
        {
            if (string.IsNullOrEmpty(discount))
            {
                order.discountPercentage = null;
                order.discountValue = null;
            }
            else
            {
                if (type == "%")
                {
                    order.discountPercentage = decimal.Parse(discount);
                    order.discountValue = null;
                }
                else
                {
                    order.discountPercentage = null;
                    order.discountValue = decimal.Parse(discount);
                }
            }
        }

        public void UpdateOrderStatus(OrderStatus status)
        {
            order.status = status.ToString();
        }

        public void UpdateOrderAsShipped(string shippingService, string trackingno, string trackingSite = "")
        {
            // notify buyer that order has been shipped
            repository.AddActivity(GetReceiver().id,
                               new ActivityMessage(GetID(), GetOwner().id,
                                           ActivityMessageType.ORDER_SHIPPED,
                                           new HtmlLink(GetOrderNumber(), GetID()).ToTransactionString(GetType())), senderDomain.id);


            UpdateOrderStatus(OrderStatus.SHIPPED);

            // add review
            AddPendingReview();

            // add comment
            var service = "Unavailable";
            if (!string.IsNullOrEmpty(shippingService))
            {
                service = shippingService;
            }

            var trackingNumber = "Unavailable";
            if (!string.IsNullOrEmpty(trackingno))
            {
                trackingNumber = trackingno;
            }

            string comment;
            if (!string.IsNullOrEmpty(trackingSite))
            {
                string trackingInfoAddress = string.Format("<a href='{0}' target='_blank'>{1}</a>", trackingSite,
                                                    trackingSite);
                comment = string.Format(OrderComment.ORDER_SHIP_DETAILED, service, trackingNumber, trackingInfoAddress);
            }
            else
            {
                comment = string.Format(OrderComment.ORDER_SHIP_STANDARD, service, trackingNumber);
            }

            AddComment(comment);

            SaveUpdatedTransaction();
        }

        public void UpdateOrderNumber(long ordernumber)
        {
            // TODO: check that it is not in use
            order.orderNumber = ordernumber;
        }

        // this should be executed after all order items have been filled
        public void UpdateOrderTax(decimal taxpercent, bool includeShippingCostInCalculation, string coupon = "")
        {
            if (taxpercent == 0)
            {
                order.taxAmount = 0;
            }
            else
            {
                if (includeShippingCostInCalculation)
                {
                    order.taxAmount = (taxpercent / 100) * (GetSubTotal() + GetShippingCost());
                }
                else
                {
                    order.taxAmount = (taxpercent / 100) * GetSubTotal();
                }
            }

            // update total
            UpdateTotal(coupon);
        }

        public void UpdatePaymentStatus(DBML.payment pay, PaymentStatus status)
        {
            pay.status = status.ToString();

            switch (status)
            {
                case PaymentStatus.Accepted:
                    // add activity
                    repository.AddActivity(pay.order.owner,
                                                   new ActivityMessage(order.id, order.receiverUserid,
                                                                       ActivityMessageType.INVOICE_PAYMENT_ACCEPTED,
                                                                       new HtmlLink(GetOrderNumber(),
                                                                                    order.id).ToTransactionString(transactionType)),
                                                   senderDomain.id);
                    if (HasDigitalOrderItems())
                    {
                        // send download links
                        SendDownloadLinksEmail();
                    }
                    break;
                case PaymentStatus.New:
                    break;
                case PaymentStatus.Chargeable:
                    break;
                case PaymentStatus.Charging:
                    break;
                case PaymentStatus.Declined:
                    // update order/invoice total
                    order.totalPaid -= pay.paidAmount;

                    UpdateTotal();

                    repository.AddActivity(pay.order.owner,
                                           new ActivityMessage(order.id, order.receiverUserid,
                                                               ActivityMessageType.INVOICE_PAYMENT_REJECTED,
                                                               new HtmlLink(GetOrderNumber(), order.id).
                                                                   ToTransactionString(transactionType)), senderDomain.id);
                    break;
                case PaymentStatus.Cancelled:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }

            // add comment
            var comment = string.Format("<p><strong>{4}</strong>: Payment #{0} of {1}{2} {3}</p>", pay.id, currency.symbol,
                                pay.paidAmount.ToString("n" + currency.decimalCount),
                                currency.code, status);

            AddComment(comment, caller_sessionid);

            SendPaymentUpdateEmail(status, status == PaymentStatus.Declined);

            SaveUpdatedTransaction();
        }
        
        public void UpdateShippingCost(string shippingCost)
        {
            if (string.IsNullOrEmpty(shippingCost))
            {
                order.shippingCost = null;
            }
            else
            {
                order.shippingCost = decimal.Parse(shippingCost);
            }
        }

        public void UpdateShippingMethod(string shippingMethod, string shipWireID = "")
        {
            order.shippingMethod = shippingMethod;
            order.shipwireShippingid = shipWireID;
        }

        public void UpdateShipwireState(ShipwireState state)
        {
            order.shipwireTransaction.state = state.ToString();
        }

        public void UpdateInventoryItem(orderItem orderItem, int inventoryDelta)
        {
            // no need for updates if delta is zero
            if (inventoryDelta == 0)
            {
                return;
            }

            inventoryLocation location;
            // throws null if we just check on value. happens when creating a new order
            if (orderItem.order.inventoryLocation1 != null)
            {
                location = orderItem.order.inventoryLocation1;
            }
            else
            {
                if (orderItem.order.inventoryLocation.HasValue)
                {
                    location = repository.GetInventoryLocation(orderItem.order.inventoryLocation.Value, senderDomain.id);
                }
                else
                {
                    location = repository.GetInventoryLocation(GeneralConstants.INVENTORY_LOCATION_DEFAULT, senderDomain.id);
                }
            }

            inventoryLocationItem locationItem;
            bool trackInventory;
            bool isDigital;
            if (orderItem.product_variant == null)
            {
                var variant = repository.GetProductVariant(orderItem.variantid, senderDomain.id);
                locationItem = variant.inventoryLocationItems.SingleOrDefault(x => x.locationid == location.id);
                trackInventory = variant.product.trackInventory;
                isDigital = variant.IsDigital();
            }
            else
            {
                locationItem = orderItem.product_variant.inventoryLocationItems.SingleOrDefault(x => x.locationid == location.id);
                trackInventory = orderItem.product_variant.product.trackInventory;
                isDigital = orderItem.product_variant.IsDigital();
            }

            if (locationItem == null)
            {
                locationItem = new inventoryLocationItem
                {
                    locationid = location.id,
                    lastUpdate = DateTime.UtcNow
                };
                if (orderItem.product_variant == null)
                {
                    var variant = repository.GetProductVariant(orderItem.variantid, senderDomain.id);
                    variant.inventoryLocationItems.Add(locationItem);
                }
                else
                {
                    orderItem.product_variant.inventoryLocationItems.Add(locationItem);
                }
            }

            var isNewEntry = orderItem.id == 0;
            var isDelete = orderItem.quantity == 0;
            var invWorker = new InventoryWorker(locationItem, senderDomain.id, trackInventory, isDigital);
            var description = order.ToHtmlLink();

            switch (GetOrderStatus())
            {
                case OrderStatus.DRAFT:
                    if (isNewEntry)
                    {
                        description += " created";
                    }
                    else if (isDelete)
                    {
                        description += string.Format(": {0} deleted", orderItem.product_variant.ToHtmlLink());
                    }
                    else
                    {
                        description += " updated";
                    }

                    if (transactionType == TransactionType.INVOICE)
                    {
                        invWorker.SetValues(description,
                                            -inventoryDelta, // available
                                            null, // on order
                                            inventoryDelta, // reserved
                                            null); // sold
                    }
                    else
                    {
                        invWorker.SetValues(description,
                                    null, // available
                                    inventoryDelta, // on order
                                    null, // reserved
                                    null); // sold
                    }
                    break;
                case OrderStatus.SENT:
                    if (transactionType == TransactionType.INVOICE)
                    {
                        if (isNewEntry)
                        {
                            invWorker.SetValues(order.ToHtmlLink() + " created",
                                                    -inventoryDelta, // available
                                                    null, // on order
                                                    inventoryDelta, // reserved
                                                    null); // sold
                        }
                    }
                    else
                    {
                        if (isNewEntry)
                        {
                            invWorker.SetValues(order.ToHtmlLink() + " created",
                                                    null, // available
                                                    inventoryDelta, // on order
                                                    null, // reserved
                                                    null); // sold
                        }
                    }
                    break;
                case OrderStatus.VIEWED:
                    break;
                case OrderStatus.PARTIAL:
                    break;
                case OrderStatus.PAID:
                    if (transactionType == TransactionType.INVOICE)
                    {
                        if (isNewEntry)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            invWorker.SetValues(order.ToHtmlLink() + " paid",
                                                    null, // available
                                                    null, // on order
                                                    -inventoryDelta, // reserved
                                                    inventoryDelta); // sold
                        }
                    }
                    break;
                case OrderStatus.SHIPPED:
                    if (transactionType == TransactionType.ORDER)
                    {
                        if (isNewEntry)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            invWorker.SetValues(order.ToHtmlLink() + " received",
                                                    inventoryDelta, // available
                                                    -inventoryDelta, // on order
                                                    null, // reserved
                                                    null); // sold
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("status");
            }

        }

        public void UpdateInventoryLocation(long? location)
        {
            if (location.HasValue)
            {
                order.inventoryLocation = location.Value;
            }
        }

        public void UpdateTerms(string term)
        {
            order.terms = term;
        }

        public void UpdateTotal(string couponCode = "")
        {
            order.UpdateOrderTotal(senderDomain.id, currency.decimalCount, couponCode, repository);
        }

        public void SaveUpdatedTransaction()
        {
            // needs an update here to update variant link in orderitem
            repository.Save();

            // save change history
            var changed = order.ToModel(transactionType, caller_sessionid);
            var comparer = new CompareObject();
            var diff = comparer.Compare(original, changed, currency);
            if (diff.Count != 0)
            {
                repository.AddChangeHistory(
                    caller_sessionid,
                    order.id,
                    transactionType == TransactionType.ORDER ? ChangeHistoryType.ORDERS : ChangeHistoryType.INVOICE,
                    diff
                    );
            }

            repository.UpdateProductsOutOfStock(senderDomain.id);
            order.lastUpdate = DateTime.UtcNow;
            repository.Save();
#if LUCENE
            // index order
            var indexer = new LuceneWorker(repository, new IdName(senderDomain.id, senderDomain.name));
            indexer.AddToIndex(LuceneIndexType.TRANSACTION, order);
#endif
        }

        public void SaveNewTransaction()
        {
            // copy addresses
            CopyBillingAndShippingAddressesFromReceiverToInvoice();

            order.transactions = new transaction();

            repository.AddOrder(order);

            repository.UpdateProductsOutOfStock(senderDomain.id);

            repository.AddChangeHistory(caller_sessionid, order.id, transactionType == TransactionType.INVOICE ? ChangeHistoryType.INVOICE : ChangeHistoryType.ORDERS, null);
            repository.UpdateCounters(senderDomain.id, 1, transactionType == TransactionType.INVOICE ? CounterType.INVOICES_SENT : CounterType.ORDERS_SENT);

#if LUCENE
            var indexer = new LuceneWorker(repository, new IdName(senderDomain.id, senderDomain.name));
            indexer.AddToIndex(LuceneIndexType.TRANSACTION, order);
#endif
        }

        private void SendPaymentUpdateEmail(PaymentStatus status, bool informOwner = false)
        {
            if (receiver == null)
            {
                return;
            }

            var viewdata = new ViewDataDictionary()
                                                   {
                                                       {"type", GetType()},
                                                       {"orderNumber", GetOrderNumber()},
                                                       {"status", status.ToDisplayString()},
                                                       {"viewloc", senderDomain.ToHostName().ToDomainUrl(GetOrderLink())}
                                                   };

            string subject = string.Format("{0} #{1}: Payment {2}", GetType(), GetOrderNumber(), status);
            var sender = order.user1;
            EmailHelper.SendEmailNow(EmailViewType.PAYMENT_STATUS_CHANGE, viewdata, subject,
                                     receiver.GetEmailAddress(), receiver.ToFullName(),
                                     sender);

            if (informOwner)
            {
                // inform sender
                viewdata["viewloc"] = order.user1.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl(order.ToOrderLink());
                EmailHelper.SendEmailNow(EmailViewType.PAYMENT_STATUS_CHANGE, viewdata, subject,
                                         sender.GetEmailAddress(), sender.ToFullName(), receiver);
            }
        }

        public void SendDownloadLinksEmail()
        {
            if (receiver == null)
            {
                return;
            }
            var viewdata = new ViewDataDictionary()
                                                   {
                                                       {"type", GetType()},
                                                       {"orderNumber", GetOrderNumber()},
                                                       {"status", PaymentStatus.Accepted},
                                                       {"viewloc", senderDomain.ToHostName().ToDomainUrl(GetOrderLink())}
                                                   };

            viewdata["links"] = GetOrderItems()
                .Where(x => x.orderItems_digitals != null)
                .Select(
                    x =>
                    (string.Format("{0}. <a href='http://z.tradelr.com/d/{1}'>download link</a>", x.description, x.orderItems_digitals.downloadid)
                     +
                     (x.product_variant.product.products_digitals.expiryDate.HasValue
                          ? string.Format(" expires {0}",
                                          x.product_variant.product.products_digitals.expiryDate.Value.
                                              ToShortDateString())
                          : "")
                    ));

            string subject = string.Format("{0} #{1}: Payment {2}", GetType(), GetOrderNumber(), PaymentStatus.Accepted);
            var sender = order.user1;
            EmailHelper.SendEmailNow(EmailViewType.ORDER_DOWNLOADLINKS, viewdata, subject,
                                     receiver.GetEmailAddress(), receiver.ToFullName(),
                                     sender);
        }


        public void UpdateCurrency(int currencyid)
        {
            order.currency = currencyid;
        }

    }

    public static class TransactionHelper
    {
        public static string ToTransactionName(this transaction value)
        {
            var primary = value.order;
            if (primary.type == TransactionType.INVOICE.ToString())
            {
                return string.Format("Invoice #{0}", primary.orderNumber.ToString("D8"));
            }
            return string.Format("Order #{0}", primary.orderNumber.ToString("D8"));
        }

        public static string ToTransactionLink(this transaction value)
        {
            return value.order.ToOrderLink();
        }
    }
}