using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.Libraries.ActionFilters;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.Controllers
{
    
    //[ElmahHandleError]
    public class browseController : baseController
    {
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return ReturnError("empty viewid");
            }
            // TODO: possible brute force hacking
            // get order that matches the id
            var order = repository.GetOrderByViewID(id);

            if (order == null)
            {
                return ReturnError("cannot find order for viewid :" + id);
            }

            // set session - only set it if user is currently not logged in
            if (!sessionid.HasValue && order.receiverUserid.HasValue)
            {
                SetAuthCookie(order.user, false);
            }
            
            if (string.Compare(order.type, TransactionType.ORDER.ToString(), true) == 0)
            {
                return RedirectToAction("view", "orders",new RouteValueDictionary(new Dictionary<string, object> { { "id", order.id } }));
            }
            return RedirectToAction("view", "invoices",
                                    new RouteValueDictionary(new Dictionary<string, object> { { "id", order.id } }));
        }

    }
}
