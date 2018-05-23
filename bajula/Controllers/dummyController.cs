using System.Web.Mvc;

namespace tradelr.Controllers
{
    public class dummyController : Controller
    {
        public ActionResult Index()
        {
            return new EmptyResult();
        }

    }
}
