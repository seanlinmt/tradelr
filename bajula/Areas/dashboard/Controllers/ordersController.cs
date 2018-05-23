using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using clearpixels.OAuth;
using Ebay;
using eBay.Service.Core.Soap;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.contacts;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.message;
using tradelr.Models.products;
using tradelr.Models.review;
using tradelr.Models.transactions;
using tradelr.Models.transactions.viewmodel;
using tradelr.Models.users;
using TransactionType = tradelr.Models.transactions.TransactionType;

namespace tradelr.Areas.dashboard.Controllers
{
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class ordersController : baseController
    {
        [HttpGet]
        [PermissionFilter(permission = UserPermission.ORDERS_ADD)]
        [NoCache]
        public ActionResult Add()
        {
            if (MASTERdomain.trialExpired)
            {
                return RedirectToAction("TrialExpired", "Error", new { Area = "" });
            }
            var viewmodel = new OrderViewModel(baseviewmodel)
                                {
                                    ContactTypes = typeof (ContactType).ToSelectList(false,null,null,ContactType.PRIVATE.ToInt().ToString())
                                };
            var orderNumber = repository.GetNewOrderNumber(subdomainid.Value, TransactionType.ORDER);
            var org = MASTERdomain.organisation;
            viewmodel.o.CreateEmptyOrder(orderNumber, org, TransactionType.ORDER);

            var allContacts = repository.GetAllOrganisationExceptOwn(subdomainid.Value, org.id).ToModelWithCurrency().ToArray();

            viewmodel.ContactList = allContacts
                                        .OrderBy(x => x.companyName)
                                        .Select(x => new SelectListItem { Text = x.companyName, Value = x.id.ToString() })
                                        .ToSelectList(null, "Select ...", "");


            viewmodel.currencyList = CurrencyHelper.GetCurrencies().Select(x => new SelectListItem()
                                                                                    {
                                                                                        Text = x.name,
                                                                                        Value = x.id.ToString(),
                                                                                        Selected = x.id == MASTERdomain.currency
                                                                                    });

            var serializer = new JavaScriptSerializer();

            viewmodel.CurrencyInfo =
                serializer.Serialize(allContacts.Select(x => new
                                                                    {
                                                                        x.id, 
                                                                        currencyid = x.currency
                                                                    }));

            viewmodel.locationList =
                MASTERdomain.inventoryLocations.Select(
                    x => new SelectListItem()
                    {
                        Text = x.name,
                        Value = x.id.ToString(),
                        Selected = x.name == GeneralConstants.INVENTORY_LOCATION_DEFAULT
                    });

            return View(viewmodel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">order number</param>
        /// <returns></returns>
        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult Delete(long id)
        {
            // is order in use
            var order = repository.GetOrder(subdomainid.Value, id);
            if (order == null)
            {
                return SendJsonErrorResponse("Order not found");
            }

            var transaction = new Transaction(order, repository, sessionid.Value);
            var status = transaction.GetOrderStatus();

            if (status != OrderStatus.DRAFT && status != OrderStatus.SENT)
            {
                return SendJsonErrorResponse("Unable to delete. Order has already been viewed.");
            }

            try
            {
#if LUCENE
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.DeleteFromIndex(LuceneIndexType.TRANSACTION, id);
#endif
                repository.UpdateCounters(subdomainid.Value, -1, CounterType.ORDERS_SENT);

                transaction.DeleteOrder();

            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(id.ToJsonOKData());
        }

        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        [NoCache]
        [HttpGet]
        public ActionResult Edit(long id)
        {

            var o = repository.GetOrder(subdomainid.Value, id);
            if (o == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            var viewmodel = new OrderViewModel(baseviewmodel, o, sessionid.Value, TransactionType.ORDER)
                                {
                                    ContactTypes =
                                        typeof (ContactType).ToSelectList(true, null, null,
                                                                          (o.user1.organisation1.subdomain !=
                                                                           o.user.organisation1.subdomain ||
                                                                           !o.receiverUserid.HasValue)
                                                                              ? ContactType.NETWORK.ToString()
                                                                              : ContactType.PRIVATE.ToString()),
                                    Addresses =
                                        {
                                            billing = o.address1.ToModel(),
                                            shipping = o.address.ToModel(),
                                            sameBillingAndShipping = false
                                        }
                                };

            // get addresses
            var allContacts = repository.GetAllOrganisationExceptOwn(subdomainid.Value, MASTERdomain.organisation.id).ToModelWithCurrency();

            viewmodel.ChangeItems = repository.GetChangeHistory(ChangeHistoryType.ORDERS, id, subdomainid.Value).OrderByDescending(x => x.changeDate).ToChangeModel();

            viewmodel.ContactList = allContacts
                .Select(x => new SelectListItem { Text = x.companyName, Value = x.id })
                .ToSelectList(viewmodel.o.receiverOrgID, "Select...", "");

            viewmodel.currencyList = CurrencyHelper.GetCurrencies().Select(x => new SelectListItem()
            {
                Text = x.name,
                Value = x.id.ToString(),
                Selected = x.id == viewmodel.o.currency.id
            });

            var serializer = new JavaScriptSerializer();

            viewmodel.CurrencyInfo = serializer.Serialize(allContacts.Select(x => new
            {
                x.id,
                currencyid = x.currency
            }));

            viewmodel.locationList =
                MASTERdomain.inventoryLocations.Select(
                    x => new SelectListItem()
                    {
                        Text = x.name,
                        Value = x.id.ToString(),
                        Selected = x.name == viewmodel.o.location
                    });

            return View("Add", viewmodel);
        }

        [NoCache]
        [HttpGet]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult EditShipping(long id)
        {
            var order = repository.GetOrder(subdomainid.Value, id);
            var viewmodel = order.ToShippingCostModel();
            return View(viewmodel);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult EditShipping(long id, string method, string cost)
        {
            var transaction = new Transaction(repository.GetOrder(subdomainid.Value, id), repository, sessionid.Value);
            transaction.UpdateShippingCost(cost);
            transaction.UpdateShippingMethod(method);
            transaction.UpdateTotal();
            transaction.AddComment("Shipping cost and method updated.", sessionid.Value);
            transaction.SaveUpdatedTransaction();

            repository.AddActivity(transaction.GetOwner().id,
                new ActivityMessage(transaction.GetID(), transaction.GetReceiver().id,
                            ActivityMessageType.ORDER_SHIPPED,
                            new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(transaction.GetType())), subdomainid.Value);


            var data = new ChangeHistory
            {
                documentType = (transaction.GetType() == TransactionType.ORDER? ChangeHistoryType.ORDERS : ChangeHistoryType.INVOICE).ToDocumentType(),
                documentName = transaction.GetOrderNumber(),
                documentLoc = accountHostname.ToDomainUrl(transaction.GetOrderLink())
            };
            var message = new Message(transaction.GetReceiver(), transaction.GetOwner(), subdomainid.Value);
            message.SendMessage(this, repository, EmailViewType.INVOICEORDER_CHANGED, data,
                                string.Format("{0} #{1} Updated", data.documentType, transaction.GetOrderNumber()), data.documentLoc);

            return Json("Shipping cost updated.".ToJsonOKMessage());
        }

        [PermissionFilter(permission = UserPermission.TRANSACTION_SEND)]
        [HttpGet]
        public ActionResult Email(long id, string t)
        {
            var data = new Email.Email();
            var me = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var orderType = OrderHelper.GetQueryType(t);

            var order = repository.GetOrder(subdomainid.Value, id);
            if (string.IsNullOrEmpty(order.viewid))
            {
                repository.UpdateOrderViewID(order);
            }

            if (!order.receiverUserid.HasValue)
            {
                throw new NotImplementedException();
            }

            // set new url location
            data.orderID = order.id;
            data.orderType = t;
            data.receiver = order.user.email;
            if (!string.IsNullOrEmpty(order.user.firstName))
            {
                var name = order.user.firstName;
                if (!string.IsNullOrEmpty(order.user.lastName))
                {
                    name = string.Concat(name, " ", order.user.lastName);
                }
                data.receiver = name;
            }
            data.sender = string.Concat(me.organisation1.name, " (", me.email, ")");
            data.viewloc = order.user.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl("/browse/" + order.viewid);
            if (orderType.Value == TransactionType.INVOICE)
            {
                data.heading = "Send Invoice By Email";
                data.subject = string.Concat("[", me.organisation1.name, "] New Sales Invoice ",
                                             order.orderNumber.ToString("D8"));
                data.message = "You have received a new invoice. To view the invoice, follow the link below: ";
            }
            else if (orderType.Value == TransactionType.ORDER)
            {
                data.heading = "Send Order By Email";
                data.subject = string.Concat("[", me.organisation1.name, "] New Purchase Order ",
                                             order.orderNumber.ToString("D8"));
                data.message = "You have received a new order. To view the order, follow the link below: ";
            }
            else
            {
                throw new Exception("Unknown type");
            }

            return View(data);
        }

        public ActionResult Review(long id)
        {
            var transaction = repository.GetOrder(subdomainid.Value, id).transactions;
            if (transaction == null ||
                !transaction.reviewid.HasValue ||
                !transaction.review.pending)
            {
                return RedirectToAction("Index", "Error");
            }

            var viewdata = new ReviewViewData(baseviewmodel)
            {
                reviewID = transaction.reviewid.Value,
                transactionID = transaction.id,
                transactionLink = transaction.ToTransactionLink(),
                transactionName = transaction.ToTransactionName(),
            };

            return View(viewdata);
        }

        [HttpGet]
        public ActionResult MarkReceived(long id)
        {
            return View((object)id);
        }

        [HttpPost]
        public ActionResult MarkReceived(long id, DateTime actualDate)
        {
            var transaction = new Transaction(repository.GetOrder(subdomainid.Value, id), repository, sessionid.Value);

            transaction.MarkReceived(actualDate);

            transaction.SaveUpdatedTransaction();

            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult Save(string terms, long? id, long orderNumber, decimal?[] tax, long?[] itemid, int[] quantity, decimal[] unitPrice,
            string discount, string discountType, int currency,
            string shippingMethod, string shippingCost, long receiverOrgID, DateTime orderDate, long location, TransactionType type)
        {
            bool newOrder = !id.HasValue;

            var order_byOrderNumber = repository.GetOrderByOrderNumber(subdomainid.Value, type, orderNumber);

            Transaction transaction;
            if (!newOrder)
            {
                // update
                transaction = new Transaction(repository.GetOrder(subdomainid.Value, id.Value), repository, sessionid.Value);

                if (transaction.GetOrderStatus() != OrderStatus.DRAFT)
                {
                    return Json("Cannot edit sent orders".ToJsonFail());
                }

                // verify that order or invoice number does not already exist
                if (order_byOrderNumber != null && order_byOrderNumber.id != id.Value)
                {
                    var msg = string.Format("{0} Number #{1} already in use", type.ToDescriptionString(), orderNumber.ToString("D8"));
                    return SendJsonErrorResponse(msg);
                }
            }
            else
            {
                if (accountLimits.invoices.HasValue && type == TransactionType.INVOICE)
                {
                    var invoicesThisMonth = repository.GetMonthlyInvoiceCount(sessionid.Value);
                    if (invoicesThisMonth >= accountLimits.invoices.Value)
                    {
                        return SendJsonErrorResponse("Monthly invoice limit exceeded. Please upgrade your <a href=\"/dashboard/account/plan\">plan</a>.");
                    }
                }

                // create
                var receiver = repository.GetPrimaryUser(receiverOrgID);
                transaction = new Transaction(MASTERdomain, receiver, type, repository, sessionid.Value);
                transaction.CreateTransaction(orderNumber, orderDate.ToUniversalTime(), terms, currency);

                // verify that order or invoice number does not already exist
                if (order_byOrderNumber != null)
                {
                    var msg = string.Format("{0} Number #{1} already in use", type.ToDescriptionString(), orderNumber.ToString("D8"));
                    return SendJsonErrorResponse(msg);
                }
            }

            var posted_orderitems = new List<orderItem>();

            if (itemid != null)
            {
                for (int i = 0; i < itemid.Length; i++)
                {
                    var id_item = itemid[i];
                    if (!id_item.HasValue)
                    {
                        continue;
                    }
                    var item_quantity = quantity[i];
                    var unit_price = unitPrice[i];
                    var item = new orderItem()
                                   {
                                       quantity = item_quantity,
                                       unitPrice = unit_price,
                                       variantid = id_item.Value
                                   };

                    // invoice only
                    if (tax != null)
                    {
                        item.tax = tax[i];
                    }

                    // insert into order
                    posted_orderitems.Add(item);
                }
            }

            if (posted_orderitems.Count == 0 || itemid == null)
            {
                return Json("No items were specified".ToJsonFail());
            }

            // update fields
            transaction.UpdateCurrency(currency);
            transaction.UpdateDiscount(discountType, discount);
            transaction.UpdateOrderNumber(orderNumber);
            transaction.UpdateShippingCost(shippingCost);
            transaction.UpdateShippingMethod(shippingMethod);
            transaction.UpdateTerms(terms);
            transaction.UpdateInventoryLocation(location);

            // email receiver of change
            if (transaction.GetOrderStatus() != OrderStatus.DRAFT)
            {
                var data = new ChangeHistory
                {
                    documentType = type == TransactionType.INVOICE? ChangeHistoryType.INVOICE.ToDocumentType() : ChangeHistoryType.ORDERS.ToDocumentType(),
                    documentName = transaction.GetOrderNumber(),
                    documentLoc = accountHostname.ToDomainUrl(transaction.GetOrderLink())
                };

                var message = new Message(transaction.GetReceiver(), transaction.GetOwner(), subdomainid.Value);
                message.SendMessage(this, repository, EmailViewType.INVOICEORDER_CHANGED, data,
                                    data.documentType + " Updated", transaction.GetOrderLink());
            }

            var variants = repository.GetProductVariants(subdomainid.Value).Where(x => itemid.Contains(x.id));

            if (!newOrder)
            {
                // log activity
                repository.AddActivity(sessionid.Value,
                    new ActivityMessage(transaction.GetID(), transaction.GetReceiver().id,
                            type == TransactionType.ORDER? ActivityMessageType.ORDER_UPDATED : ActivityMessageType.INVOICE_UPDATED,
                            new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(type)), subdomainid.Value);


                // handle deleted items first
                var orderItemsToDelete = transaction.GetOrderItems().Where(x => !itemid.Contains(x.variantid)).ToArray();

                foreach (var entry in orderItemsToDelete)
                {
                    var oitem = entry;
                    transaction.DeleteOrderItem(oitem);
                }
            }

            // add order items
            foreach (var entry in posted_orderitems)
            {
                var variantid = entry.variantid;

                // try get from existing order
                var existingOrderitem = transaction.GetOrderItems().SingleOrDefault(x => x.variantid == variantid);
                int inventoryDelta;
                if (existingOrderitem == null)
                {
                    // this is a new item so we need to add it
                    var v = variants.Single(x => x.id == variantid);
                    entry.description = v.ToProductFullTitle();

                    transaction.AddOrderItem(entry, v.product.products_digitals);

                    inventoryDelta = entry.quantity;
                    existingOrderitem = entry;
                }
                else
                {
                    // get delta first
                    inventoryDelta = entry.quantity - existingOrderitem.quantity;

                    existingOrderitem.unitPrice = entry.unitPrice;
                    existingOrderitem.quantity = entry.quantity;
                    existingOrderitem.tax = entry.tax;
                }

                transaction.UpdateInventoryItem(existingOrderitem, inventoryDelta);
            }
            
            transaction.UpdateTotal();
            if (!newOrder)
            {
                transaction.SaveUpdatedTransaction();
            }
            else
            {
                transaction.SaveNewTransaction();
            }

            return Json(transaction.GetID().ToJsonOKData());
        }


        [HttpGet]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult Ship(long id)
        {
            var order = repository.GetOrder(subdomainid.Value, id);
            if (order.ebayID.HasValue)
            {
                var viewmodel = new EbayOrderShipViewModel();
                viewmodel.orderID = id;
                viewmodel.shippingService = order.shippingMethod;

                return View("shipEbay", viewmodel);
            }
            else
            {
                // normal tradelr shipment
                var viewmodel = new OrderShipViewModel();
                viewmodel.useShipwire = !string.IsNullOrEmpty(MASTERdomain.shipwireEmail) &&
                                        !string.IsNullOrEmpty(MASTERdomain.shipwirePassword);
                viewmodel.orderID = id;
                return View(viewmodel);
            }
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult ShipEbay(long id, string shippingService, string trackingno, string feedbackComment)
        {
            if (string.IsNullOrEmpty(feedbackComment))
            {
                return Json("Please enter a feedback".ToJsonFail());
            }

            if (string.IsNullOrEmpty(shippingService))
            {
                return Json("Please specify a shipping service".ToJsonFail());
            }

            var order = repository.GetOrder(subdomainid.Value, id);
            if (order == null)
            {
                return Json("Order not found".ToJsonFail());
            }

            var transaction = new Transaction(order, repository, sessionid.Value);

            // update local order
            transaction.UpdateOrderAsShipped(shippingService, trackingno);

            // update ebay order
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            var ebayservice = new OrderService(token.token_key);

            // create feedback entry
            var feedback = new FeedbackInfoType();
            feedback.CommentText = feedbackComment;
            feedback.CommentType = CommentTypeCodeType.Positive;

            // create shipment entry
            var shipment = new ShipmentType();
            shipment.ShipmentTrackingNumber = trackingno;
            shipment.ShippingCarrierUsed = shippingService;

            // complete sale has to be called for each orderitem
            foreach (var ebayOrderitem in order.ebay_order.ebay_orderitems)
            {
                ebayservice.CompleteSale(order.ebay_order.orderid, ebayOrderitem.lineid, true, true, feedback, shipment);
            }


            return Json("Order shipped".ToJsonOKMessage());
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult Ship(long id, string trackingno, string trackingAddress, string shippingService, string shipwire)
        {
            try
            {
                var order = repository.GetOrder(subdomainid.Value, id);
                if (order == null)
                {
                    return Json("Order not found".ToJsonFail());
                }

                var transaction = new Transaction(order, repository, sessionid.Value);

                if (!string.IsNullOrEmpty(shipwire))
                {
                    if (!transaction.HasValidShippingAddress())
                    {
                        return SendJsonErrorResponse("The destination shipping address is incomplete.");
                    }
                    var shipwireService = transaction.GetShipWireService();
                    var shipwireItems = order.ToShipwireItems();
                    var shippingMethod = order.shipwireShippingid;
                    var address = transaction.GetShipwireShippingAddress();
                    shipwireService.CreateOrder(transaction.GetID().ToString(), shippingMethod, shipwireItems, address);
                    var resp = shipwireService.SubmitOrder();

                    // got response?
                    if (resp == null)
                    {
                        return SendJsonErrorResponse("No response from Shipwire. Please try again later.");
                    }

                    // check for exceptions
                    var exceptions = resp.GetExceptions();
                    if (exceptions.Count != 0)
                    {
                        return SendJsonErrorResponse(exceptions[transaction.GetID().ToString()]);
                    }

                    // check for one order
                    var submittedOrder = resp.OrderInformation.Order[0];

                    transaction.UpdateOrderStatus(OrderStatus.SHIPPED);

                    transaction.AddShipwireTransaction(submittedOrder.number.ToString(), XElement.Parse(shipwireService.GetXmlResponse()));

                    transaction.SaveUpdatedTransaction();
                    
                }
                else
                {
                    transaction.UpdateOrderAsShipped(shippingService, trackingno, trackingAddress);

                    // notify buyer that order has been shipped
                    var viewloc = MASTERdomain.ToHostName().ToDomainUrl(transaction.GetOrderLink());
                    var emailContent = new OrderShippedEmailContent
                    {
                        orderNumber = transaction.GetOrderNumber(),
                        shippingAddress = transaction.GetShippingAddress().ToHtmlString(),
                        sender = transaction.GetOwner().ToEmailName(false),
                        viewloc = viewloc
                    };

                    string subject = "Invoice #" + emailContent.orderNumber + " has been shipped";
                    var msg = new Message(transaction.GetReceiver(), transaction.GetOwner(), subdomainid.Value);
                    var result = msg.SendMessage(this, repository, EmailViewType.ORDER_SHIPPED, emailContent, subject, viewloc);

                    if (!result.success)
                    {
                        Syslog.Write(result.message);
                    }
                }

                return Json("".ToJsonOKMessage());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_SEND)]
        public ActionResult Send(long id, string subject, string message, string viewloc)
        {
            var transaction = new Transaction(repository.GetOrder(subdomainid.Value, id), repository, sessionid.Value);

            // get necessary information
            var me = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var sender = string.Concat(me.organisation1.name, " (", me.email, ")");

            // send mail (do this last in case we can't send email)
            var data = new NewOrderEmailContent
            {
                message = message,
                viewloc = viewloc,
                sender = sender,
            };

            var msg = new Message(transaction.GetReceiver(), transaction.GetOwner(), subdomainid.Value);
            var result = msg.SendMessage(this, repository, EmailViewType.INVOICEORDER_NEW, data, subject, viewloc);

            if (!result.success)
            {
                return Json(result.message.ToJsonFail());
            }

            // update order status
            transaction.UpdateOrderStatus(OrderStatus.SENT);

            // log activity
            string comment;
            if (transaction.GetType() == TransactionType.ORDER)
            {
                // PURCHASE ORDER
                comment = "Purchase Order sent.";

                repository.AddActivity(transaction.GetOwner().id,
                                       new ActivityMessage(transaction.GetID(), transaction.GetOwner().id,
                                                   ActivityMessageType.ORDER_SENT,
                                                   new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(TransactionType.ORDER)), subdomainid.Value);
                repository.AddActivity(transaction.GetOwner().id,
                                       new ActivityMessage(transaction.GetID(), transaction.GetReceiver().id,
                                                   ActivityMessageType.ORDER_RECEIVED,
                                                   new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(TransactionType.ORDER),
                                                   new HtmlLink(transaction.GetOwner().ToEmailName(true), transaction.GetOwner().id).ToContactString()), subdomainid.Value);
            }
            else
            {
                // SALES INVOICE
                comment = "Sales Invoice sent.";
                repository.AddActivity(transaction.GetOwner().id,
                                       new ActivityMessage(transaction.GetID(), transaction.GetOwner().id,
                                                   ActivityMessageType.INVOICE_SENT,
                                                   new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(TransactionType.INVOICE)), subdomainid.Value);
                repository.AddActivity(transaction.GetOwner().id,
                                       new ActivityMessage(transaction.GetID(), transaction.GetReceiver().id,
                                                   ActivityMessageType.INVOICE_RECEIVED,
                                                   new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(TransactionType.INVOICE),
                                                   new HtmlLink(transaction.GetOwner().ToEmailName(true), transaction.GetOwner().id).ToContactString()), subdomainid.Value);
            }

            transaction.AddComment(comment, sessionid.Value);

            return Json("".ToJsonOKMessage());
        }

        public ActionResult View(long? id)
        {
            var order = repository.GetOrder(subdomainid.Value, id.Value);

            organisation sender = order.user1.organisation1;
            var receiverAddress = order.receiverAddress;
            if (order.receiverUserid.HasValue)
            {
                receiverAddress = order.user.organisation1.ToFullOrganisationAddress();
            }

            // log activity
            // only log this if viewed by the receiver
            if (order.receiverUserid.HasValue && 
                sessionid.Value == order.receiverUserid.Value &&
                order.status == OrderStatus.SENT.ToString())
            {
                repository.AddActivity(order.owner,
                                       new ActivityMessage(order.id, order.owner,
                                                   ActivityMessageType.ORDER_VIEWED,
                                                   new HtmlLink(order.orderNumber.ToString("D8"), order.id).ToTransactionString(TransactionType.ORDER),
                                                   new HtmlLink(order.user.ToEmailName(true), order.receiverUserid.Value).ToContactString()), sender.subdomain);
                repository.UpdateOrderStatus(order.id, TransactionType.ORDER, null, sessionid.Value, OrderStatus.VIEWED);
            }

            var primaryView = new OrderView(order, sender, receiverAddress, sessionid.Value, TransactionType.ORDER);

            primaryView.SetStatusRibbon(!primaryView.order.isOwner);
            primaryView.SetButtonsToShow(TransactionType.ORDER, !primaryView.order.isOwner);
            primaryView.comments = order.ToTransaction().comments.OrderByDescending(x => x.created).ToModel(true);

            if (Request.HttpMethod == "POST")
            {
                return View("orderView", primaryView);
            }

            var printView = new PrintView(baseviewmodel)
            {
                transactionID = order.ToTransaction().id,
                primaryView = primaryView
            };
            
            return View("PrintView", printView);
            
        }
    }
}
