using System;
using System.Web;
using System.Web.Mvc;
using tradelr.Areas.dashboard.Models.media;
using tradelr.Areas.dashboard.Models.orchard.media.services;
using tradelr.Areas.dashboard.Models.orchard.media.viewmodels;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class mediapickerController : baseController
    {
        public ActionResult Index(string name, string mediaPath)
        {
            var mediaService = new MediaService(MASTERdomain.uniqueid);
            var mediaFolders = mediaService.GetMediaFolders(mediaPath);
            var mediaFiles = string.IsNullOrEmpty(mediaPath) ? null : mediaService.GetMediaFiles(mediaPath);
            var model = new MediaFolderEditViewModel { FolderName = name, MediaFiles = mediaFiles, MediaFolders = mediaFolders, MediaPath = mediaPath };
            ViewData["Service"] = mediaService;
            return View(model);
        }

        public ActionResult CreateFolder(string path, string folderName)
        {
            try
            {
                var mediaService = new MediaService(MASTERdomain.uniqueid);
                mediaService.CreateFolder(HttpUtility.UrlDecode(path), folderName);
                return Json(true);
            }
            catch (Exception exception)
            {
                return Json(string.Format("Creating Folder failed: {0}", exception.Message));
            }
        }

    }
}
