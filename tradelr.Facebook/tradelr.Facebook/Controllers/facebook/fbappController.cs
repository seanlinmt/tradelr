using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Facebook.OAuth;
using FacebookToolkit.Rest;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Facebook.ActionFilters;
using tradelr.Facebook.Models.facebook;
using tradelr.Library.Constants;
using tradelr.Library;
using tradelr.Logging;
using tradelr.Models.account;
using tradelr.Models.oauth;
using tradelr.Models.photos;
using tradelr.Models.shipping;
using tradelr.Models.users;

namespace tradelr.Facebook.Controllers.facebook
{
    [ElmahHandleError]
    public class fbappController : Controller
    {
        private readonly ITradelrRepository repository;

        public fbappController()
        {
            repository = new TradelrRepository();
        }

        public ActionResult Configure(string pageid, string address, PageType? pagetype, string profileid, string fb_sig_user)
        {
            bool isError = false;
            string identifier = "";

            if ((string.IsNullOrEmpty(pageid) && string.IsNullOrEmpty(profileid)) || 
                string.IsNullOrEmpty(address))
            {
                isError = true;
            }
            if (!string.IsNullOrEmpty(pageid))
            {
                identifier = pageid;
            }
            else if (!string.IsNullOrEmpty(profileid))
            {
                identifier = profileid;
            }
            else
            {
                isError = true;
                Syslog.Write(new Exception("NULL FB identifier"));
            }

            Uri storeAddress = null;
            try
            {
                if (address.IndexOf('.') == -1)
                {
                    // user only enter store name
                    address = string.Format("{0}.tradelr.com", address);
                }
                if (!address.StartsWith("http"))
                {
                    // user did not enter http
                    address = string.Format("http://{0}", address);
                }
                storeAddress = new Uri(address);
            }
            catch (Exception ex)
            {
                Syslog.Write(new Exception("FB: Unable to parse " + address));
                isError = true;
            }

            // pagetype can be empty if from a profile
            if (!pagetype.HasValue)
            {
                pagetype = PageType.PROFILE;
            }

            if (!isError)
            {
                string subdomain = "";
                if (storeAddress.Host.Split('.').Length > 2)
                {
                    int lastIndex = storeAddress.Host.LastIndexOf(".");
                    int index = storeAddress.Host.LastIndexOf(".", lastIndex - 1);
                    subdomain = storeAddress.Host.Substring(0, index);
                }
                else
                {
                    isError = true;
                }

                var mastersd = repository.GetSubDomains().Where(x => x.name == subdomain).SingleOrDefault();

                if (mastersd == null)
                {
                    Syslog.Write(ErrorLevel.INFORMATION, "New FB subdomain:" + subdomain);
                    // create new account
                    return RedirectToAction("Redirect", new
                                                            {
                                                                url =
                                                            string.Format(
                                                                "{0}/newaccount?id={1}&identifier={2}&pagetype={3}",
                                                                GeneralConstants.FACEBOOK_APP_URL, subdomain, identifier,
                                                                pagetype.Value)
                                                            });
                }

                // check if there's already an entry, we ignore if there's already an entry
                var existing =
                    repository.GetFacebookPage(identifier).Where(x => x.subdomainid == mastersd.id).SingleOrDefault();

                if (existing == null)
                {
                    var newEntry = new facebookPage { subdomainid = mastersd.id, pageid = identifier };
                    repository.AddFacebookPage(newEntry);
                }
            }

            string returnUrl = pagetype.Value.ToReturnUrl(identifier);
            var viewdata = new FacebookViewData
                               {pageUrl = returnUrl, errorMessage = "This is not a valid store address"};
            if (isError)
            {
                return View("Error",viewdata);
            }

            return RedirectToAction("Redirect", new { url = returnUrl });
        }

        public ActionResult NewAccount(string code, string id, string identifier, PageType pagetype)
        {
            var client = new OAuthFacebook(GeneralConstants.FACEBOOK_API_KEY, GeneralConstants.FACEBOOK_API_SECRET,
                                           HttpUtility.UrlEncode(
                                               string.Format(
                                                   "{0}/newaccount/{1}?identifier={2}&pagetype={3}",
                                                   GeneralConstants.FACEBOOK_APP_URL, id, identifier, pagetype)),
                                                   "read_stream,email,publish_stream,offline_access,manage_pages");

            // starting our authorisation process
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Redirect", new{ url = client.AuthorizationLinkGet()});
            }

