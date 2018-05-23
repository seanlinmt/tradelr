using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.subdomain;

namespace tradelr.Controllers.digital
{
    public class cController : Controller
    {
        // creates a cart item when a pre buyer clicks on buy now link
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Redirect(GeneralConstants.HTTP_HOST);
            }

            long variantid;
            string hostname;

            using (var db = new tradelrDataContext())
            {
                var digital = db.products_digitals.SingleOrDefault(x => x.linkid == id);
                if (digital == null)
                {
                    return Redirect(GeneralConstants.HTTP_HOST);
                }

                hostname = digital.product.MASTERsubdomain.ToHostName();
                // check expiry
                if (digital.expiryDate.HasValue && DateTime.UtcNow > digital.expiryDate.Value)
                {
                    return Redirect(hostname.ToDomainUrl());
                }

                variantid = digital.product.product_variants.First().id;
            }

            return Redirect(hostname.ToDomainUrl("/cart/add/" + variantid));
        }

    }
}
