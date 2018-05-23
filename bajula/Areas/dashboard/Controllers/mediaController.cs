using System;
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
    public class mediaController : baseController
    {
        public ActionResult Add()
        {
            throw new NotImplementedException();
        }

        public ActionResult AddFromClient()
        {
            var viewModel = new MediaItemAddViewModel();
            try
            {
                UpdateModel(viewModel);

                if (Request.Files.Count < 1 || Request.Files[0].ContentLength == 0)
                    return Content(string.Format("<script type=\"text/javascript\">var result = {{ error: \"{0}\" }};</script>", string.Format("You didn't give me a file to upload")));

                IMediaService _mediaService = new MediaService(MASTERdomain.uniqueid);
                try
                {
                    _mediaService.GetMediaFiles(viewModel.MediaPath);
                }
                catch //media api needs a little work, like everything else of course ;) <- ;) == my stuff included. to clarify I need a way to know if the path exists or have UploadMediaFile create paths as necessary but there isn't the time to hook that up in the near future
                {
                    _mediaService.CreateFolder("", viewModel.MediaPath);
                }

                var file = Request.Files[0];
                var publicUrl = _mediaService.UploadMediaFile(viewModel.MediaPath, file, viewModel.ExtractZip);

                return Content(string.Format("<script type=\"text/javascript\">var result = {{ url: \"{0}\" }};</script>", publicUrl));
            }
            catch (Exception exception)
            {
                return Content(string.Format("<script type=\"text/javascript\">var result = {{ error: \"{0}\" }};</script>", string.Format("ERROR: Uploading media file failed: {0}", exception.Message)));
            }
        }

        public ActionResult Index()
        {
            throw new NotImplementedException();
        }

        public ActionResult Create()
        {
            throw new NotImplementedException();
        }

        public ActionResult Edit()
        {
            throw new NotImplementedException();
        }

        public ActionResult EditMedia()
        {
            throw new NotImplementedException();
        }

        public ActionResult EditMediaDeletePOST()
        {
            throw new NotImplementedException();
        }


        public ActionResult EditProperties()
        {
            throw new NotImplementedException();
        }

        public ActionResult EditPropertiesDeletePOST()
        {
            throw new NotImplementedException();
        }

    }
}
