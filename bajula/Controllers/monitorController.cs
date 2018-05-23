using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using clearpixels.Logging;

namespace tradelr.Controllers
{
    /// <summary>
    /// used to monitor clicks on features which have not been actually implemented yet
    /// </summary>
    public class monitorController : Controller
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string feature)
        {
            Syslog.Write("clicked: " + feature);
            return new EmptyResult();
        }
    }
}
