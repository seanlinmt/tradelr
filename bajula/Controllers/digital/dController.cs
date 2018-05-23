using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.subdomain;

namespace tradelr.Controllers.digital
{
    public class dController : Controller
    {
        // actual file download
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect(GeneralConstants.HTTP_HOST);
            }

            string filename;
            string path;
            string contentType;

            using (var db = new tradelrDataContext())
            {
                var digitalOrder = db.orderItems_digitals.SingleOrDefault(x => x.downloadid == id);
                if (digitalOrder == null)
                {
                    return Redirect(GeneralConstants.HTTP_HOST);
                }

                // check download limit
                var spec = digitalOrder.orderItem.product_variant.product.products_digitals;

                if (spec.limit.HasValue && spec.limit.Value <= digitalOrder.downloadCount)
                {
                    return Redirect(digitalOrder.orderItem.product_variant.product.MASTERsubdomain.ToHostName().ToDomainUrl());
                }

                // check expiry
                if (spec.expiryDate.HasValue && DateTime.UtcNow > spec.expiryDate.Value)
                {
                    return Redirect(digitalOrder.orderItem.product_variant.product.MASTERsubdomain.ToHostName().ToDomainUrl());
                }
                
                filename = spec.filename;
                path = GeneralConstants.APP_ROOT_DIR + spec.filepath;

                if (!System.IO.File.Exists(path))
                {
                    return Redirect(digitalOrder.orderItem.product_variant.product.MASTERsubdomain.ToHostName().ToDomainUrl());
                }

                // no need to store cookie as user may use different device, makes it easier to define the term download limit
                digitalOrder.downloadCount++;

                db.SubmitChanges();
            }

            var buffer = new byte[256];
            var fs = new FileStream(path, FileMode.Open);

            // try get mimetype
            if (fs.Length >= 256)
                fs.Read(buffer, 0, 256);
            else
                fs.Read(buffer, 0, (int)fs.Length);

            // reset to beginning of stream
            fs.Seek(0, SeekOrigin.Begin);
            
            try
            {
                UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                var mimeTypePtr = new IntPtr(mimetype);
                contentType = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                contentType = "unknown/unknown";
            }

            // return file
            return File(fs, contentType, filename);
        }

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static UInt32 FindMimeFromData(
            UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] String pwzMimeProposed,
            UInt32 dwMimeFlags,
            out UInt32 ppwzMimeOut,
            UInt32 dwReserverd
        );
    }
}
