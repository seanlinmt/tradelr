using System;
using System.Linq;
using Ebay;
using Ebay.Resources;
using eBay.Service.Core.Sdk;
using eBay.Service.Core.Soap;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library;
using clearpixels.Logging;
using tradelr.Library.geo;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.export.ebay;
using tradelr.Models.networks;
using tradelr.Models.payment;
using tradelr.Models.products;
using tradelr.Models.transactions;
using tradelr.Models.users;
using TransactionType = tradelr.Models.transactions.TransactionType;

namespace tradelr.Models.ebay
{
    public class EbayWorker
    {
        private MASTERsubdomain sd;
        private string token;
        private user seller;

        public EbayWorker(MASTERsubdomain sd, string token)
        {
            this.sd = sd;
            this.token = token;
            seller = sd.organisation.users.First();
        }
        public void PollForEbayOrders()
        {
            var service = new OrderService(token);
            try
            {
                // get completed orders from a month before
                var completed = service.GetOrders(OrderStatusCodeType.Completed, DateTime.Now.AddMonths(-1),
                                                    DateTime.Now);

                SaveEbayOrders(completed);

                // exclude active orders for now because buyer email is not shown
                /*
                // get all orders from last day
                var all = service.GetOrders(OrderStatusCodeType.All, DateTime.Now.AddDays(-1), DateTime.Now);

                SaveEbayOrders(all);
                 * */
            }
            catch (ApiException ex)
            {
                if (ex.Message.Contains("Validation of the authentication token in API request failed"))
                {
                    // delete auth token
                    var ebay = new NetworksEbay(sd.id);
                    ebay.ClearSynchronisation();
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                Syslog.Write(service.responseXML);
            }
        }


        private void SaveEbayOrders(OrderTypeCollection collection)
        {
            using (var repository = new TradelrRepository())
            {
                foreach (OrderType entry in collection)
                {
                    Transaction transaction;
                    // check if order already exists
                    var existingEbayOrder = repository.GetSubDomain(sd.id).ebay_orders.SingleOrDefault(x => x.orderid == entry.OrderID);

                    if (existingEbayOrder != null)
                    {
                        var order = existingEbayOrder.orders.Single();
                        transaction = new Transaction(order, repository, seller.id);

                        // update order status
                        existingEbayOrder.status = entry.OrderStatus.ToString();
                    }
                    else
                    {
                        // check if user already exists
                        var buyer = repository.GetUserByEbayID(entry.BuyerUserID);

                        if (buyer == null)
                        {
                            // get receiver and add to contact db
                            var userService = new UserService(token);

                            // get by itemid as invalid request seems to be returned when get by userid
                            var ebaybuyer = userService.GetUser(entry.BuyerUserID);

                            // we assume that same buyer for all transactions so we get the first email address
                            var buyeremail = entry.TransactionArray.ItemAt(0).Buyer.Email;

                            buyer = SaveEbayBuyer(ebaybuyer, buyeremail);
                        }

                        // add a shipping and billing address
                        if (entry.ShippingAddress != null)
                        {
                            var buyername = Utility.SplitFullName(entry.ShippingAddress.Name);
                            var handler = new AddressHandler(buyer.organisation1, repository);
                            handler.SetShippingAndBillingAddresses(buyername[0],
                                                                   buyername[1],
                                                                   entry.ShippingAddress.CompanyName ?? "",
                                                                   entry.ShippingAddress.Street1 + "\r\n" + entry.ShippingAddress.Street2,
                                                                   entry.ShippingAddress.CityName,
                                                                   null,
                                                                   entry.ShippingAddress.PostalCode,
                                                                   entry.ShippingAddress.Phone,
                                                                   entry.ShippingAddress.Country.ToString().ToCountry().id,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   buyername[0],
                                                                   buyername[1],
                                                                   entry.ShippingAddress.CompanyName ?? "",
                                                                   entry.ShippingAddress.Street1 + "\r\n" + entry.ShippingAddress.Street2,
                                                                   entry.ShippingAddress.CityName,
                                                                   null,
                                                                   entry.ShippingAddress.PostalCode,
                                                                   entry.ShippingAddress.Phone,
                                                                   entry.ShippingAddress.Country.ToString().ToCountry().id,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   entry.ShippingAddress.StateOrProvince,
                                                                   true);
                        }

                        // add normal order
                        transaction = new Transaction(sd, buyer, TransactionType.INVOICE, repository, seller.id);

                        transaction.CreateTransaction(repository.GetNewOrderNumber(sd.id, TransactionType.INVOICE),
                                                      entry.CreatedTime, "",
                                                      entry.AmountPaid.currencyID.ToString().ToCurrency().id);

                        // mark as sent
                        var tradelr_orderstatus = GetOrderStatus(entry.OrderStatus);
                        transaction.UpdateOrderStatus(tradelr_orderstatus);

                        // add ebay specific order information
                        var newEbayOrder = new ebay_order();
                        newEbayOrder.orderid = entry.OrderID;
                        newEbayOrder.status = entry.OrderStatus.ToString();
                        newEbayOrder.created = entry.CreatedTime;
                        newEbayOrder.subdomainid = sd.id;
                        transaction.AddEbayOrderInformation(newEbayOrder);

                        foreach (eBay.Service.Core.Soap.TransactionType trans in entry.TransactionArray)
                        {
                            var ebay_itemid = trans.Item.ItemID;

                            // get product details
                            var itemservice = new ItemService(token);
                            var item = itemservice.GetItem(ebay_itemid);

                            // add new product if necessary
                            var existingproduct = repository.GetProducts(sd.id).SingleOrDefault(x => x.ebayID.HasValue && x.ebay_product.ebayid == ebay_itemid);
                            if (existingproduct == null)
                            {
                                // add new product  (triggered when synchronisation is carried out the first time)
                                var newproduct = new Listing();
                                newproduct.Populate(item);
                                var importer = new ProductImport();
                                var pinfo = importer.ImportEbay(newproduct, sd.id);

                                repository.AddProduct(pinfo, sd.id);
                                existingproduct = pinfo.p;
                            }
                            else
                            {
                                // if existing product is completed then we need to relist
                                if (entry.OrderStatus == OrderStatusCodeType.Completed ||
                                    entry.OrderStatus == OrderStatusCodeType.Shipped)
                                {
                                    // see if product listing is still active
                                    if (item.SellingStatus.ListingStatus == ListingStatusCodeType.Completed ||
                                        item.SellingStatus.ListingStatus == ListingStatusCodeType.Ended)
                                    {
                                        // set status to inactive
                                        existingproduct.ebay_product.isActive = false;

                                        // check if we should autorelist
                                        if (existingproduct.ebay_product.autorelist)
                                        {
                                            // check that product has enough stock
                                            if (existingproduct.HasStock(existingproduct.ebay_product.quantity))
                                            {
                                                var exporter =
                                                        new EbayExporter(
                                                            existingproduct.ebay_product.siteid.ToEnum<SiteCodeType>(),
                                                            sd.ToHostName(),
                                                            token,
                                                            sd);

                                                exporter.BuildItem(existingproduct.ebay_product);
                                            }
                                        }
                                    }
                                }
                            }

                            // add tradelr order item
                            var orderItem = new orderItem
                            {
                                description = item.Title,
                                variantid = existingproduct.product_variants[0].id,
                                unitPrice = (decimal)trans.TransactionPrice.Value,
                                quantity = trans.QuantityPurchased
                            };

                            if (trans.Taxes != null)
                            {
                                orderItem.tax =
                                    (decimal) (trans.Taxes.TotalTaxAmount.Value/trans.TransactionPrice.Value);
                            }
                            
                            transaction.AddOrderItem(orderItem, null);

                            // update inventory
                            transaction.UpdateInventoryItem(orderItem, trans.QuantityPurchased);

                            // add ebay order item
                            var ebayorderitem = new ebay_orderitem();
                            ebayorderitem.lineid = trans.OrderLineItemID;
                            newEbayOrder.ebay_orderitems.Add(ebayorderitem);
                        }

                        // update shipping
                        transaction.UpdateShippingCost(entry.ShippingServiceSelected.ShippingServiceCost.Value.ToString());
                        transaction.UpdateShippingMethod(entry.ShippingServiceSelected.ShippingService);

                        // update tax : ebay tax is the shipping tax which applies to the entire order total
                        // may or may not include shipping cost
                        if (entry.ShippingDetails.SalesTax != null)
                        {
                            transaction.UpdateOrderTax((decimal)entry.ShippingDetails.SalesTax.SalesTaxPercent,
                                                       entry.ShippingDetails.SalesTax.ShippingIncludedInTax);
                        }
                        

                        transaction.UpdateTotal();
                        transaction.SaveNewTransaction(); ////////////////////// SAVE INVOICE
                    }

                    // the following applies to both new and existing order
                    var existingPayment = transaction.GetPayments().SingleOrDefault(x => x.reference == entry.OrderID);
                    if (existingPayment != null)
                    {
                        var newstatus = GetPaymentStatus(entry.CheckoutStatus.Status);
                        if (existingPayment.status != newstatus.ToString())
                        {
                            transaction.UpdatePaymentStatus(existingPayment, newstatus);
                        }
                    }
                    else
                    {
                        // if payment has been made then add payment
                        if (entry.CheckoutStatus.Status == CompleteStatusCodeType.Complete)
                        {
                            var p = new DBML.payment();
                            p.status = GetPaymentStatus(entry.CheckoutStatus.Status).ToString();
                            p.method = entry.CheckoutStatus.PaymentMethod.ToString();
                            p.created = entry.CheckoutStatus.LastModifiedTime;
                            p.notes = entry.BuyerCheckoutMessage;
                            p.paidAmount = (decimal)entry.AmountPaid.Value;
                            p.reference = entry.OrderID;

                            transaction.AddPayment(p, false);
                        }
                    }

                    // if there is a shipped date, mark as ship if not already done so
                    if (transaction.GetOrderStatus() != OrderStatus.SHIPPED &&
                        entry.ShippedTimeSpecified)
                    {
                        transaction.UpdateOrderStatus(OrderStatus.SHIPPED);

                        if (entry.ShippingDetails.ShipmentTrackingDetails.Count != 0)
                        {
                            foreach (ShipmentTrackingDetailsType trackentry in entry.ShippingDetails.ShipmentTrackingDetails)
                            {
                                var comment = string.Format(OrderComment.ORDER_SHIP_STANDARD,
                                                            trackentry.ShippingCarrierUsed,
                                                            trackentry.ShipmentTrackingNumber);
                                transaction.AddComment(comment);
                            }
                        }
                        else
                        {
                            transaction.AddComment(OrderComment.ORDER_SHIP, created: entry.ShippedTime);
                        }
                    }
                    repository.Save();  // save per order
                }
            }
        }

        // http://developer.ebay.com/devzone/xml/docs/Reference/eBay/types/CompleteStatusCodeType.html
        private PaymentStatus GetPaymentStatus(CompleteStatusCodeType code)
        {
            PaymentStatus status;
            switch (code)
            {
                case CompleteStatusCodeType.Incomplete: 
                    status = PaymentStatus.New;
                    break;
                case CompleteStatusCodeType.Complete:
                    status = PaymentStatus.Accepted;
                    break;
                case CompleteStatusCodeType.Pending:
                    status = PaymentStatus.Charging;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("code");
            }

            return status;
        }

        private OrderStatus GetOrderStatus(OrderStatusCodeType code)
        {
            OrderStatus status = OrderStatus.DRAFT;
            switch (code)
            {
                case OrderStatusCodeType.Active:
                case OrderStatusCodeType.Authenticated:
                case OrderStatusCodeType.InProcess:
                    status = OrderStatus.VIEWED;
                    break;
                case OrderStatusCodeType.Inactive:
                    status = OrderStatus.DRAFT;
                    break;
                case OrderStatusCodeType.Completed:
                case OrderStatusCodeType.Shipped:
                    status = OrderStatus.SHIPPED;
                    break;
                case OrderStatusCodeType.Cancelled:
                case OrderStatusCodeType.Default:
                case OrderStatusCodeType.Invalid:
                case OrderStatusCodeType.CustomCode:
                case OrderStatusCodeType.All:
                    throw new ArgumentOutOfRangeException("code");
            }
            return status;
        }

        private user SaveEbayBuyer(UserType buyer, string buyeremail)
        {
            // check if buyer already exists
            using (var repository = new TradelrRepository())
            {
                var newuser = new user()
                {
                    role = UserRole.USER.ToInt(),
                    email = buyeremail.Contains("@")?buyeremail:"",  // this might be invalid request
                    firstName = buyer.UserID,
                    lastName = "",
                    viewid = Crypto.Utility.GetRandomString(),
                    permissions = (int)UserPermission.USER,
                    organisation1 = new organisation
                    {
                        subdomain = sd.id,
                        name = buyer.UserID
                    },
                    ebay_user = new ebay_user
                    {
                        ebayuserid = buyer.UserID,
                        feedback = buyer.FeedbackScore
                    }
                };

                repository.AddUser(newuser);
                return newuser;
            }
        }
    }
}