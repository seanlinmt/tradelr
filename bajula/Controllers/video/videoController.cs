using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;
using tradelr.DBML.Helper;
using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using tradelr.Models.google;
using tradelr.Models.photos;
using tradelr.Models.users;

namespace tradelr.Controllers.video
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    public class videoController : baseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(string url, long? productid)
        {
            var youtube = new YouTube(url);

            // try to obtain thumbnail image
            var imageurl = youtube.GetExternalThumbnailUrl();

            if (string.IsNullOrEmpty(imageurl))
            {
                return Json("Invalid address".ToJsonFail());
            }

            // return thumbnail image, image id, video link
            var img = imageurl.ReadAndSaveProductImageFromUrl(subdomainid.Value, sessionid.Value,productid);

            // save video
            repository.AddVideo(youtube.id, img.id, subdomainid.Value);

            repository.Save();

            return Json(img.ToModel(Imgsize.MEDIUM, youtube.id).ToJsonOKData());
        }

    }
}
