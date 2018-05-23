using System.Collections.Generic;
using System.Linq;
using clearpixels.Logging;
using tradelr.Models.photos;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public long AddImage(image img)
        {
            db.images.InsertOnSubmit(img);
            db.SubmitChanges();
            return img.id;
        }

        public void AddProductImage(product_image img)
        {
            db.product_images.InsertOnSubmit(img);
            db.SubmitChanges();
        }

        public void DeleteImage(string filename)
        {
            var pimage = db.product_images.SingleOrDefault(x => x.url == filename);
            if (pimage != null)
            {
                foreach (var row in pimage.products)
                {
                    row.thumb = null;
                }
                db.product_images.DeleteOnSubmit(pimage);
                
            }
            else
            {
                var image = db.images.SingleOrDefault(x => x.url == filename);
                if (image != null)
                {
                    foreach (var org in image.organisations)
                    {
                        org.logo = null;
                    }
                    foreach (var usr in image.users)
                    {
                        usr.profilePhoto = null;
                    }
                    db.images.DeleteOnSubmit(image);
                }
                else
                {
                    Syslog.Write("Failed to delete image " + filename);
                }
            }
            Save("DeleteImage");
        }

        public string[] DeleteImage(long imageid, long subdomainid, PhotoType imageType)
        {
            var data = db.images.Where(x => x.id == imageid && x.subdomain == subdomainid);
            if (imageType != PhotoType.ALL)
            {
                data.Where(x => x.imageType == imageType.ToString());
            }

            var imagepaths = data.Select(x => x.url).ToArray();

            db.images.DeleteAllOnSubmit(data);
            db.SubmitChanges();

            return imagepaths;
        }

        public image GetImage(long id)
        {
            return db.images.SingleOrDefault(x => x.id == id);
        }
        
        public IQueryable<image> GetImages(PhotoType type, long contextID)
        {
            return db.images.Where(x => x.imageType == type.ToString() && x.contextID == contextID);
        }
        
        public IQueryable<image> GetImagesAll()
        {
            return db.images;
        }

        public product_image GetProductImage(long imageid)
        {
            return db.product_images.SingleOrDefault(x => x.id == imageid);
        }

        public void UpdateProductImages(long subdomainid, long productid, IEnumerable<string> imageIDs)
        {
            var result =
                db.product_images.Where(x => x.subdomainid == subdomainid && imageIDs.Contains(x.id.ToString()));
            foreach (var row in result)
            {
                row.productid = productid;
            }
            db.SubmitChanges();
        }
    }
}