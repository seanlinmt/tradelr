using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using clearpixels.Facebook;
using clearpixels.Facebook.OAuth;
using FacebookToolkit.Rest;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.facebook.app;

namespace tradelr.Controllers.facebook
{
    //[ElmahHandleError]
    public class fbappController : Controller
    {
        private readonly ITradelrRepository repository;

        public fbappController()
        {
            repository = new TradelrRepository();
        }

        /// <summary>
        /// accepts
        /// </summary>
        /// <param name="pageid"></param>
        /// <param name="address"></param>
        /// <param name="profileid"></param>
        /// <param name="token">signed request</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Configure(string pageid, string address, string affiliate, string profileid, string token)
        {
            bool isError = (string.IsNullOrEmpty(pageid) && string.IsNullOrEmpty(profileid)) ||
                           string.IsNullOrEmpty(address);

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
            catch
            {
                Syslog.Write(new Exception("FB: Unable to parse " + address));
                isError = true;
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
                     return View("Error", new FacebookPageViewModel
                                             {
                                                 errorMessage = "This is not a valid store address", 
                                                 pageID = pageid
                                             });
                }

                var mastersd = repository.GetSubDomains().SingleOrDefault(x => x.name == subdomain);

                if (mastersd == null)
                {
                    Syslog.Write("New FB subdomain:" + subdomain);
                    // TODO: create new account
                    var client = new OAuthFacebook(GeneralConstants.FACEBOOK_APP_ID, GeneralConstants.FACEBOOK_API_SECRET,
                                           GeneralConstants.HTTP_SECURE + "/fbapp/tab", "email");

                    if (!client.ValidateSignedRequest(token))
                    {
                        return View("Error", new FacebookPageViewModel
                        {
                            errorMessage = "There was an error processing your request",
                            pageID = pageid
                        });
                    }
                    var payload = client.ParseSignedRequest(token);

                    var facebook = new FacebookService(payload.oauth_token);
                    var fb_usr = facebook.People.GetUser("me");

                    try
                    {
                        var account = new Account(repository, fb_usr.email, subdomain, AccountPlanType.BASIC, affiliate);
                        var errorString = account.CreateAccountWithFacebookLogin(fb_usr);
                        if (!string.IsNullOrEmpty(errorString))
                        {
                            return View("Error", new FacebookPageViewModel
                            {
                                errorMessage = errorString,
                                pageID = pageid
                            });
                        }
                        mastersd = account.mastersubdomain;
                    }
                    catch (Exception ex)
                    {
                        Syslog.Write(ex);
                        return View("Error", new FacebookPageViewModel
                        {
                            errorMessage = "There was an error processing your request. Your store was not created. Please try again.",
                            pageID = pageid
                        });
                    }
                }

                // check if there's already an entry, we ignore if there's already an entry
                var existing =
                    repository.GetFacebookPage(pageid).SingleOrDefault(x => x.subdomainid == mastersd.id);

                if (existing == null)
                {
                    existing = new facebookPage { subdomainid = mastersd.id, pageid = pageid };
                    repository.AddFacebookPage(existing);
                }

                var viewmodel = new FacebookGalleryViewModel();
                viewmodel.InitGalleryView(existing, repository, false);

                return View("Gallery", viewmodel);
            }

            return View("Error",
                        new FacebookPageViewModel {pageID = pageid, errorMessage = "This is not a valid store address"});
        }

        /// <summary>
        /// handler for canvas page for tradelr facebook app
        /// </summary>
        public ActionResult Index()
        {
            return View();
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

            Syslog.Write(string.Format("FB app install by http://graph.facebook.com/{0}", identifier));
            return new EmptyResult();
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
                Syslog.Write(string.Format("FB app removed by <a href='http://graph.facebook.com/{0}' target='_blank'>{0}</a>", identifier));
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

        public ActionResult Store(string id)
        {
            return Redirect(id.ToTradelrDomainUrl(""));
        }

        public ActionResult Tab(string code, string signed_request)
        {
            var client = new OAuthFacebook(GeneralConstants.FACEBOOK_APP_ID, GeneralConstants.FACEBOOK_API_SECRET,
                                           GeneralConstants.HTTP_SECURE + "/fbapp/tab", "email");

            if (!string.IsNullOrEmpty(code))
            {
                client.AccessTokenGet(code);

                var access_token = client.token;

                // TODO: save token for use later?

                return Redirect(string.Format("http://www.facebook.com/add.php?api_key={0}&pages=1", GeneralConstants.FACEBOOK_APP_ID));
            }

            if (!string.IsNullOrEmpty(signed_request))
            {
                if (!client.ValidateSignedRequest(signed_request))
                {
                    throw new NotImplementedException();
                }
                var payload = client.ParseSignedRequest(signed_request);
                
                var fbpage = repository.GetFacebookPage(payload.page.id).SingleOrDefault();
                if (fbpage != null)
                {
                    var viewModel = new FacebookGalleryViewModel();
                    viewModel.InitGalleryView(fbpage, repository, payload.page.liked);

                    var owner = fbpage.MASTERsubdomain.organisation.users.FirstOrDefault();
                    if (owner != null && owner.FBID == payload.user_id)
                    {
                        viewModel.isOwner = true;
                        viewModel.token = payload.oauth_token;
                    }

                    return View("Gallery", viewModel);
                }

                return View("Configure", new FacebookConfigureViewModel()
                                             {
                                                 pageID = payload.page.id,
                                                 profileID = payload.user_id,
                                                 signed_request = signed_request
                                             });
            }

            throw new NotImplementedException();
        }
    }
}
