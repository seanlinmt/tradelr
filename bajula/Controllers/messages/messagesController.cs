using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using tradelr.DataAccess;
using tradelr.Libraries.ActionFilters;
using tradelr.Models.message;

namespace tradelr.Controllers.messages
{
    //[ElmahHandleError]
    public class messagesController : baseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult inbox(string page)
        {
            return message_inbox("inbox", page);
        }

        public ActionResult sent(string page)
        {
            return message_inbox("sent", page);
        }

        public ActionResult message_inbox(string type, string page)
        {
            var owner = sessionid.Value;

            var msgs = repository.GetMessages(owner, type == "inbox").ToModel();

            return View("messages", msgs);
        }

    }
}