            if (!client.AccessTokenGet(code))
            {
                return View("Error", new FacebookViewData {errorMessage = "Unable to obtain permission", pageUrl = pagetype.ToReturnUrl(identifier)});
            }

            // check subdomain is valid
            id = id.ToLower();

            // also check special domain list
            if (GeneralConstants.SUBDOMAIN_RESTRICTED.Contains(id))
            {
                return View("Error", new FacebookViewData { errorMessage = "Store address is not available", pageUrl = pagetype.ToReturnUrl(identifier) });
            }

            var mastersubdomain = repository.GetSubDomains().Where(x => x.name == id).SingleOrDefault();
            if (mastersubdomain != null)
            {
                return View("Error", new FacebookViewData { errorMessage = "Store address is not available", pageUrl = pagetype.ToReturnUrl(identifier) });
            }

            var facebook = new FacebookService(client.token);
            var fb_usr = facebook.People.GetUser("me");
            if (fb_usr == null)
            {
                return View("Error", new FacebookViewData { errorMessage = "Unable to create account", pageUrl = pagetype.ToReturnUrl(identifier) });
            }

            // verify that email has not been used to register another account
            if (repository.GetUserByEmail(fb_usr.email).Where(x => (x.role & (int)UserRole.CREATOR) != 0).SingleOrDefault() != null)
            {
                Syslog.Write(ErrorLevel.INFORMATION, "Facebook email address in use: " + fb_usr.email);
                return View("Error", new FacebookViewData { errorMessage = "Email address is already registered", pageUrl = pagetype.ToReturnUrl(identifier) });
            }

            var usr = new user
            {
                role = (int)UserRole.ADMIN,
                viewid = Crypto.Utility.GetRandomString(),
                permissions = (int)UserPermission.ADMIN,
                FBID = fb_usr.id,
                email = fb_usr.email ?? "",
                externalProfileUrl = fb_usr.link,
                firstName = fb_usr.first_name,
                lastName = fb_usr.last_name,
                gender = fb_usr.gender,
                externalProfilePhoto = string.Format("https://graph.facebook.com/{0}/picture?type=large", fb_usr.id)
            };

            // create subdomain entry
            mastersubdomain = new MASTERsubdomain
            {
                flags = 0,
                name = id,
                total_outofstock = 0,
                total_contacts_public = 0,
                total_contacts_private = 0,
                total_contacts_staff = 0,
                total_invoices_sent = 0,
                total_invoices_received = 0,
                total_orders_sent = 0,
                total_orders_received = 0,
                total_products_mine = 0,
                accountType = AccountPlanType.ULTIMATE.ToString()
            };

            repository.AddMasterSubdomain(mastersubdomain);

            // create organisation first
            var org = new organisation
            {
                subdomain = mastersubdomain.id,
                name = fb_usr.name
            };

            repository.AddOrganisation(org);
            usr.organisation = org.id;

            // CREATE DEFAULT STRUCTURES
            // add default inventory location
            var loc = new inventoryLocation
            {
                name = GeneralConstants.INVENTORY_LOCATION_DEFAULT,
                subdomain = mastersubdomain.id,
                lastUpdate = DateTime.UtcNow
            };
            repository.AddInventoryLocation(loc, mastersubdomain.id);

            // add default shipping profile
            var shippingProfile = new shippingProfile()
            {
                title = "Default",
                type = ShippingProfileType.FLATRATE.ToString(),
                subdomainid = mastersubdomain.id
            };
            repository.AddShippingProfile(shippingProfile);

            // update subdomain entry
            mastersubdomain.creator = org.id;

            // create facebookpage to link to subdomain
            var newEntry = new facebookPage { subdomainid = mastersubdomain.id, pageid = identifier };
            repository.AddFacebookPage(newEntry);


