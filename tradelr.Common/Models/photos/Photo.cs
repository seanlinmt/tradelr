using System.Collections.Generic;
using System.Linq;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.Library.Constants;

namespace tradelr.Common.Models.photos
{
    public class Photo
    {
        public long id { get; set; }
        public string bigUrl { get; set; }
        public string url { get; set; }
        public string externalid { get; set; } // for like youtube id
    }

    public static class PhotoHelper
    {
        public static Photo ToModel(this product_image img, Imgsize size, string externalid = null)
        {
            return new Photo()
            {
                id = img.id,
                bigUrl = img.url,
                url = Img.by_size(img.url, size),
                externalid = externalid
            };
        }

        public static IEnumerable<Photo> ToModel(this IQueryable<product_image> images, Imgsize size)
        {
            foreach (var image in images)
            {
                yield return image.ToModel(size);
            }
        }

        public static List<Photo> ToModel(this IEnumerable<product_image> images, Imgsize thumbnailSize, Imgsize? bigSize = null)
        {
            var data = new List<Photo>();
            foreach (var image in images)
            {
                var photo = new Photo();
                photo.id = image.id;
                photo.url = Img.by_size(image.url, thumbnailSize);
                if (bigSize.HasValue)
                {
                    photo.bigUrl = Img.by_size(image.url, bigSize.Value);
                }
                data.Add(photo);
            }
            return data;
        }

        public static List<Photo> ToModel(this IEnumerable<image> images, Imgsize thumbnailSize, Imgsize? bigSize)
        {
            var data = new List<Photo>();
            foreach (var image in images)
            {
                var photo = new Photo();
                photo.id = image.id;
                photo.url = Img.by_size(image.url, thumbnailSize);
                if (bigSize.HasValue)
                {
                    photo.bigUrl = Img.by_size(image.url, bigSize.Value);
                }
                if (image.videos != null)
                {
                    photo.externalid = image.videos.youtubeid;
                }
                data.Add(photo);
            }
            return data;
        }

        public static Photo ToModel(this image img, Imgsize size, string externalid = null)
        {
            return new Photo()
                       {
                           id = img.id,
                           bigUrl = img.url,
                           url = Img.by_size(img.url, size),
                           externalid = externalid
                       };
        }

        public static IEnumerable<Photo> ToModel(this IQueryable<image> images, Imgsize size)
        {
            foreach (var image in images)
            {
                yield return image.ToModel(size);
            }
        }

        public static string ToPhysicalPath(this image img)
        {
            return string.Concat(GeneralConstants.APP_ROOT_DIR, img.url);
        }
    }
}
