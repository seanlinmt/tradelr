using System.Collections.Specialized;
using System.Threading;
using System.Web.Mvc;
using Google.GData.Client;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.export.gbase;
using tradelr.Models.google.gbase;
using tradelr.Models.networks;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class gbaseController : baseController
    {
        public ActionResult clearToken()
        {
            repository.DeleteGoogleBaseSync(MASTERdomain);

            // delete inventory location
            repository.DeleteInventoryLocation(Networks.LOCATIONNAME_GBASE, subdomainid.Value);
            repository.Save();
            return Json(true.ToJsonOKData());
        }

        public ActionResult getToken(string upload, string accountid)
        {
            var parameters = new NameValueCollection();
            parameters.Add("upload", upload);
            parameters.Add("sd", accountHostname);
            parameters.Add("path", "/dashboard/gbase/saveToken");
            parameters.Add("accountid", accountid);

            var continueUrl = string.Format("{0}/callback{1}", GeneralConstants.HTTP_SECURE, parameters.ToQueryString(true));

            return Redirect(AuthSubUtil.getRequestUrl(continueUrl, "https://www.googleapis.com/auth/structuredcontent", false, true));
        }
    
        public ActionResult haveToken()
        {
            if (MASTERdomain.gbaseid.HasValue &&
                !string.IsNullOrEmpty(MASTERdomain.googleBase.sessiontoken))
            {
                return Json(true.ToJsonOKData());
            }
            return Json(false.ToJsonOKData());
        }

        public ActionResult saveToken(string token, bool? upload, long accountid)
        {
            var sessionToken = AuthSubUtil.exchangeForSessionToken(token, null);
            
            // save accountid
            if (MASTERdomain.gbaseid.HasValue)
            {
                MASTERdomain.googleBase.accountid = accountid;
            }
            else
            {
                MASTERdomain.googleBase = new googleBase()
                                              {
                                                  accountid = accountid,
                                                  sessiontoken = sessionToken
                                              };
            }

            repository.Save();

            // check if account exist first
            var gbaseitem = new GoogleBaseExporter(subdomainid.Value, accountHostname);
            var viewmodel = "";
            if (gbaseitem.VerifyAccount())
            {
                // start synchronisation
                var gbase = new NetworksGbase(subdomainid.Value, sessionid.Value, accountHostname);
                new Thread(() => gbase.StartSynchronisation(upload)).Start();
            }
            else
            {
                viewmodel = "Unable to connect to your Google Merchant Center account.";
            }

            return View("close", (object)viewmodel);
        }

        [HttpPost]
        public ActionResult settings(int? country)
        {
            if (!country.HasValue)
            {
                return Json("Invalid country selected".ToJsonFail());
            }

            MASTERdomain.googleBase.country = country.Value;
            repository.Save();

            return Json("Settings saved successfully".ToJsonOKMessage());
        }
    }
}
