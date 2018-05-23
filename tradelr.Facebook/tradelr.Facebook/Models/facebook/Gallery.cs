using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Facebook.Models.facebook
{
    public class Gallery
    {
        public IEnumerable<SelectListItem> categories { get; set; }
        public IEnumerable<Product> products { get; set; }
        public string viewAllUrl { get; set; }
    }
}