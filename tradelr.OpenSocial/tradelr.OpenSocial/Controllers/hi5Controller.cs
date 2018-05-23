using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Facebook.ActionFilters;
using tradelr.Logging;
using tradelr.OpenSocial.Models;

namespace tradelr.OpenSocial.Controllers
{
    [ElmahHandleError]
    public class hi5Controller : Controller
    {
        private readonly tradelrDataContext db;

        public hi5Controller()
        {
            db = new tradelrDataContext();
        }

        /// <summary>
        /// saves mapping
        /// </summary>
        /// <returns></returns>
        public ActionResult Configure(string address, string ownerid, string viewerid, bool? isCanvas)
        {
            if (string.IsNullOrEmpty(address))
            {
                Syslog.Write(ErrorLevel.ERROR, "os: address is empty");
                return new EmptyResult();
            }

            // parse address
            Uri storeAddress = null;
            try
            {
                storeAddress = new Uri(address);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return new EmptyResult();
            }

            string subdomain = "";
            if (storeAddress.Host.Split('.').Length > 2)
            {
                int lastIndex = storeAddress.Host.LastIndexOf(".");
                int index = storeAddress.Host.LastIndexOf(".", lastIndex - 1);
                subdomain = storeAddress.Host.Substring(0, index);
            }
            else
            {
                Syslog.Write(ErrorLevel.ERROR, "os: no subdomain");
                return new EmptyResult();
            }

            var mastersd = db.MASTERsubdomains.Where(x => x.name == subdomain).SingleOrDefault();

            if (mastersd == null)
            {
                Syslog.Write(ErrorLevel.ERROR, "os: subdomain does not exist");
                return new EmptyResult();
            }

            var identifier = string.Concat("hi5.com.", ownerid);
            // check if there's already an entry, we ignore if there's already an entry
            var existing =
                db.opensocialPages.Where(x => x.osid == identifier && x.subdomainid == mastersd.id).SingleOrDefault();

            if (existing == null)
            {
                var newEntry = new opensocialPage() { subdomainid = mastersd.id, osid = identifier };
                db.opensocialPages.InsertOnSubmit(newEntry);
                db.SubmitChanges();
            }
            bool canvas = false;
            if (isCanvas.HasValue && isCanvas.Value)
            {
                canvas = true;
            }
            return RedirectToAction("Index", new {ownerid, viewerid, canvas});
        }

        /// <summary>
        /// returns store content if there is 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string ownerid, string viewerid, bool? isCanvas)
        {
            if (string.IsNullOrEmpty(ownerid))
            {
                Syslog.Write(ErrorLevel.ERROR, "OwnerID empty");
                return Content("<span class='error'>owner ID not specified.</span>");
            }
            var identifier = string.Concat("hi5.com.", ownerid);
            var ospage = db.opensocialPages.Where(x => x.osid == identifier).SingleOrDefault();
            var viewdata = new OpenSocialViewData { isCanvas = isCanvas.GetValueOrDefault()};
            if (ospage != null)
            {
                var subdomain = ospage.MASTERsubdomain.name;
                // render store
                viewdata.storeName = ospage.MASTERsubdomain.organisation.name;
                
                if (isCanvas.GetValueOrDefault())
                {
                    viewdata.gallery = new Gallery
                    {
                        viewAllUrl = string.Concat("http://", subdomain, ".tradelr.com"),
                        products = db.products.Where(
                                        x => x.user.organisation1.subdomain == ospage.subdomainid &&
                                             x.sellingPrice.HasValue)
                                 .OrderByDescending(x => x.id)
                                 .Take(20)
                                 .ToModel(subdomain)
                    };
                    return View("GalleryCanvas", viewdata);
                }

                viewdata.gallery = new Gallery
                {
                    viewAllUrl = string.Concat("http://", subdomain, ".tradelr.com"),
                    products = db.products.Where(
                                    x => x.user.organisation1.subdomain == ospage.subdomainid &&
                                         x.sellingPrice.HasValue)
                             .OrderBy(x => db.Random())
                             .Take(16)
                             .ToModel(subdomain)
                };
                return View("GalleryProfile", viewdata);
            }
            viewdata.storeName = "tradelr for hi5";
            if (viewerid == ownerid)
            {
                return View("Profile", viewdata);
            }
            return View("NotConfigured", viewdata);
        }

        public ActionResult Install(string fb_sig_user)
        {
            throw new NotImplementedException();
            return new EmptyResult();
        }

        public ActionResult Uninstall(string opensocial_owner_id, string opensocial_viewer_id)
        {
            Syslog.Write(ErrorLevel.INFORMATION, string.Concat("hi5 delete:",opensocial_owner_id,",",opensocial_viewer_id));
            if (!string.IsNullOrEmpty(opensocial_viewer_id) && opensocial_viewer_id == opensocial_owner_id)
            {
                opensocial_owner_id = string.Concat("hi5.com.", opensocial_owner_id);
                var existing = db.opensocialPages.Where(x => x.osid == opensocial_owner_id).SingleOrDefault();
                if (existing != null)
                {
                    db.opensocialPages.DeleteOnSubmit(existing);
                    db.SubmitChanges();
                }
            }
            return new EmptyResult();
        }
    }
}
