using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Library.Constants;
using tradelr.Models.tour;

namespace tradelr.Controllers.external
{
    //[ElmahHandleError]
#if !DEBUG
    [OutputCache(Duration = GeneralConstants.DURATION_1DAY_SECS, VaryByParam = "None")]
#endif
    [TradelrHttp]
    public class tourController : Controller
    {
        public ActionResult Contacts()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_contacts,
                title = "Manage Business Contacts & Customers"
            };
            return View("Index", viewdata);
        }

        public ActionResult Engage()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_engage,
                title = "Engage Customers Around You"
            };
            return View("Index", viewdata);
        }

        public ActionResult Index()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_transactions,
                title = "Track Invoices and Purchase Orders"
            };
            return View("Index", viewdata);
        }

        public ActionResult Inventory()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_inventory,
                title = "Manage and Share Inventory Information"
            };
            return View("Index", viewdata);
        }
        
        public ActionResult Transactions()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_transactions,
                title = "Track Invoices and Purchase Orders"
            };
            return View("Index", viewdata);
        }

        public ActionResult Store()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_store,
                title = "Setup Your Personal Online Store and Sell Online"
            };
            return View("Index", viewdata);
        }

        public ActionResult Web()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_web,
                title = "Web-based Software"
            };
            return View("Index", viewdata);
        }

        public ActionResult Security()
        {
            var viewdata = new TourViewData()
            {
                control = TradelrControls.tour_security,
                title = "SSL Encryption and Secure Platform"
            };
            return View("Index", viewdata);
        }
    }
}
