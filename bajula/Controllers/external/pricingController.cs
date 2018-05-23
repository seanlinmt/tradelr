using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;

namespace tradelr.Controllers.external
{
    
    //[ElmahHandleError]
    public class pricingController : Controller
    {
        //
        // GET: /pricing/
#if !DEBUG
        [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
#endif
        public ActionResult Index()
        {
            return View();
        }

    }
}
