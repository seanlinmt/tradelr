using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.comments;
using tradelr.Models.subdomain;
using tradelr.Models.time;
using tradelr.Models.transactions;
using tradelr.Models.transactions.viewmodel;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [TradelrHttps]
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    public class transactionsController : baseController
    {
        [HttpPost]
        public ActionResult AddNote(long id, string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return SendJsonErrorResponse("Comment required");
            }
            var c = new comment
            {
                comments = comment,
                created = DateTime.UtcNow,
                creator = sessionid.Value,
                transactionID = id
            };
            repository.AddOrderComment(c);

            repository.Save();

            string body = this.RenderViewToString(TradelrControls.comments.ToDescriptionString(), c.ToModel(true)).Trim();

            // send email
            var transaction = repository.GetTransaction(id);
            var order = transaction.order;
            var emailContent = new CommentEmailContent { comment = comment };
            string hostNameTarget = null;
            user receiver = null;
            if (order.user1.organisation1.subdomain == subdomainid.Value)
            {
                // check could have been done before as if receiver has been deleted then they wouldnt have been able to leave a message anyway!
                if (order.receiverUserid.HasValue)
                {
                    receiver = order.user;
                    hostNameTarget = order.user.organisation1.MASTERsubdomain.ToHostName();
                }
            }
            else
            {
                receiver = order.user1;
                hostNameTarget = order.user1.organisation1.MASTERsubdomain.ToHostName();
            }

            if (receiver != null)
            {
                if (order.type == TransactionType.ORDER.ToString())
                {
                    emailContent.targetName = "Order #" + order.orderNumber.ToString("D8");
                    emailContent.commentsLink = hostNameTarget.ToDomainUrl(order.ToOrderLink());
                }
                else
                {
                    emailContent.targetName = "Invoice #" + order.orderNumber.ToString("D8");
                    emailContent.commentsLink = hostNameTarget.ToDomainUrl(order.ToOrderLink());
                }

                var sender = repository.GetUserById(sessionid.Value, subdomainid.Value);

                emailContent.creator = sender.ToEmailName(true);


                var msg = new tradelr.Models.message.Message(receiver, sender, subdomainid.Value);
                var result = msg.SendMessage(this, repository, EmailViewType.INVOICEORDER_NEW_COMMENT, emailContent,
                                             string.Format("New Note for {0}", emailContent.targetName), emailContent.commentsLink);

                if (!result.success)
                {
                    return Json(result.message.ToJsonFail());
                }
            }

            return Json(body.ToJsonOKData());
        }

        public ActionResult Index()
        {
            var viewmodel = new TransactionViewModel(baseviewmodel)
            {
                timeline = typeof(Timeline).ToSelectList(false).ToFilterList(),
                statuses = new[]
                               {
                                    new SelectListItem(){Text = OrderStatus.DRAFT.ToDescriptionString(), Value = OrderStatus.DRAFT.ToString()},
                                    new SelectListItem(){Text = OrderStatus.SENT.ToDescriptionString(), Value = OrderStatus.SENT.ToString()},
                                    new SelectListItem(){Text = OrderStatus.VIEWED.ToDescriptionString(), Value = OrderStatus.VIEWED.ToString()},
                                    new SelectListItem(){Text = OrderStatus.PARTIAL.ToDescriptionString(), Value = OrderStatus.PARTIAL.ToString()},
                                    new SelectListItem(){Text = OrderStatus.PAID.ToDescriptionString(), Value = OrderStatus.PAID.ToString()},
                                    new SelectListItem(){Text = OrderStatus.SHIPPED.ToDescriptionString(), Value = OrderStatus.SHIPPED.ToString()}
                               }.ToFilterList(),
                permission = permission
            };
            return View(viewmodel);
        }

        
        public ActionResult List(string term, OrderStatus? status, TransactionType? type, TimeLine? interval, int rows, int page, string sidx, string sord)
        {
            IEnumerable<order> results;

            if (!type.HasValue)
            {
                type = TransactionType.ALL;
            }

            // admins can see all orders created, users can only see orders they have created
            if (permission.HasFlag(UserPermission.TRANSACTION_VIEW))
            {
                results = repository.GetOrders(subdomainid.Value, type.Value, sessionid.Value, interval, sidx, sord, false);
            }
            else
            {
                results = repository.GetOrders(subdomainid.Value, type.Value, sessionid.Value, interval, sidx, sord, true);
            }

            if (status.HasValue)
            {
                results = results.Where(x => x.status == status.Value.ToString());
            }
#if LUCENE
            if (!string.IsNullOrEmpty(term))
            {
                var search = new LuceneSearch();
                var ids = search.TransactionSearch(term.ToLower(), accountSubdomainName);
                results = results.Where(x => ids.Select(y => y.id).Contains(x.id.ToString())).AsEnumerable();
                results = results.Join(ids, x => x.id.ToString(), y => y.id, (x, y) => new { x, y.score })
                    .OrderByDescending(x => x.score).Select(x => x.x);
            }
#endif
            var records = results.Count();
            var total = (records / rows);
            if (records % rows != 0)
            {
                total++;
            }
            // return in the format required for jqgrid
            results = results.Skip(rows * (page - 1)).Take(rows);

            var orders = results.ToTransactionJqGrid(sessionid.Value);
            orders.page = page;
            orders.records = records;
            orders.total = total;
            return Json(orders);
        }
    }
}
