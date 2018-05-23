using System.Web;
using System.Web.Mvc;
using tradelr.Library;

namespace tradelr.Controllers
{
    public class callbackController : Controller
    {
        //
        // GET: /redirect/
        public ActionResult Index(string sd, string path, string type, string token, string upload, string accountid)
        {
            string url =
                sd.ToDomainUrl(string.Concat(HttpUtility.UrlDecode(path), "?type=", type, "&token=", token,
                                                "&upload=", upload, "&accountid=", accountid));
            return Redirect(url);
        }
    }
}
