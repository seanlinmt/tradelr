using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using tradelr.Libraries.ActionFilters;
using tradelr.Models.users;

namespace tradelr.Controllers
{
    public class sslController : Controller
    {
        /// <summary>
        /// obtains external non SSL files through our site
        /// </summary>
        /// <returns></returns>
        [RoleFilter(role = UserRole.USER)]
        [OutputCache(VaryByParam = "url", Duration = 3600)]
        public ActionResult Index(string url)
        {
            var request = WebRequest.Create(HttpUtility.UrlDecode(url));
            var response = request.GetResponse();
            return new FileStreamResult(response.GetResponseStream(), response.ContentType);
        }

    }
}
