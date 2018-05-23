using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using clearpixels.Logging;
using tradelr.Library.files;
using tradelr.Models.photos;

namespace tradelr.DBML.Helper
{
    public static class ImgHelper
    {
        public static string BuildFilename(long ownerid, string extension)
        {
            return String.Concat(ownerid, "_", DateTime.UtcNow.Ticks.ToString("X"), extension);
        }

        public static product_image ReadAndSaveProductImageFromUrl(this string url, long subdomainid, long ownerid, long? productid)
        {
            var req = WebRequest.Create(url);
            WebResponse resp = null;
            try
            {
                resp = req.GetResponse();
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }

            if (resp == null)
            {
                return null;
            }
            try
            {
                var image = new product_image();
                using (var repository = new TradelrRepository())
                {
                    var sd = repository.GetSubDomain(subdomainid);
                    var extension = url.ToImageFormat().ToStringExtension();
                    var filename = BuildFilename(ownerid, extension);

                    var handler = new FileHandler(filename, UploadFileType.IMAGE, sd.uniqueid);
                    
                    image.productid = productid;
                    image.subdomainid = subdomainid;
                    image.url = handler.Save(resp.GetResponseStream());

                    if (productid.HasValue)
                    {
                        repository.AddProductImage(image);
                        var product = repository.GetProduct(productid.Value, subdomainid);
                        if (product != null && !product.thumb.HasValue)
                        {
                            product.thumb = image.id;
                        }
                        repository.Save("ReadAndSaveProductImageFromUrl");

                    }
                }
                return image;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return null;
            }
        }

        public static image ReadAndSaveFromUrl(this string url, long subdomainid, long ownerid, long contextid, PhotoType type)
        {
            var req = WebRequest.Create(url);
            WebResponse resp = null;
            try
            {
                resp = req.GetResponse();
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }

            if (resp == null)
            {
                return null;
            }
            try
            {
                var image = new image();
                using (var repository = new TradelrRepository())
                {
                    var sd = repository.GetSubDomain(subdomainid);
                    var extension = url.ToImageFormat().ToStringExtension();
                    var filename = BuildFilename(ownerid, extension);
                    var handler = new FileHandler(filename, UploadFileType.IMAGE, sd.uniqueid);

                    image.imageType = type.ToString();
                    image.subdomain = subdomainid;
                    image.contextID = contextid;
                    image.url = handler.Save(resp.GetResponseStream());

                    repository.AddImage(image);
                    switch (type)
                    {
                        case PhotoType.PROFILE:
                            var usr = repository.GetUserById(ownerid);
                            if (usr != null)
                            {
                                usr.profilePhoto = image.id;
                            }
                            repository.Save("ReadAndSaveFromUrl:Profile");
                            break;
                        default:
                            break;
                    }
                }
                return image;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return null;
            }
        }

        public static ImageFormat ToImageFormat(this string filename)
        {
            var extIndex = filename.LastIndexOf('.');
            var ext = filename.Substring(extIndex);
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".JPG":
                case "JPEG":
                    return ImageFormat.Jpeg;
                case ".png":
                case ".PNG":
                    return ImageFormat.Png;
                case ".gif":
                case ".GIF":
                    return ImageFormat.Gif;
                default:
                    // 14/11: commented out because gbase will flood log
                    //Syslog.Write(string.Concat("Unrecognised image extension: ", filename));
                    return ImageFormat.Jpeg;
            }
        }

        public static string ToSavedImageUrl(this string datauri, long sessionid, long subdomainid)
        {
            var segments = datauri.Split(new[] { ',' });
            if (segments.Length != 2)
            {
                Syslog.Write("Invalid datauri: " + datauri);
                return "";
            }
            var type = segments[0];
            var data = segments[1];

            Regex datauriRegex = new Regex("data:image/(.+);base64");
            var match = datauriRegex.Match(type);
            string extension = "";
            switch (match.Groups[1].Value)
            {
                case "jpeg":
                    extension = ".jpg";
                    break;
                case "png":
                    extension = ".png";
                    break;
                case "gif":
                    extension = ".gif";
                    break;
                default:
                    break;
            }

            if (String.IsNullOrEmpty(extension))
            {
                Syslog.Write("Invalid datauri: " + datauri);
                return "";
            }

            var bytes = Convert.FromBase64String(data);
            var ms = new MemoryStream(bytes);
            var filename = BuildFilename(sessionid, extension);

            string url = "";
            using (var repository = new TradelrRepository())
            {
                var sd = repository.GetSubDomain(subdomainid);

                var handler = new FileHandler(filename, UploadFileType.IMAGE, sd.uniqueid);

                url = handler.Save(ms);
            }

            return url;
        }

        private static string ToStringExtension(this ImageFormat format)
        {
            if (format == ImageFormat.Gif)
            {
                return ".gif";
            }
            if (format == ImageFormat.Png)
            {
                return ".png";
            }
            return ".jpg";
        }
    }
}
