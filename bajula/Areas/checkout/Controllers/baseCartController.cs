using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using tradelr.Areas.checkout.Models;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Areas.checkout.Controllers
{
    //[ElmahHandleError]
    public abstract class baseCartController : Controller
    {
        protected readonly ITradelrRepository repository;
        protected readonly tradelrDataContext db;
        protected long? subdomainid;
        protected cart cart;
        protected long? sessionid;
        protected readonly BaseViewModel baseviewmodel;

        public baseCartController()
        {
            cart = null;
            db = new tradelrDataContext();
            repository = new TradelrRepository(db);
            baseviewmodel = new BaseViewModel();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            baseviewmodel.cartid = GetCartID(filterContext);
            baseviewmodel.isMobileDevice = false;
            if (Request.Browser.ScreenPixelsWidth < 1000 &&
                Request.Browser.ScreenPixelsHeight < 1000 &&
                Request.Browser.IsMobileDevice)
            {
                baseviewmodel.isMobileDevice = true;
            }

            if (!string.IsNullOrEmpty(baseviewmodel.cartid))
            {
                UpdateCartSession();
            }

            // send user straight to final view if we have already processed the cart
            if (cart != null && cart.orderid.HasValue 
                && !(string.Compare(RouteData.Values["controller"].ToString(),"order", true) == 0 && 
                    string.Compare(RouteData.Values["action"].ToString().ToLower(), "index", true) == 0))
            {
                filterContext.Result = new RedirectResult("/checkout/order/" + baseviewmodel.cartid);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetCartID(ActionExecutingContext filterContext)
        {
            // try get from route data
            var cartid = RouteData.Values["cartid"] as string;
            if (!string.IsNullOrEmpty(cartid))
            {
                return cartid;
            }

            // try get from urlreferer  (last char is a '/')
            if (filterContext.RequestContext.HttpContext.Request.UrlReferrer != null)
            {
                cartid = filterContext.RequestContext.HttpContext.Request.UrlReferrer.Segments[3];
            }

            return cartid;
        }

        private void UpdateCartSession()
        {

            if (Session["cartid"] == null)
            {
                if (!string.IsNullOrEmpty(baseviewmodel.cartid))
                {
                    cart = db.carts.Where(x => x.id.ToString() == baseviewmodel.cartid).SingleOrDefault();
                    if (cart != null)
                    {
                        Session["cartid"] = baseviewmodel.cartid;
                    }
                }
            }

            if (Session["cartid"] != null)
            {
                if (cart == null)
                {
                    cart = db.carts.Where(x => x.id.ToString() == baseviewmodel.cartid).SingleOrDefault();
                }
            }

            if (cart != null)
            {
                sessionid = cart.userid;
                subdomainid = cart.subdomainid;

                baseviewmodel.store_name = cart.MASTERsubdomain.organisation.name;
                baseviewmodel.year = DateTime.UtcNow.Year;
                if (cart.userid.HasValue)
                {
                    baseviewmodel.user_name = cart.user.firstName;
                }
            }
        }

        protected void UpdateUserSession(long sessionvalue)
        {
            sessionid = sessionvalue;
            baseviewmodel.user_name = cart.user.firstName;
        }
    }
}
