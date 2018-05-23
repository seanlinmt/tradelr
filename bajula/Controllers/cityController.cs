using System.Linq;
using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Controllers
{
    //[ElmahHandleError]
    public class cityController : baseController
    {
        public ActionResult Find(string q)
        {
            var result = repository.FindMASTERCity(q);
            var data = result.Select(x => new
                                               {
                                                   id = x.id,
                                                   title = x.name
                                               });
            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

    }
}
