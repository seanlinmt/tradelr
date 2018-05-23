using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Controllers.reports
{
    //[ElmahHandleError]
    public class reportingController : baseController
    {
        private ActionResult Index()
        {
            return View();
        }

    }
}
