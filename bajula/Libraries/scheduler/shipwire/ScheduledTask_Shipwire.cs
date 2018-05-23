using System;
using System.Linq;
using System.Threading;
using Shipwire;
using Shipwire.inventory;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Controllers;
using tradelr.Crypto;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Email.Models;
using tradelr.Libraries.reporting;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.comments;
using tradelr.Models.email;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.Libraries.scheduler
{
    public static partial class ScheduledTask
    {
        public static void ShipwirePollForInventoryUpdates()
        {
            using (var repository = new TradelrRepository())
            {
                var cryptor = new AESCrypt();
                var sds =
                    repository.GetSubDomains().Where(
                        x =>
                        x.shipwireEmail != null && x.shipwirePassword != null && 
                        x.shipwireEmail != "" && x.shipwirePassword != "");
                foreach (var sd in sds)
                {
                    var email = sd.shipwireEmail;
                    var pass = cryptor.Decrypt(sd.shipwirePassword, sd.id.ToString());
                    var shipwire = new ShipwireService(email, pass);
                    var updater = new InventoryUpdate(shipwire, sd.id);
                    new Thread(() => updater.Update(WarehouseLocation.Values)).Start();
                }
            }
        }
        private static bool ShipwirePollForShippedStatus(ITradelrRepository repository)
        {
            bool changed = false;
            var orders =
                repository.GetOrders().Where(
                    x =>
                    x.shipwireTransaction != null && x.shipwireTransaction.state == ShipwireState.ORDER_SUBMITTED.ToString());
            foreach (var entry in orders)
            {
                var transaction = new Transaction(entry, repository, null);
                if (transaction.GetReceiver() == null)
                {
                    continue;
                }
                
                var shipwire = transaction.GetShipWireService();
                if (shipwire == null)
                {
                    continue;
                }
                
                var transactionid = transaction.GetShipwireTransactionID();
                shipwire.CreateTrackingUpdate(transactionid);
                var resp = shipwire.SubmitTrackingUpdate();
                if (resp == null)
                {
                    continue;
                }
                // try get matching order
                var responseOrder = resp.Orders.Where(x => x.id == transactionid).SingleOrDefault();
                if (responseOrder != null)
                {
                    if (responseOrder.shipped)
                    {
                        transaction.UpdateShipwireState(ShipwireState.ORDER_SHIPPED);

                        // add comment
                        string comment = string.Format(OrderComment.ORDER_SHIP_DETAILED, responseOrder.shipperFullName, responseOrder.TrackingNumber.Value, responseOrder.TrackingNumber.href);
                        transaction.AddComment(comment);

                        transaction.SaveUpdatedTransaction();

                        changed = true;

                        // notify buyer that order has been shipped
                        var subdomain = transaction.GetOwner().organisation1.MASTERsubdomain;
                        repository.AddActivity(transaction.GetReceiver().id,
                                           new ActivityMessage(transaction.GetID(), transaction.GetOwner().id,
                                                       ActivityMessageType.ORDER_SHIPPED,
                                                       new HtmlLink(transaction.GetOrderNumber(), transaction.GetID()).ToTransactionString(transaction.GetType())), subdomain.id);

                        var viewloc = subdomain.ToHostName().ToDomainUrl(transaction.GetOrderLink());

                        // notify buyer that order has been shipped
                        var emailContent = new OrderShippedEmailContent
                        {
                            orderNumber = transaction.GetOrderNumber(),
                            shippingAddress = transaction.GetShippingAddress().ToHtmlString(),
                            sender = transaction.GetOwner().ToEmailName(false),
                            viewloc = viewloc
                        };

                        string subject = "Invoice #" + emailContent.orderNumber + " has shipped";
                        var msg = new Models.message.Message(transaction.GetReceiver(), transaction.GetOwner(), subdomain.id);
                        var controller = new dummyController();
                        msg.SendMessage(controller, repository, EmailViewType.ORDER_SHIPPED, emailContent, subject, viewloc);
                    }
                    else
                    {
                        Syslog.Write("Shipwire order not shipped {0} : {1}",
                                               transaction.GetShipwireTransactionID(),
                                               shipwire.GetXmlResponse());
                    }

                }
                else
                {
                    transaction.UpdateShipwireState(ShipwireState.ORDER_NOTFOUND);
                    transaction.SaveUpdatedTransaction();
                    changed = true;
                    Syslog.Write("Cannot find order for {0} : {1}",
                                               transaction.GetShipwireTransactionID(),
                                               shipwire.GetXmlResponse());
                }
            }
            return changed;
        }
    }
}