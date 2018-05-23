using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using tradelr.Common.Library.Imaging;
#if RACKSPACE
using tradelr.FS.Rackspace;
#endif
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Libraries.ActionFilters;
#if AZURE
using tradelr.Libraries.CloudStorage.Azure;
#endif
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.Constants;
using tradelr.Library.files;
using clearpixels.Logging;
using tradelr.Models.photos;
using tradelr.Models.users;

namespace tradelr.Controllers
{
    //[ElmahHandleError]
    
    public class photosController : baseController
    {
        [HttpPost]
        [RoleFilter(role = UserRole.CREATOR)]
        public void Delete(PhotoType type, string ids)
        {
            if (sessionid == null || String.IsNullOrEmpty(ids))
            {
                return;
            }
            var imageids = ids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // check that image is currently logged in user
            // so far only being called to handle single deletes
            var imagepaths = new List<string>();

            foreach (var id in imageids)
            {
                long imageid = long.Parse(id);
                switch (type)
                {
                    case PhotoType.PRODUCT:
                        var img = db.product_images.SingleOrDefault(x => x.id == imageid && x.subdomainid == subdomainid);
                        if (img != null)
                        {
                            // if main thumbnail of photo then set thumbnail id to null
                            var p = repository.GetProducts(subdomainid.Value).SingleOrDefault(x => x.thumb.HasValue && x.thumb.Value == imageid);
                            if (p != null)
                            {
                                p.thumb = null;
                            }
                            imagepaths.Add(img.url);
                            db.product_images.DeleteOnSubmit(img);
                        }
                        break;
                    case PhotoType.PROFILE:
                    case PhotoType.COMPANY:
                    case PhotoType.BACKGROUND:
                    case PhotoType.ALL:
                        var paths = repository.DeleteImage(imageid, subdomainid.Value, type);
                        imagepaths.AddRange(paths);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("type");
                }
            }

            repository.Save();

            // delete images
            foreach (var entry in imagepaths)
            {
                var filepath = GeneralConstants.APP_ROOT_DIR + entry;

                if (System.IO.File.Exists(filepath))
                {
                    System.IO.File.Delete(filepath);
                }
                else
                {
                    Syslog.Write(string.Format("Delete fail. File {0} does not exist", filepath));
                }
            }
        }

        public ActionResult Upload(PhotoType type, long? id)
        {
            if (!sessionid.HasValue)
            {
                return SendJsonSessionExpired();
            }
            long ownerid = sessionid.Value;

            if (Request.Files.Count < 1)
            {
                throw new Exception();
            }

            var imageUpload = Request.Files[0];
            var extIndex = imageUpload.FileName.LastIndexOf('.');
            var ext = imageUpload.FileName.Substring(extIndex);
            string filename = ImgHelper.BuildFilename(sessionid.Value, ext);

            var handler = new FileHandler(filename, UploadFileType.IMAGE, MASTERdomain.uniqueid);

            var url = handler.Save(imageUpload.InputStream);

            if (string.IsNullOrEmpty(url))
            {
                return Content("," + GeneralConstants.PHOTO_UPLOAD_ERROR_PATH);
            }

            image image = null;
            product_image pimage = null;
            long imageid;
            switch (type)
            {
                case PhotoType.BACKGROUND:
                case PhotoType.PROFILE:
                case PhotoType.COMPANY:
                    image = new image
                                {
                                    imageType = type.ToString(), 
                                    url = url, 
                                    subdomain = subdomainid.Value
                                };
                    imageid = repository.AddImage(image);
                    break;
                case PhotoType.PRODUCT:
                    pimage = new product_image
                                 {
                                     url = url, 
                                     subdomainid = subdomainid.Value
                                 };
                    repository.AddProductImage(pimage);
                    imageid = pimage.id;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // depending on image type....
            user usr;
            string retVal = "";
            long profileID;
            string thumbnailUrl;
            switch (type)
            {
                case PhotoType.BACKGROUND:
                    thumbnailUrl = Img.by_size(url, Imgsize.COMPACT);
                    retVal = string.Concat(imageid, ",#background_image,", thumbnailUrl);
                    break;
                case PhotoType.COMPANY:
                    thumbnailUrl = Img.by_size(url, Imgsize.MEDIUM);
                    profileID = id.HasValue ? id.Value : ownerid;
                    usr = repository.GetUserById(profileID, subdomainid.Value);
                    usr.organisation1.logo = imageid;
                    image.contextID = usr.organisation.Value;
                    repository.Save();
                    retVal = string.Concat(imageid, ",#company_image,", thumbnailUrl);
                    CacheHelper.Instance.invalidate_dependency(DependencyType.organisation, subdomainid.Value.ToString());
                    break;
                case PhotoType.PROFILE:
                    thumbnailUrl = Img.by_size(url, Imgsize.MEDIUM);
                    profileID = id.HasValue ? id.Value : ownerid;
                    usr = repository.GetUserById(profileID, subdomainid.Value);
                    usr.profilePhoto = imageid;
                    image.contextID = usr.id;
                    repository.Save();
                    retVal = string.Concat(imageid, ",#profile_image,", thumbnailUrl);
                    break;
                case PhotoType.PRODUCT:
                    thumbnailUrl = Img.by_size(url, Imgsize.MEDIUM);
                    retVal = string.Concat(imageid, ",#product_images,", thumbnailUrl);
                    // for when editing products
                    // when creating new product entry, contextid is only updated when product is saved, it  will be 0 if images
                    // uploaded and then product is not saved
                    if (id.HasValue)
                    {
                        var productid = id.Value;
                        pimage.productid = productid;
                        repository.Save();
                        repository.UpdateProductMainThumbnail(productid, subdomainid.Value, imageid.ToString());
                    }
                    break;
            }

            return Content(retVal);
        }

    }
}
