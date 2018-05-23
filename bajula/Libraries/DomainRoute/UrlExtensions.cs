using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.Mvc;
using tradelr.Libraries.DomainRoute;

// WARNING: changing the namespace will break
namespace System.Web.Mvc.Html
{
    public static class UrlExtensions
    {
        public static string Action(this UrlHelper urlHelper,string actionName, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, null, new RouteValueDictionary(), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper,string actionName, object routeValues, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, string controllerName, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, controllerName, new RouteValueDictionary(), new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, RouteValueDictionary routeValues, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, null, routeValues, new RouteValueDictionary(), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, object routeValues, object htmlAttributes, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, null, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, null, routeValues, htmlAttributes, requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, string controllerName, object routeValues, object htmlAttributes, bool requireAbsoluteUrl)
        {
            return urlHelper.Action(actionName, controllerName, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes), requireAbsoluteUrl);
        }

        public static string Action(this UrlHelper urlHelper, string actionName, string controllerName, RouteValueDictionary routeValues, bool requireAbsoluteUrl)
        {
            if (requireAbsoluteUrl)
            {
                HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
                RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);

                routeData.Values["controller"] = controllerName;
                routeData.Values["action"] = actionName;

                DomainRoute domainRoute = routeData.Route as DomainRoute;
                if (domainRoute != null)
                {
                    DomainData domainData = domainRoute.GetDomainData(new RequestContext(currentContext, routeData), routeData.Values);
                    return urlHelper.Action(actionName, controllerName, routeData.Values, domainData.Protocol, domainData.HostName);
                }
            }
            return urlHelper.Action(actionName, controllerName, routeValues);
        }
    }
}