            try
            {
                // if user exist then we still need to verify email
                Random rnd = RandomNumberGenerator.Instance;
                usr.confirmationCode = rnd.Next();
                repository.AddUser(usr);

                // generate photo
                new Thread(() => usr.externalProfilePhoto.ReadAndSaveFromUrl(mastersubdomain.id, usr.id, usr.id, PhotoType.PROFILE)).Start();

                // add access token
                var oauthdb = new oauth_token
                {
                    token_key = client.token,
                    token_secret = "",
                    type = OAuthTokenType.FACEBOOK.ToString(),
                    subdomainid = mastersubdomain.id,
                    appid = usr.id.ToString(),
                    authorised = true
                };
                repository.AddOAuthToken(oauthdb);

                // obtain any other account tokens
                var accounts = facebook.Account.GetAccountTokens("me");
                if (accounts != null && accounts.data != null)
                {
                    foreach (var account in accounts.data)
                    {
                        if (account.name != null)
                        {
                            var ftoken = new facebook_token
                            {
                                pageid = account.id,
                                subdomainid = mastersubdomain.id,
                                accesstoken = account.access_token,
                                name = account.name
                            };
                            repository.AddUpdateFacebookToken(ftoken);
                        }
                    }
                }
                repository.Save();

                // send confirmation email
                var viewdata = new ViewDataDictionary()
                                   {
                                       {"host", id.ToSubdomainUrl()},
                                       {"confirmCode", usr.confirmationCode},
                                       {"email", usr.email}
                                   };
                EmailHelper.SendEmailNow(EmailViewType.ACCOUNT_CONFIRMATION, viewdata, "New Account Details and Email Verification Link",
                               usr.email, usr.ToFullName(), usr.id);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return View("Error", new FacebookViewData { errorMessage = "Unable to create account", pageUrl = pagetype.ToReturnUrl(identifier) });
            }

            return RedirectToAction("Redirect", new { url = pagetype.ToReturnUrl(identifier) });
        }

        /// <summary>
        /// handler for canvas page for tradelr facebook app
        /// </summary>
        public ActionResult Index(string auth_token, string fb_sig_user)
        {
            var viewdata = new FacebookViewData();

            return View(viewdata);
        }

        public ActionResult Install(string fb_sig_user, string fb_sig_page_id)
        {
            string identifier = "";
            if (!string.IsNullOrEmpty(fb_sig_user))
            {
                identifier = fb_sig_user;
            }
            if (!string.IsNullOrEmpty(fb_sig_page_id))
            {
                identifier = fb_sig_page_id;
            }

            Syslog.Write(ErrorLevel.INFORMATION, string.Format("FB app install by http://graph.facebook.com/{0}", identifier) );
            return new EmptyResult();
        }

