using System;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;
using tradelr.Models.users;

namespace tradelr.Controllers.home
{
    //[ElmahHandleError]
    public class homeController : baseController
    {
        public ActionResult Index()
        {
            // return front page
            if (!subdomainid.HasValue)
            {
                return View();
            }

            // this point onward we have a subdomainid
            organisation org = MASTERdomain.organisation;
            // subdomain does not exist
            if (org == null)
            {
                return Redirect(GeneralConstants.HTTP_HOST);
            }

            // check if account has been setup properly
            if (role.HasFlag(UserRole.CREATOR) && !MASTERdomain.currency.HasValue)
            {
                return RedirectToAction("setup", "register", new { Area = "" });
            }

            // do we return store view which is on an unsecure connection
            if (IsStoreEnabled || sessionid.HasValue)
            {
                var template = CreateLiquidTemplate("index", "Welcome");
                template.InitContentTemplate("templates/index.liquid");

                return Content(template.Render());
            }

            if (sessionid == null)
            {
                return RedirectToAction("Index", "login");
            }

            // this is actually unreachable
            // user is logged in
            return RedirectToAction("Index", "dashboard", new { Area = "dashboard" });
        }

    }
}
