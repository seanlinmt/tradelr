using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI;
using clearpixels.Facebook.Resources;
using clearpixels.OAuth;
using Ebay;
using eBay.Service.Core.Soap;
using tradelr.Areas.dashboard.Models.product.ebay;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.scheduler;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.ebay;
using tradelr.Models.networks;
using tradelr.Models.products;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [TradelrHttps]
    [RoleFilter(role = UserRole.USER)]
    public class ebayController : baseController
    {
        [HttpGet]
        [OutputCache(VaryByParam = "*", Location = OutputCacheLocation.Client, Duration = GeneralConstants.DURATION_1MONTH_SECS)]
        public ActionResult CategorySelector(ListingTypeCodeType type, SiteCodeType site, int? parentid, int? level)
        {
            Debug.Assert(parentid.HasValue || level.HasValue);

            var cats = db.ebay_categories.Where(x => x.siteid == site.ToString());

            if (parentid.HasValue)
            {
                cats = cats.Where(x => x.parentid == parentid.Value && x.categoryid != parentid.Value);
            }

            if (level.HasValue)
            {
                cats = cats.Where(x => x.level == level.Value);
            }

            var viewmodel = new EbayCategoryCondition();

            if (!cats.Any() && parentid.HasValue)
            {
                // we have reached a leaf, get item conditions
                var condition_category = db.ebay_categories.SingleOrDefault(x => x.siteid == site.ToString() && 
                                                                                    x.categoryid == parentid.Value);
                if (condition_category != null)
                {
                    viewmodel.conditions = condition_category.ebay_conditions
                        .Select(x => new IdName()
                                         {
                                             name = x.name,
                                             id = x.value.ToString()
                                         });

                    viewmodel.durations = condition_category.ebay_listingdurations
                        .Where(x => x.listingtypeid == type.ToString())
                        .Select(x => new IdName()
                                         {
                                             name = EbayProductViewModel.DurationNames.ContainsKey(x.duration)?EbayProductViewModel.DurationNames[x.duration]:x.duration,
                                             id = x.duration
                                         });
                }
            }
            else
            {
                viewmodel.categories = cats.Select(x => new IdName()
                                                            {
                                                                id = x.categoryid.ToString(),
                                                                name = x.name
                                                            });
            }

            return Json(viewmodel.ToJsonOKData(), JsonRequestBehavior.AllowGet);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult clearToken()
        {
            try
            {
                var ebay = new NetworksEbay(subdomainid.Value);
                ebay.ClearSynchronisation();
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return Json(false.ToJsonOKData());
            }
            return Json(true.ToJsonOKData());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult getToken()
        {
            var ebayservice = new EbayService();
            var requesturl = ebayservice.GetRequestTokenUrl();

            var oauthdb = new oauth_token
            {
                token_key = ebayservice.SessionID,
                token_secret = "",
                type = OAuthTokenType.EBAY.ToString(),
                subdomainid = subdomainid.Value,
                appid = sessionid.Value.ToString(),
                authorised = false
            };

            repository.AddOAuthToken(oauthdb);

            // need to append ruparameter
            var parameters = new NameValueCollection {{"sid", ebayservice.SessionID}};

            return Redirect(string.Format(requesturl + "&ruparams={0}", parameters.ToQueryString(true).Substring(1)));
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Import(bool active, bool scheduled, bool unsold)
        {
            var importedcount = 0;
            var duplicatecount = 0;
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            if (token == null)
            {
                return Json("Please connect your account with eBay first".ToJsonFail());
            }

            try
            {
                var service = new UserService(token.token_key);
                var entries = service.GetMyEbaySelling(active, scheduled, unsold);
                var importer = new ProductImport();
                foreach (var entry in entries)
                {
                    var ebayid = entry.id;
                    if (repository.GetProducts(subdomainid.Value).Any(y => y.ebayID.HasValue && y.ebay_product.ebayid == ebayid))
                    {
                        duplicatecount++;
                    }
                    else
                    {
                        var pinfo = importer.ImportEbay(entry, subdomainid.Value);

                        repository.AddProduct(pinfo, subdomainid.Value);

                        importedcount++;
                    }
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json(string.Format("{0} products imported. {1} duplicates.", importedcount, duplicatecount).ToJsonOKMessage());
        }

        public ActionResult ProductSettings(long? id)
        {
            int? categoryid = null;
            ebay_product ebayproduct = null;
            if (id.HasValue)
            {
                var p = repository.GetProduct(id.Value, subdomainid.Value);
                if (p != null && p.ebayID.HasValue)
                {
                    ebayproduct = p.ebay_product;
                    categoryid = p.ebay_product.categoryid;

                    // expires entry
                    if (p.ebay_product.isActive && p.ebay_product.endTime < DateTime.UtcNow)
                    {
                        p.ebay_product.isActive = false;
                        repository.Save("ProductSettings");
                    }
                }
            }

            var viewmodel = new EbayProductViewModel(ebayproduct, MASTERdomain, db);
            viewmodel.PopulateCategories(categoryid);

            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult ListingEnd(string id)
        {
            var viewmodel = new EbayEndListingViewModel();
            viewmodel.itemid = id;
            viewmodel.reason = new[]
                                   {
                                       new SelectListItem()
                                           {
                                               Value = EndReasonCodeType.LostOrBroken.ToString(),
                                               Text = "The item was lost or broken"
                                           },
                                       new SelectListItem()
                                           {
                                               Value = EndReasonCodeType.NotAvailable.ToString(),
                                               Text = "The item is no longer available for sale"
                                           },
                                       new SelectListItem()
                                           {
                                               Value = EndReasonCodeType.Incorrect.ToString(),
                                               Text = "The start price or reserve price is incorrect"
                                           },
                                       new SelectListItem()
                                           {
                                               Value = EndReasonCodeType.OtherListingError.ToString(),
                                               Text =
                                                   "The listing contained an error (other than start price or reserve price)"
                                           },
                                       new SelectListItem()
                                           {
                                               Value = EndReasonCodeType.SellToHighBidder.ToString(),
                                               Text = "Sell the item to the highest eligible bidder"
                                           }
                                   };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult ListingEnd(string id, EndReasonCodeType reason)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            if (token == null)
            {
                return Json("".ToJsonFailData());
            }

            try
            {
                var service = new ItemService(token.token_key);
                service.EndFixedPriceItem(id, reason);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("Ended".ToJsonOKData());
        }

        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Sync()
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            if (token == null)
            {
                return Json("You are not linked to ebay".ToJsonFail());
            }

            var worker = new EbayWorker(token.MASTERsubdomain, token.token_key);
            new Thread(worker.PollForEbayOrders).Start();

            token.MASTERsubdomain.ebay_lastsync = DateTime.UtcNow;

            repository.Save();

            return Json(DateTime.UtcNow.ToString("s").ToJsonOKData());
        }
    }
}
