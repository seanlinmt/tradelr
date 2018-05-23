using System;
using System.Linq;
using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    [TradelrHttps]
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.INVENTORY_MODIFY)]
    public class stockUnitController : baseController
    {
        [HttpGet]
        
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string unitTitle, string ids, string unitTitleSelected)
        {
            unitTitle = unitTitle.Trim();

            // if it's empty then
            if (string.IsNullOrEmpty(unitTitle))
            {
                return SendJsonErrorResponse("Empty unit name");
            }

            var sunit = new stockUnit
            {
                unitID = string.IsNullOrEmpty(unitTitleSelected) ? repository.AddMasterStockUnit(unitTitle).id : long.Parse(unitTitleSelected),
                details = "",
                subdomainid = subdomainid.Value
            };
            try
            {
                repository.AddStockUnit(sunit);
                if (sunit.id == 0)
                {
                    return SendJsonErrorResponse("Unit already exist");
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(sunit.ToModel().ToJsonOKData());
        }

        public ActionResult Find(string q, string limit)
        {
            var result = repository.FindMASTERStockUnit(q, sessionid.Value);
            var data = from r in result
                       select new
                                  {
                                      title = r.name
                                  };
            return Json(data.ToList().ToJsonOKData(), JsonRequestBehavior.AllowGet);
        }

    }
}