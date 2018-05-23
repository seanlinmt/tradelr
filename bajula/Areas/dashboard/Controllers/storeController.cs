using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Google.GData.WebmasterTools;
using tradelr.Areas.dashboard.Models.store;
using tradelr.Areas.dashboard.Models.store.general;
using tradelr.Areas.dashboard.Models.store.policies;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.JSON;
using tradelr.Models.google;
using tradelr.Models.payment;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class storeController : baseController
    {
        [HttpPost]
        public ActionResult Policies(string paymentTerms, string returnPolicy)
        {
            MASTERdomain.paymentTerms = paymentTerms;
            MASTERdomain.returnPolicy = returnPolicy;

            repository.Save(); // SUBMIT

            return Json("Store policies updated successfully".ToJsonOKMessage());
        }

        [HttpGet]
        public ActionResult Settings()
        {
            var viewmodel = new StoreSettingsViewModel(baseviewmodel)
                                {
                                    general = new GeneralSettings()
                                                  {
                                                      orgid = MASTERdomain.organisation.id,
                                                      store_enabled =
                                                          subdomainFlags.HasFlag(SubdomainFlags.STORE_ENABLED),
                                                      motd = MASTERdomain.organisation.motd,
                                                      storeName = MASTERdomain.storeName,
                                                      facebookCoupon =
                                                          MASTERdomain.facebookCoupon.HasValue
                                                              ? MASTERdomain.coupon.code
                                                              : ""
                                                  },
                                    policies = new PolicySettings()
                                                   {
                                                       paymentTerms = MASTERdomain.paymentTerms,
                                                       refundPolicy = MASTERdomain.returnPolicy
                                                   }
                                };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Settings(bool storeEnabled, string motd, string storeName, string facebookCoupon)
        {
            MASTERdomain.organisation.motd = motd;
            MASTERdomain.storeName = storeName;

            // website verificaiton
            bool verifySite = false;
            Webmaster webmaster = null;
            SitesEntry site = null;

            // store
            if (storeEnabled)
            {
                // check that payment methods have been configured
                var paymentmethods = new PaymentMethodList();
                paymentmethods.Initialise(MASTERdomain, false);
                if (paymentmethods.count == 0)
                {
                    return Json("You need to specify your paypal id and/or a payment method before you can enable your store. <a href='/dashboard/account#account_payment'>Setup payment methods</a>".ToJsonFail());
                }

                // don't allow store to be enabled
                if (MASTERdomain.trialExpired)
                {
                    return Json("Store cannot be made public. Your trial period has expired.".ToJsonFail());
                }

                // only register the following with google if store not already enabled
                if (!IsStoreEnabled)
                {
                    MASTERdomain.flags |= (int)SubdomainFlags.STORE_ENABLED;

#if !DEBUG
                    // update info to google webmaster 
                    webmaster = new Webmaster();
                    var url = accountHostname.ToDomainUrl("", true);
                    site = webmaster.AddSite(url);

                    if (site != null)
                    {
                        var metaTag = site.VerificationMethod.Value;
                        MASTERdomain.metaTagVerification = metaTag;
                        verifySite = true;
                    }
#endif
                }
            }
            else
            {
                MASTERdomain.flags &= ~(int)SubdomainFlags.STORE_ENABLED;
            }

            // handle facebook coupon
            if (!string.IsNullOrEmpty(facebookCoupon))
            {
                var coupon =
                    repository.GetCoupons(subdomainid.Value).SingleOrDefault(x => x.code == facebookCoupon && !x.expired);
                if (coupon != null)
                {
                    MASTERdomain.facebookCoupon = coupon.id;
                }
            }
            else
            {
                MASTERdomain.facebookCoupon = null;
            }

            repository.Save(); // SUBMIT

            // need to be done here so that metatag is saved and then can be shown at store page
            if (verifySite)
            {
                var url = accountHostname.ToDomainUrl("", true);
                
                // verify site
                webmaster.VerifySite(url, site);

                // add sitemap
                new Thread(() =>
                               {
                                   var sitemap = string.Concat(url, "google/sitemap");
                                   webmaster.AddSitemap(url, sitemap);
                               }).Start();

            }

            CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomainid.Value.ToString());
            return Json(OPERATION_SUCCESSFUL.ToJsonOKData());
        }


    }
}
