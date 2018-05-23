using System.Linq;
using System.Web.Mvc;
using tradelr.Libraries.ActionFilters;

namespace tradelr.Controllers
{
    //[ElmahHandleError]
    public class tagsController : baseController
    {
        public ActionResult Find(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return Content("");
            }

            var data = db.tags.Where(x => x.name.StartsWith(q)).Select(x => x.name.Replace("_", " ")).ToArray();
            return Content(string.Join("\n", data));
        }


        public ActionResult ArticleTags(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return Content("");
            }

            var data = db.article_tags.Where(x => x.name.StartsWith(q)).Select(x => x.name.Replace("_", " ")).ToArray();
            return Content(string.Join("\n", data));
        }

    }
}
