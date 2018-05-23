using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;

namespace tradelr.Controllers.external
{
    
    //[ElmahHandleError]
    [TradelrHttp]
    public class aboutController : Controller
    {
        //
        // GET: /about/
        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
        public ActionResult Index()
        {
            return View();
        }

    }
}