        /// <summary>
        /// profile tab
        /// </summary>
        /// <returns></returns>
        public ActionResult Profile(string fb_sig_in_profile_tab, string fb_sig_page_id, string fb_sig_type,
            string fb_sig_profile_id, int? fb_sig_is_fan)
        {
            if (string.IsNullOrEmpty(fb_sig_in_profile_tab))
            {
                Syslog.Write(new Exception("Not from Page" + fb_sig_type + "," + fb_sig_page_id + "," + fb_sig_profile_id));
            }

            string pageType;
            if (!string.IsNullOrEmpty(fb_sig_type))
            {
                pageType = fb_sig_type == "APPLICATION" ? "APPLICATION" : "FANPAGE";
            }
            else
            {
                pageType = "PROFILE";
            }

            var viewdata = new FacebookViewData {isTab = true, pageID = fb_sig_page_id, pageType = pageType, profileID = fb_sig_profile_id};

            // look for an entry in db, if none found for this page then it has not been configured yet.
            string identifier = "";
            if (!string.IsNullOrEmpty(fb_sig_page_id))
            {
                identifier = fb_sig_page_id;
            }
            else if (!string.IsNullOrEmpty(fb_sig_profile_id))
            {
                identifier = fb_sig_profile_id;
            }

            var fbpage = repository.GetFacebookPage(identifier).SingleOrDefault();
            if (fbpage != null)
            {
                var subdomain = fbpage.MASTERsubdomain.name;
                var org = fbpage.MASTERsubdomain.organisation;
                // render store
                viewdata.isConfigured = true;
                viewdata.storeName = org.name;
                viewdata.gallery = new Gallery
                                       {
                                           viewAllUrl = string.Concat("http://", subdomain, ".tradelr.com"),
                                           categories = repository.GetProductCategories(null, fbpage.subdomainid)
                                           .Select(x => new SelectListItem(){Text = x.MASTERproductCategory.name, Value = x.id.ToString()}),
                                           products = repository.GetProducts(fbpage.subdomainid)
                                                    .OrderByDescending(x => x.id)
                                                    .Take(21)
                                                    .ToModel(subdomain)
                                       };
                // show logo if there is one
                if (fbpage.MASTERsubdomain.organisation.logo.HasValue)
                {
                    viewdata.logoUrl = string.Concat(viewdata.gallery.viewAllUrl, org.image.ToModel(Imgsize.BANNER).url);
                    viewdata.storeName = "";
                }

                // handle coupon message
                if (fbpage.MASTERsubdomain.facebookCoupon.HasValue)
                {
                    if (fb_sig_is_fan == 1)
                    {
                        var coupon = fbpage.MASTERsubdomain.coupon;
                        var currency = fbpage.MASTERsubdomain.currency.ToCurrency();
                        string description;
                        string minimumPurchase = " on all purchases.";
                        if (coupon.minimumPurchase.HasValue)
                        {
                            minimumPurchase = string.Format(" with a minimum purchase of {0}{1}.", currency.code,
                                                            coupon.minimumPurchase.Value.ToString("n" +
                                                                                                  currency.decimalCount));
                        }
                        if (coupon.couponPercentage.HasValue)
                        {
                            description = string.Format("{0}% discount", coupon.couponPercentage.Value.ToString("n2"));
                        }
                        else
                        {
                            description = string.Format("{0}{1} off", currency.code,
                                                        coupon.couponValue.Value.ToString("n" + currency.decimalCount));
                        }
                        viewdata.couponMessage = string.Format(
                            "<p><strong>Discount code:</strong> {0}</p><p>{1}{2}</p>", coupon.code, description,
                            minimumPurchase);
                    }
                    else
                    {
                        viewdata.couponMessage = string.Format("<p class='strong'><img src='{0}/Content/img/arrow_up.png' /> Click on the like button above to view the latest discount code.</p>",
                            GeneralConstants.FACEBOOK_HOST);
                    }
                }

                return View("Gallery", viewdata);
            }

            return View(viewdata);
        }

        public ActionResult Redirect(string url)
        {
            var viewdata = new FacebookViewData { pageUrl = url };

            return View(viewdata);
        }

        /// <summary>
        /// more info at http://wiki.developers.facebook.com/index.php/Post-Remove_Callback_URL
        /// </summary>
        /// <param name="fb_sig_user"></param>
        /// <param name="fb_sig_page_id"></param>
        /// <returns></returns>
        public ActionResult Uninstall(string fb_sig_user, string fb_sig_page_id)
        {
            string identifier = "";
            if (!string.IsNullOrEmpty(fb_sig_page_id))
            {
                identifier = fb_sig_page_id;
            }
            else if (!string.IsNullOrEmpty(fb_sig_user))
            {
                identifier = fb_sig_user;
            }
            var fbpage = repository.GetFacebookPage(identifier).SingleOrDefault();
            if (fbpage != null)
            {
                repository.DeleteFacebookPage(fbpage);
                Syslog.Write(ErrorLevel.INFORMATION, string.Format("FB app removed by <a href='http://graph.facebook.com/{0}' target='_blank'>{0}</a>", identifier));
            }
            return new EmptyResult();
        }

        private void PostAppAddMessage(Api api, string profileid)
        {
            if (string.IsNullOrEmpty(profileid))
            {
                Syslog.Write(new Exception("Cannot post to stream"));
                return;
            }

            var image = new attachment_media_image()
                            {
                                href = GeneralConstants.FACEBOOK_APP_URL,
                                src = string.Format("{0}/Content/img/logo2.png", GeneralConstants.FACEBOOK_HOST),
                type = attachment_media_type.image
            };

            var attachment = new attachment
            {
                media = new List<attachment_media>() { image },
                caption = GeneralConstants.FACEBOOK_APP_URL,
                name = "tradelr for Facebook",
                href = GeneralConstants.FACEBOOK_APP_URL,
                description = "Display your products from tradelr.com on Facebook."
            };
            try
            {
                api.Stream.Publish("added tradelr to my profile", attachment, null, null, long.Parse(profileid));
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            
        }
    }
}
