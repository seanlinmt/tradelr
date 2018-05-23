using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.comments;
using tradelr.Models.counter;
using tradelr.Models.history;
using tradelr.Models.transactions;
using tradelr.Models.transactions.viewmodel;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class invoicesController : baseController
    {
        [HttpGet]
        [NoCache]
        [PermissionFilter(permission = UserPermission.INVOICES_ADD)]
        public ActionResult Add()
        {
            if (MASTERdomain.trialExpired)
            {
                return RedirectToAction("TrialExpired", "Error", new { Area = "" });
            }
            var viewmodel = new OrderViewModel(baseviewmodel);

            var orderNumber = repository.GetNewOrderNumber(subdomainid.Value, TransactionType.INVOICE);
            var org = MASTERdomain.organisation;

            viewmodel.o.CreateEmptyOrder(orderNumber, org, TransactionType.INVOICE);

            if (accountLimits.invoices.HasValue)
            {
                var invoicesThisMonth = repository.GetMonthlyInvoiceCount(sessionid.Value);
                if (invoicesThisMonth == accountLimits.invoices.Value)
                {
                    viewmodel.LimitHit = true;
                }
                else if (invoicesThisMonth > accountLimits.invoices.Value)
                {
                    viewmodel.LimitHit = true;
                    Syslog.Write("Monthly invoice limit exceeded " + sessionid.Value);
                }
            }

            var allContacts = repository.GetAllOrganisationExceptOwn(subdomainid.Value, org.id).Select(x => new { x.name, x.id }).ToList();

            viewmodel.ContactList = allContacts
                .OrderBy(x => x.name)
                .Select(x => new SelectListItem { Text = x.name, Value = x.id.ToString() })
                .ToSelectList(null, "Select ...", "");

            viewmodel.currencyList = CurrencyHelper.GetCurrencies().Select(x => new SelectListItem()
            {
                Text = x.name,
                Value = x.id.ToString(),
                Selected = x.id == MASTERdomain.currency
            });

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

        [HttpPost]
        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        public ActionResult Delete(long id)
        {
            // is order in use
            var order = repository.GetOrder(subdomainid.Value, id);
            if (order == null)
            {
                return SendJsonErrorResponse("Invoice not found");
            }
            var transaction = new Transaction(order, repository, sessionid.Value);
            var status = transaction.GetOrderStatus();

            if (status != OrderStatus.DRAFT && status != OrderStatus.SENT)
            {
                return SendJsonErrorResponse("Unable to delete. Invoice has already been viewed.");
            }

            try
            {
#if LUCENE
                var indexer = new LuceneWorker(db, MASTERdomain.ToIdName());
                indexer.DeleteFromIndex(LuceneIndexType.TRANSACTION, id);
#endif
                repository.UpdateCounters(subdomainid.Value, -1, CounterType.INVOICES_SENT);
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
        public ActionResult Edit(long id)
        {
            var invoice = repository.GetOrder(subdomainid.Value, id);
            if (invoice == null)
            {
                return RedirectToAction("NotFound", "Error");
            }

            // only allow invoices to be edited for drafts
            if (invoice.status != OrderStatus.DRAFT.ToString())
            {
                return RedirectToAction("Index", "Error", new { Area = "", msg = "Unable to edit invoices once it has been sent.", redirect = "/dashboard/transactions" });
            }

            var viewmodel = new OrderViewModel(baseviewmodel, invoice, sessionid.Value, TransactionType.INVOICE);
            var org = MASTERdomain.organisation;

            var allContacts = repository.GetAllOrganisationExceptOwn(subdomainid.Value, org.id).Select(x => new { x.name, x.id }).ToList();

            viewmodel.ContactList = allContacts
                .OrderBy(x => x.name)
                .Select(x => new SelectListItem { Text = x.name, Value = x.id.ToString() })
                .ToSelectList(viewmodel.o.receiverOrgID, "Select ...", "");

            viewmodel.ChangeItems = repository.GetChangeHistory(ChangeHistoryType.INVOICE, id, subdomainid.Value).OrderByDescending(x => x.changeDate).ToChangeModel();

            viewmodel.currencyList = CurrencyHelper.GetCurrencies().Select(x => new SelectListItem()
            {
                Text = x.name,
                Value = x.id.ToString(),
                Selected = x.id == viewmodel.o.currency.id
            });

            viewmodel.locationList =
                MASTERdomain.inventoryLocations.Select(
                    x => new SelectListItem()
                    {
                        Text = x.name,
                        Value = x.id.ToString(),
                        Selected = x.name == viewmodel.o.location
                    });

            return View("add", viewmodel);
        }

        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        [HttpGet]
        public ActionResult Terms()
        {
            user usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            return View("terms", (object)MASTERdomain.paymentTerms);
        }

        [PermissionFilter(permission = UserPermission.TRANSACTION_MODIFY)]
        [HttpPost]
        public ActionResult Terms(string terms)
        {
            MASTERdomain.paymentTerms = terms;
            repository.Save();
            return Json("".ToJsonOKMessage());
        }

        public ActionResult View(long? id)
        {
            var invoice = repository.GetOrder(subdomainid.Value, id.Value);

            organisation sender = invoice.user1.organisation1;
            var receiverAddress = invoice.receiverAddress;
            if (invoice.receiverUserid.HasValue)
            {
                receiverAddress = invoice.user.organisation1.ToFullOrganisationAddress();
            }

            // log activity
            if (invoice.receiverUserid.HasValue && 
                sessionid.Value == invoice.receiverUserid.Value &&
                invoice.status == OrderStatus.SENT.ToString())
            {
                // update status
                repository.AddActivity(invoice.owner,
                                       new ActivityMessage(invoice.id, invoice.owner,
                                                   ActivityMessageType.INVOICE_VIEWED,
                                                   new HtmlLink(invoice.orderNumber.ToString("D8"), invoice.id).ToTransactionString(TransactionType.INVOICE),
                                                   new HtmlLink(invoice.user.ToEmailName(true), invoice.receiverUserid.Value).ToContactString()), sender.subdomain);
                repository.UpdateOrderStatus(id.Value, TransactionType.INVOICE, null, sessionid.Value, OrderStatus.VIEWED);
            }

            var primaryView = new OrderView(invoice, sender, receiverAddress, sessionid.Value, TransactionType.INVOICE);

            primaryView.SetStatusRibbon(!primaryView.order.isOwner);
            primaryView.SetButtonsToShow(TransactionType.INVOICE, !primaryView.order.isOwner);
            primaryView.comments = invoice.ToTransaction().comments.OrderByDescending(x => x.created).ToModel(true);

            if (Request.HttpMethod == "POST")
            {
                return View("invoiceView", primaryView);
            }

            var printView = new PrintView(baseviewmodel)
            {
                transactionID = invoice.ToTransaction().id,
                primaryView = primaryView
            };

            return View("PrintView", printView);
        }
    }
}
