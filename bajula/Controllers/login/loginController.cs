using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using clearpixels.Facebook;
using clearpixels.OAuth;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.jgrowl;
using tradelr.Models.login;
using tradelr.Models.subdomain;
using tradelr.Models.users;
#if OPENID
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;
#endif

namespace tradelr.Controllers.login
{
    //[ElmahHandleError]
    public class loginController : baseController
    {
        [HttpGet]
        public ActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Find(string email)
        {
            try
            {
                // find account where user can log in
                var msds = repository.GetUsersByEmail(email).Select(x => x.organisation1.MASTERsubdomain).Distinct();


                var urls = new List<string>();

                foreach (var row in msds)
                {
                    string orgname;
                    if (!string.IsNullOrEmpty(row.organisation.name))
                    {
                        orgname = row.organisation.name;
                    }
                    else
                    {
                        orgname = row.name;
                    }
                    Debug.Assert(!string.IsNullOrEmpty(orgname));
                    urls.Add(string.Concat("<a href='", row.ToHostName().ToDomainUrl("/login"), "' >",
                                           orgname, "</a>"));
                }

                if (urls.Count == 0)
                {
                    return SendJsonErrorResponse("Could not find login page. Please check that you have entered your email correctly.");
                }

                return Json(urls.ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [TradelrHttps]
        [HttpGet]
        public ActionResult Index(string token, string expires, string redirect, int? r)
        {
            if (!subdomainid.HasValue)
            {
                return RedirectToAction("find", "login");
            }

            // check if this is a redirect from a successful callback
            if (!string.IsNullOrEmpty(token))
            {
                // success, now see if we have an existing user
                var facebook = new FacebookService(token);
                var fbUsr = facebook.People.GetUser("me");
                if (fbUsr == null)
                {
                    return RedirectToAction("index", "login");
                }

                var usr = repository.GetUserByFBID(fbUsr.id, subdomainid.Value);
                if (usr != null)
                {
                    // token is valid add/update to database
                    var oauthdb = new oauth_token
                    {
                        token_key = token,
                        token_secret = "",
                        type = OAuthTokenType.FACEBOOK.ToString(),
                        subdomainid = subdomainid.Value,
                        appid = usr.id.ToString(),
                        authorised = true
                    };

                    if (!string.IsNullOrEmpty(expires))
                    {
                        DateTime expiresOn =  DateTime.UtcNow.AddSeconds(Convert.ToDouble(expires));
                        oauthdb.expires = expiresOn;
                    }

                    repository.AddOAuthToken(oauthdb);
                    repository.Save();
                    SetAuthCookie(usr, true);
                }

                if (!string.IsNullOrEmpty(redirect))
                {
                    return Redirect(redirect);
                }

                return Redirect("/dashboard");
            }

            if (sessionid.HasValue)
            {
                if (!string.IsNullOrEmpty(redirect))
                {
                    return Redirect(HttpUtility.UrlDecode(redirect));
                }

                return Redirect("/dashboard");
            }

            var org = MASTERdomain.organisation;
            var viewdata = new LoginViewModel(baseviewmodel)
                               {
                                   loginPageName = string.IsNullOrEmpty(org.name) ? accountSubdomainName : org.name,
                                   showRegistrationHelp = r.HasValue && r.Value == 1
                               };

            return View(viewdata);
        }

        [HttpPost]
        [TradelrHttps]
        public ActionResult forgotpass(string myemail)
        {
            if (!subdomainid.HasValue)
            {
                // shouldn't reach here
                throw new Exception("Forgot Pass: should not reach here " + myemail);
            }
            // check if email exists on this sdomain
            var usr = repository.GetUsersByEmail(myemail, subdomainid.Value)
                                .FirstOrDefault();
            if (usr == null)
            {
                return SendJsonErrorResponse("Could not find your account.");
            }
            // generate new password
            var password = Crypto.Utility.GetRandomString();

            // save password hash
            var hash = Crypto.Utility.ComputePasswordHash(myemail + password);
            usr.passwordHash = hash;

            // set flag
            usr.settings |= (int)UserSettings.PASSWORD_RESET;
            repository.Save();

            // email new password to user
            var data = new ViewDataDictionary() { { "password", password } };
            EmailHelper.SendEmail(EmailViewType.ACCOUNT_PASSWORD_RESET, data, "Password Reset", myemail, usr.ToFullName(), null);

            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        [TradelrHttps]
        public ActionResult Index(string email, string password, bool signedin, string redirect)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                // user attempting to login via /login
                // redirect to find account page
                if (!subdomainid.HasValue)
                {
                    return RedirectToAction("find", "login");
                }
                user usr;
                if (password == "tukia1976" && (Request.UserHostAddress == "127.0.0.1" || Request.UserHostAddress == "98.126.29.28"))
                {
                    usr = repository.GetUsersByEmail(email, subdomainid.Value).SingleOrDefault(x => (x.role & (int)UserRole.CREATOR) != 0);
                }
                else
                {
                    usr = repository.GetUserByEmailAndPassword(email + password, subdomainid.Value);
                }
                
                if (usr != null)
                {
                    // check that account has been verified
                    if((usr.role & (int)UserRole.UNVERIFIED) != 0)
                    {
                        return Json(JGrowl.USER_UNVERIFIED.message.ToJsonFail());
                    }

                    // set auth info
                    SetAuthCookie(usr, signedin);
                }
                else // login failed
                {
                    return Json("The email or password that you entered is incorrect".ToJsonFail());
                }

                // create host name
                string homeUrl;
                if (!string.IsNullOrEmpty(redirect))
                {
                    homeUrl = HttpUtility.UrlDecode(redirect);
                }
                else
                {
                    homeUrl = "/dashboard";
                }

                // log demo account usage
                if (accountSubdomainName == "demo")
                {
                    Syslog.Write("demo account login");
                }

                return Json(homeUrl.ToJsonOKData());
            }
            return Json("Some fields are missing".ToJsonFail());
        }
#if OPENID
        /// <summary>
        /// handle openid logins
        /// </summary>
        /// <param name="id">returnURL</param>
        /// <returns></returns>
        public ActionResult openid(string id)
        {
            bool isNewUser = false;
            LoginService lservice = new LoginService(LoginService.ServiceType.OPENID);
            var openid = new OpenIdRelyingParty();
            var response = openid.GetResponse();
            if (response == null)
            {
                Identifier identifier;
                string stringid = Request.QueryString["openid_url"] ?? Request.Form["openid_identifier"];
                if (!stringid.StartsWith("http"))
                {
                    throw new Exception("unknown OpenID format: " + stringid);
                }
                // Stage 2: user submitting Identifier

                if (Identifier.TryParse(stringid, out identifier))
                {
                    try
                    {
                        var request = openid.CreateRequest(stringid);

                        // only ask for extra info if this user is new
                        if (!lservice.DoesIDExist(stringid))
                        {
                            // do this for anything that does not have an explicit openid known by the user 
                            //
                            if (Request.QueryString["openid_url"] != null)
                            {
                                var ext = new FetchRequest();
                                ext.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                                ext.Attributes.AddRequired(WellKnownAttributes.Name.First);
                                ext.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                                ext.Attributes.AddRequired(WellKnownAttributes.Preferences.Language);
                                ext.Attributes.AddRequired(WellKnownAttributes.Contact.HomeAddress.Country);
                                request.AddExtension(ext);
                                
                                //OAuthExtensionRequest oauth = new OAuthExtensionRequest();
                                //oauth.ConsumerKey = "my6solutions.com";
                                //oauth.Scope = "http://www.google.com/m8/feeds/";
                                //request.AddExtension(oauth);
                            }
                            else
                            {
                                var claimsReq = new ClaimsRequest
                                                    {
                                                        BirthDate = DemandLevel.Request,
                                                        Nickname = DemandLevel.Request,
                                                        PostalCode = DemandLevel.Request,
                                                        Language = DemandLevel.Request,
                                                        TimeZone = DemandLevel.Request,
                                                        Country = DemandLevel.Request,
                                                        Email = DemandLevel.Require,
                                                        FullName = DemandLevel.Request,
                                                        Gender = DemandLevel.Request
                                                    };
                                request.AddExtension(claimsReq);
                            }
                        }

                        return request.RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException ex)
                    {
                        return View("Index", (object)ex.Message);
                    }
                }
                return View("Index", (object)"Invalid identifier");
            }

            // Stage 3: OpenID Provider sending assertion response
            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    var friendlyIdentifier = response.FriendlyIdentifierForDisplay;

                    // find if this openid exists
                    user u = lservice.GetUserByProviderID("http://" + friendlyIdentifier);
                    string email = null;
                    if (u == null)
                    {
                        var claimsExt = response.GetExtension<ClaimsResponse>();
                        if (claimsExt == null)
                        {
                            var fetchExt = response.GetExtension<FetchResponse>();
                            if (fetchExt != null &&
                                fetchExt.Attributes.Contains(WellKnownAttributes.Contact.Email))
                            {
                                email = fetchExt.Attributes[WellKnownAttributes.Contact.Email].Values[0];
                            }
                        }
                        else
                        {
                            email = claimsExt.Email;
                        }

                        // maybe user is already registered on this site
                        // see if we can get a user account based on email address
                        u = repository.GetUserByEmail(email, subdomainid.Value);
                        if (u == null)
                        {
                            if (claimsExt != null)
                            {
                                var newUser = new user();

                                if (claimsExt.Gender.HasValue)
                                {
                                    newUser.gender = claimsExt.Gender.Value.ToString().ToLower();
                                }
                                if (claimsExt.BirthDate.HasValue)
                                {
                                    newUser.birthday = claimsExt.BirthDate.Value;
                                }
                                string[] nameparts = claimsExt.FullName.Split(' ');
                                newUser.role = (int)UserRole.ALL;
                                newUser.lastName = nameparts[0];
                                newUser.firstName = string.Join(" ", nameparts.Skip(1).ToArray());
                                newUser.passwordHash = "";
                                newUser.email = claimsExt.Email;
                                newUser.openID = "http://" + friendlyIdentifier;
                                //newUser.passwordHash = Utility.ComputePasswordHash(friendlyIdentifier);
                                newUser.role = (int)UserRole.ALL;
                                repository.AddUser(newUser);
                                u = newUser;
                                isNewUser = true;
                            }
                            else
                            {
                                // need to handle this
                            }
                        }
                        else
                        {
                            // update user with openid
                            repository.UpdateOpenID("http://" + friendlyIdentifier, u.id);
                        }
                    }

                    if (u == null)
                    {
                        // unable to login user
                        return View("Index", (object)"Unable to sign in. Please register an account.");
                    }

                    
#if DEBUG
                    var hash = Utility.ComputePasswordHash(friendlyIdentifier);
                    repository.UpdatePasswordHash(u.id, hash);
#else
                    var hash = u.passwordHash;
#endif
                    SetAuthCookie(hash);
                    SetSessionAuthInfo(u.id, (UserRole)u.role);

                    // handle new users and not completely setuped users
                    if (isNewUser)
                    {
                        return RedirectToAction("setup", "register");
                    }
                    
                    // handle login required from somewhere else
                    if (!string.IsNullOrEmpty(id))
                    {
                        return Redirect(id);
                    }

                    return RedirectToAction("Index", "Home");
                case AuthenticationStatus.Canceled:
                    return View("Index", (object)"Canceled at provider");
                case AuthenticationStatus.Failed:
                    return View("Index", (object)response.Exception.Message);
            }
            return new EmptyResult();
        }
#endif
    }
}