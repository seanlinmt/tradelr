using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;
using clearpixels.Models;
using tradelr.Common;
using tradelr.Common.Models.currency;
using tradelr.Controllers.error;
using tradelr.Controllers.liquid;
using tradelr.Crypto;
using tradelr.Crypto.token;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models;
using tradelr.Models.account;
using tradelr.Models.account.plans;
using tradelr.Models.jgrowl;
using tradelr.Models.liquid;
using tradelr.Models.liquid.models;
using tradelr.Models.liquid.models.Blog;
using tradelr.Models.liquid.models.Cart;
using tradelr.Models.liquid.models.LinkList;
using tradelr.Models.mobile;
using tradelr.Models.store;
using tradelr.Models.subdomain;
using tradelr.Models.users;
using System.Linq;
using Utility = tradelr.Library.Utility;

namespace tradelr.Controllers
{
    [HandleMobileBrowser]
    public abstract class baseController : Controller
    {
        private const int COOKIE_LIFETIME = 1209600; // 14 days 
#if DEBUG
        private const int COOKIE_LIFETIME_MIN = 300; // 5 minutes 
#else
        private const int COOKIE_LIFETIME_MIN = 1200; // 20 mins 
#endif
        private TradelrSecurityToken token;

        protected readonly ITradelrRepository repository;
        protected readonly tradelrDataContext db;
        protected const string OPERATION_SUCCESSFUL = "OK";
        protected long? sessionid;
        protected long? subdomainid;
        protected string accountHostname;
        protected string accountSubdomainName;
        protected UserRole role;
        protected UserPermission permission;
        protected AccountPlanLimits accountLimits;
        protected SubdomainStats stats;
        protected SubdomainFlags subdomainFlags;
        protected bool IsStoreEnabled;
        protected readonly BaseViewModel baseviewmodel;
        protected MASTERsubdomain MASTERdomain;

