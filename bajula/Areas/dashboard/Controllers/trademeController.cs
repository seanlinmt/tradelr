using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using clearpixels.Facebook.Resources;
using clearpixels.OAuth;
using eBay.Service.Core.Soap;
using tradelr.Areas.dashboard.Models.product.ebay;
using tradelr.Areas.dashboard.Models.product.trademe;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
    [TradelrHttps]
    public class trademeController : baseController
    {
        [HttpGet]
        [OutputCache(VaryByParam = "*", Location = OutputCacheLocation.Client, Duration = GeneralConstants.DURATION_1MONTH_SECS)]
        public ActionResult CategorySelector(string parentid)
        {
            var cats = db.trademe_categories
                        .Where(x => x.parentid == parentid && 
                                    x.id != parentid && x.active);

            var viewmodel = new EbayCategoryCondition();

            if (!cats.Any() && !string.IsNullOrEmpty(parentid))
            {
                // we have reached a leaf, get item conditions
                var selectedCat = db.trademe_categories.SingleOrDefault(x => x.id == parentid);
                if (selectedCat != null)
                {
                    viewmodel.durations = selectedCat.trademe_listingdurations
                        .Where(x => x.categoryid == selectedCat.id)
                        .Select(x => new IdName()
                        {
                            name = x.duration == 1 ? (x.duration + " day"):(x.duration + " days"),
                            id = x.duration.ToString(),
                        });
                }
            }
            else
            {
                viewmodel.categories = cats.Select(x => new IdName()
                {
                    id = x.id,
                    name = x.name
                });
            }

            return Json(viewmodel.ToJsonOKData(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ClearToken()
        {
            try
            {
                repository.DeleteOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return Json(false.ToJsonOKData());
            }
            return Json(true.ToJsonOKData());
        }
        
        /// <summary>
        /// gets oauth token for domain admin
        /// </summary>
        /// <returns></returns>
        public ActionResult GetToken()
        {
            var client = new OAuthClient(OAuthTokenType.TRADEME, 
                                        OAuthClient.OAUTH_TRADEME_CONSUMER_KEY,
                                        OAuthClient.OAUTH_TRADEME_CONSUMER_SECRET, 
                                        TradeMe.RestBase.CallbackUrl, 
                                        "HMAC-SHA1",
                                        "MyTradeMeRead,MyTradeMeWrite");

            if (!client.GetRequestToken())
            {
                return Redirect("/Error");
            }

            var oauthdb = new oauth_token
            {
                token_key = client.oauth_token,
                token_secret = client.oauth_secret,
                type = OAuthTokenType.TRADEME.ToString(),
                subdomainid = subdomainid.Value,
                appid = sessionid.Value.ToString(),
                authorised = false
            };

            repository.AddOAuthToken(oauthdb);

            return Redirect(client.authorize_url);
        }

        public ActionResult ProductSettings(long? id)
        {
            string categoryid = "";
            trademe_product trademeProduct = null;
            if (id.HasValue)
            {
                var p = repository.GetProduct(id.Value, subdomainid.Value);
                if (p != null && p.trademeID.HasValue)
                {
                    trademeProduct = p.trademe_product;
                    categoryid = p.trademe_product.categoryid;

                    // expires entry
                    if (p.trademe_product.isActive && p.trademe_product.endTime < DateTime.UtcNow)
                    {
                        p.trademe_product.isActive = false;
                        repository.Save("ProductSettings");
                    }
                }
            }

            var viewmodel = new TrademeProductViewModel(trademeProduct, MASTERdomain, db);
            viewmodel.PopulateCategories(categoryid);

            return View(viewmodel);
        }
    }
}
