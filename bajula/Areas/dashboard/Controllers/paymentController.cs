using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.account.payment;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Library.payment;
using clearpixels.Logging;
using tradelr.Models.payment;
using tradelr.Models.transactions;
using tradelr.Models.users;
using tradelr.Time;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class paymentController : baseController
    {
        [HttpGet]
        [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
        public ActionResult method_add()
        {
            var viewmodel = new PaymentMethodViewModel();

            viewmodel.methodList = typeof (PaymentMethod).ToSelectList(false, true);

            return View(viewmodel);
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
        public ActionResult method_del(long id)
        {
            var method = MASTERdomain.paymentMethods.SingleOrDefault(x => x.id == id);
            if (method == null)
            {
                return SendJsonErrorResponse("Could not find method");
            }

            if (MASTERdomain.paymentMethods.Count == 1)
            {
                return
                    Json(
                        "Cannot delete only payment method. A Paypal ID or a custom method needs to be defined.".
                            ToJsonFail());
            }

            db.paymentMethods.DeleteOnSubmit(method);
            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("Entry deleted successfully".ToJsonOKMessage());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
        public ActionResult method_edit(long id)
        {
            var method = MASTERdomain.paymentMethods.SingleOrDefault(x => x.id == id);
            if (method == null)
            {
                return SendJsonErrorResponse("Could not find method");
            }
            return View("method_add", method.ToFullModel());
        }

        [HttpGet]
        [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
        public ActionResult method_list()
        {
            return View(MASTERdomain.paymentMethods.OrderBy(x => x.name).ToModel());
        }

        [HttpPost]
        [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
        public ActionResult method_save(long? id, string name, string instructions, string method, string identifier)
        {
            var pmethod = new paymentMethod();
            if (id.HasValue)
            {
                pmethod = MASTERdomain.paymentMethods.SingleOrDefault(x => x.id == id);
                if (pmethod == null)
                {
                    return SendJsonErrorResponse("Could not find method");
                }
            }
            else
            {
                MASTERdomain.paymentMethods.Add(pmethod);
            }

            // sanity checks
            if (method == PaymentMethod.Paypal.ToString())
            {
                if (!MASTERdomain.currency.ToCurrencyCode().IsCurrencySupportedByPaypal())
                {
                    var errorMessage = string.Concat("<strong>", MASTERdomain.currency.ToCurrencyName(),
                                                     "</strong> is not supported by Paypal. Please change your currency if you want to accept payments via Paypal.");
                    return Json(errorMessage.ToJsonFail());
                }
            }

            pmethod.method = method;
            pmethod.identifier = identifier;
            pmethod.name = name;
            pmethod.instructions = instructions;

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return View("method_list", MASTERdomain.paymentMethods.OrderBy(x => x.name).ToModel());
        }

        [HttpPost]
        public ActionResult Create(string amount, long method, string notes, long orderID, string paymentDate)
        {
            var transaction = new Transaction(repository.GetOrder(subdomainid.Value, orderID), repository,
                                              sessionid.Value);
            var paidAmount = decimal.Parse(amount,
                                           NumberStyles.AllowDecimalPoint |
                                           NumberStyles.AllowThousands |
                                           NumberStyles.AllowThousands);

            // check that more isn't being paid
            var totalCost = transaction.GetTotal();
            if (transaction.GetTotalPaid() + paidAmount > totalCost)
            {
                return
                    SendJsonErrorResponse("You are attempting to pay more than necessary.");
            }

            // get method name
            var method_entry = MASTERdomain.paymentMethods.SingleOrDefault(x => x.id == method);
            if (method_entry == null)
            {
                return SendJsonErrorResponse("Unable to find payment method");
            }

            var payment = new payment
            {
                method = method_entry.name,
                paidAmount = paidAmount,
                notes = notes,
                orderid = transaction.GetID(),
                status = PaymentStatus.New.ToString(),
                created = TimeUtil.GetDateTime(paymentDate, GeneralConstants.DATEFORMAT_JAVASCRIPT)
            };

            transaction.AddPayment(payment, true);

            return Json("".ToJsonOKMessage());

        }

        [HttpGet]
        public ActionResult Method(long id)
        {
            var viewmodel = new tradelr.Models.payment.Payment();
            viewmodel.paymentMethods.Initialise(MASTERdomain, false);
            viewmodel.orderID = id;

            return View(viewmodel);
        }

        /// <summary>
        /// handles manual payment
        /// </summary>
        /// <param name="id">order ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult New(long id)
        {
            var viewmodel = new tradelr.Models.payment.Payment {orderID = id};
            var order = repository.GetOrder(id);
            
            if (order == null)
            {
                Syslog.Write("Unable to find order:" + id);
                return Json("Invalid ID".ToJsonFail(), JsonRequestBehavior.AllowGet);
            }

            viewmodel.paymentMethods.Initialise(MASTERdomain, true);
            
            var totalAmount = order.total;
            var currency = order.currency.ToCurrency();
            viewmodel.invoiceAmount = totalAmount.ToString("n" + currency.decimalCount);
            viewmodel.totalUnpaid = (totalAmount - order.totalPaid).ToString("n" + currency.decimalCount);
            viewmodel.currencyCode = currency.code;
            viewmodel.orderType = order.type.ToEnum<TransactionType>();

            return View(viewmodel);
        }
        
        [HttpPost]
        public ActionResult paypalCheckout(long id)
        {
            var order = repository.GetOrder(id);

            if (order == null)
            {
                Syslog.Write("Unable to find order:" + id);
                return Json("Invalid invoice".ToJsonFail());
            }
            
            // can't accept partially paid invoices at the moment
            if (order.totalPaid != 0)
            {
                return Json("Unable to use Paypal for partially paid invoices".ToJsonFail());
            }

            // check if there is already an incomplete payment
            var existingPaymentEntry =
                order.payments.SingleOrDefault(x => x.orderid == order.id && x.method == PaymentMethodType.Paypal.ToString());
            if (existingPaymentEntry != null)
            {
                // just in case
                if (string.IsNullOrEmpty(existingPaymentEntry.redirectUrl))
                {
                    return SendJsonErrorResponse(new NotSupportedException("redirecturl NULL:" + existingPaymentEntry.id));
                }
                return Json(existingPaymentEntry.redirectUrl.ToJsonOKData());
            }

            var sd = order.user1.organisation1.MASTERsubdomain;

            var invoice = new Transaction(order, repository, sessionid.Value);

            var pworker = new PaypalWorker(Utility.GenerateUniqueCode(),
                                           invoice,
                                           repository,
                                           sd.GetPaypalID(),
                                           invoice.GetCurrency().id,
                                           accountHostname.ToDomainUrl(invoice.GetOrderLink()));

            try
            {
                var redirecturl = pworker.GetPaymentUrl();
                return Json(redirecturl.ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpGet]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Review(long id)
        {
            var order = repository.GetOrder(subdomainid.Value, id);
            if (order == null)
            {
                return Json("Order not found".ToJsonFail());
            }

            var payment = order.payments.FirstOrDefault(x => x.status == PaymentStatus.New.ToString());

            if (payment == null)
            {
                return Json("No payment to review".ToJsonFail());
            }

            var currency = order.currency.ToCurrency();

            var viewmodel = new PaymentReviewViewModel();
            viewmodel.date_created = payment.created.ToString(GeneralConstants.DATEFORMAT_INVOICE);
            viewmodel.paymentid = payment.id;

            viewmodel.order_total = currency.symbol + order.total.ToString("n" + currency.decimalCount);
            viewmodel.order_title = string.Format("{0} #{1}", order.type, order.orderNumber.ToString("D8"));
            viewmodel.order_type = order.type;

            viewmodel.payment_amount = payment.paidAmount.ToString("n" + currency.decimalCount);
            viewmodel.payment_method = payment.method;
            viewmodel.payment_notes = payment.notes;

            return View(viewmodel);
        }

        [HttpPost]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Review(long id, string decision)
        {
            var pay = repository.GetPayment(id);

            var order = pay.order;

            // only owner can approve payment
            if (order.user1.id != sessionid.Value)
            {
                return Json("Only the owner can review this payment".ToJsonFail());
            }

            var transaction = new Transaction(pay.order, repository, sessionid);

            switch (decision)
            {
                case "accept":
                    transaction.UpdatePaymentStatus(pay, PaymentStatus.Accepted);
                    break;
                case "reject":
                    transaction.UpdatePaymentStatus(pay, PaymentStatus.Declined);
                    break;
                default:
                    throw new ArgumentException();
            }

            return Json("Status updated successfully".ToJsonOKMessage());
        }
    }
}