        protected baseController()
        {
            db = new tradelrDataContext();
            repository = new TradelrRepository(db);
            permission = UserPermission.NONE;
            baseviewmodel = new BaseViewModel();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
            if (Request.RawUrl.Contains("?redirect="))
            {
                baseviewmodel.notifications = "Session expired. Please log in again.".ToNotification().ToJson();
            }
            
            base.OnActionExecuted(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            accountHostname = Request.Headers["Host"];
#if DEBUG
            if (accountHostname == "local")
            {
                filterContext.Result = new RedirectResult(GeneralConstants.HTTP_HOST + Request.Url.PathAndQuery);
                return;
            }
#else
            if (accountHostname == null || accountHostname == "tradelr.com" || accountHostname == "98.126.29.28")
            {
                filterContext.Result = new RedirectResult(GeneralConstants.HTTP_HOST + Request.Url.PathAndQuery);
                return;
            }
#endif
            ////////////////////// subdomain check ///////////////////////////////
#if DEBUG
            if (accountHostname.EndsWith("localhost"))
#else
            if (accountHostname.EndsWith("tradelr.com"))
#endif
            {
                ////////////// handles case for subdomains
                string[] host = accountHostname.Split('.');
                string hostSegment = "";
                
                // not on a subdomain
                if (!Utility.IsOnSubdomain(host, out hostSegment))
                {
                    return;
                }

                MASTERdomain = db.GetSubDomain(hostSegment);

                // ensure that incoming host name matches x.tradelr.com.
                // this is to handle www.x.tradelr.com returning www.tradelr.com
                if (MASTERdomain != null)
                {
#if DEBUG
                    var validHost = string.Format("{0}.localhost", hostSegment);
#else
                    var validHost = string.Format("{0}.tradelr.com", hostSegment);
#endif
                    if (validHost != accountHostname)
                    {
                        filterContext.Result = new RedirectResult(string.Format("{0}://{1}", Request.Url.Scheme, validHost));
                        return;
                    }
                }
            }
            else
            {
                ////////////////// handles case for custom subdomains
                MASTERdomain = db.GetCustomHostname(accountHostname);
            }
            
            if (MASTERdomain == null)
            {
                // subdomain does not exist
                filterContext.RequestContext.HttpContext.Response.StatusCode = HttpStatusCode.NotFound.ToInt();
                filterContext.Result = new RedirectResult(GeneralConstants.HTTP_HOST);
                return;
            }

            /////////// SUBDOMAIN EXISTS
            accountLimits = MASTERdomain.accountType.ToEnum<AccountPlanType>().ToAccountLimit();
            accountSubdomainName = MASTERdomain.name;
            subdomainid = MASTERdomain.id;
            stats = MASTERdomain.ToSubdomainStats();
            IsStoreEnabled = MASTERdomain.IsStoreEnabled();
            baseviewmodel.storeEnabled = IsStoreEnabled;
            subdomainFlags = (SubdomainFlags) MASTERdomain.flags;

            if ((MASTERdomain.flags & (int)SubdomainFlags.OFFLINE_ENABLED) != 0)
            {
                var browsertype = Request.Browser.Type.ToLower();
                if (browsertype.Contains("safari") || browsertype.Contains("chrome"))
                {
                    baseviewmodel.manifestFile = "manifest=\"/manifest\"";
                }
            }
            baseviewmodel.orgName = MASTERdomain.organisation.name;
            baseviewmodel.shopUrl = accountHostname;

            /////////////////////// session check ///////////////////////////////
            token = Request.RequestContext.HttpContext.Items["token"] as TradelrSecurityToken;

            if (token == null)
            {
                GetAuthCookie();
            }

            if (token != null)
            {
                Request.RequestContext.HttpContext.Items["token"] = token;
                sessionid = token.UserID;
                var usr = db.GetUserById(sessionid.Value, subdomainid.Value);
                if (usr != null)
                {
                    baseviewmodel.notifications = usr.ToNotification(MASTERdomain.trialExpired).ToJson();

                }

                role = token.UserRole.ToEnum<UserRole>();
                baseviewmodel.role = role;

                permission = token.Permission.ToEnum<UserPermission>();
                baseviewmodel.permission = permission;
            }

            base.OnActionExecuting(filterContext);
        }

        protected string GetCartIdFromCookie()
        {
            var cartid = "";
            // check if there is a cookie
            if (Request.Cookies["cart"] != null)
            {
                cartid = HttpUtility.UrlDecode(Request.Cookies["cart"].Value);
            }

            return cartid;
        }

        protected LiquidTemplate    CreateLiquidTemplate(string templateName, string page_title)
        {
            var template = new LiquidTemplate(MASTERdomain, (bool)Session[BrowserCapability.IsMobileSession]);
            template.InitLayoutTemplate("layout/theme.liquid");
            if (!string.IsNullOrEmpty(page_title))
            {
                template.AddParameters("page_title", page_title);
            }
            template.AddParameters("template", templateName);
            template.AddParameters("linklists", new LinkLists(MASTERdomain));
            template.AddParameters("pages", new Pages(MASTERdomain));
            template.AddParameters("blogs", new Blogs(MASTERdomain));
            template.AddParameters("collections", new Collections(MASTERdomain, sessionid));
            template.AddParameters("powered_by_link", "<a href='http://www.tradelr.com'>This online store is powered by tradelr</a>");

            // settings
            var settings_file = string.Format("{0}/{1}/config/settings_data.json", GeneralConstants.APP_ROOT_DIR, template.handler.GetThemeUrl());
            template.AddParameters("settings", template.ReadThemeSettings(settings_file)); 

            // cart
            var cartid = GetCartIdFromCookie();
            var liquidCart = new Cart();
            if (!string.IsNullOrEmpty(cartid))
            {
                var cart = MASTERdomain.carts.SingleOrDefault(x => x.id.ToString() == cartid);
                if (cart != null)
                {
                    if (cart.orderid.HasValue)
                    {
                        // cart already processed. clear cookie
                        Response.Cookies["cart"].Expires = DateTime.UtcNow.AddMonths(-1);
                    }
                    else
                    {
                        liquidCart = cart.ToLiquidModel(sessionid);
                    }
                }
            }
            template.AddParameters("cart", liquidCart);
            template.AddParameters("shop", MASTERdomain.ToLiquidModel());

            template.AddHeaderContent(MASTERdomain.metaTagVerification);

            // login panel
            template.AddParameters("login_for_store",
                                   this.RenderViewToString("~/Views/store/liquid/loginPanel.ascx",
                                                           new LoginPanel()
                                                               {
                                                                   isLoggedIn =sessionid.HasValue,
                                                                   hostName = accountHostname.ToDomainUrl()
                                                               }));
            // add filters
            // currency
            var currency = MASTERdomain.currency.ToCurrency();
            template.AddFilterValues("currency", currency);

            // add registers
            template.AddRegisters("current_url", Request.Url);

            // add term form date
            if (TempData["form"] != null)
            {
                template.AddParameters("posted_contact_form", TempData["form"]);
            }

            return template;
        }

        protected LiquidTemplate CreatePageMissingTemplate()
        {
            var template = CreateLiquidTemplate("404", "Page not found");
            template.InitContentTemplate("templates/404.liquid");

            return template;
        }

        protected ActionResult ReturnError(string message)
        {
            Syslog.Write(new Exception(message));
            return RedirectToAction("Index","error");
        }

        protected ActionResult RedirectToLogin()
        {
            return Redirect("/login?redirect=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery));
        }

        protected ActionResult RedirectToUnsecure()
        {
            var redirectUrl = string.Concat("http://", Request.Url.Authority, Request.Url.PathAndQuery);
            return Redirect(redirectUrl);
        }

        private void GetAuthCookie()
        {
            if (Request.Cookies != null)
            {
                var httpCookie = Request.Cookies["token"];
                if (httpCookie != null)
                {
                    try
                    {
                        token = new TradelrSecurityToken(httpCookie.Value);
                    }
                    catch (Exception ex)
                    {
                        // expired, clear cookie
                        ClearAuthCookie();
                        ClearOldCookie();
                    }
                }
            }
        }

        protected void SetAuthCookie(user usr, bool rememberme)
        {
            DateTime expires;
            if (rememberme)
            {
                expires = DateTime.UtcNow.AddSeconds(COOKIE_LIFETIME);
            }
            else
            {
                expires = DateTime.UtcNow.AddSeconds(COOKIE_LIFETIME_MIN);
            }

            ///// handle permissions
            if (usr.permissions.HasValue)
            {
                permission = (UserPermission)usr.permissions.Value;
            }
            else
            {
                permission = ((UserRole)usr.role).HasFlag(UserRole.CREATOR) ? UserPermission.ADMIN : UserPermission.USER;
            }

            token = new TradelrSecurityToken(usr.id.ToString(), usr.role.ToString(), permission.ToInt().ToString(), expires);
            
            Response.Cookies["token"].Value = token.Serialize();
            Response.Cookies["token"].Expires = expires;
            Response.Cookies["token"].HttpOnly = true;
#if SUPPORT_HTTPS
            Response.Cookies["token"].Secure = true;
#endif
            // can't log out if the following is set
            //Response.Cookies["token"].Domain = accountHostname;

            // update last login
            usr.lastLogin = DateTime.UtcNow;
            repository.Save();
        }

        protected void ClearAuthCookie()
        {
            var httpCookie = Response.Cookies["token"];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddMonths(-1);
            }
        }

        protected void ClearOldCookie()
        {
            var httpCookie = Response.Cookies["auth"];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddMonths(-1);
            }
        }

        protected ActionResult SendJsonErrorResponse(Exception ex)
        {
            ErrorSignal.FromCurrentContext().Raise(ex);
            return Json("Sorry! We did something wrong and we're looking into it now.".ToJsonFail());
        }
        protected ActionResult SendJsonErrorResponse(string message)
        {
            ErrorSignal.FromCurrentContext().Raise(new Exception(message));
            return Json(HttpUtility.HtmlEncode(message).ToJsonFail());
        }

        protected ActionResult SendJsonSessionExpired()
        {
            //Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return Json("Session Expired".ToJsonFail());
        }
    }
}
