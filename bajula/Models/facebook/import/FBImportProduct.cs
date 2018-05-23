using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Library.Constants;

namespace tradelr.Models.facebook.import
{
    public class FBImportProduct
    {
        public long id { get; set; } // id = albumid for product import, id = photoid for collection import
        public string title { get; set; }
        public string description { get; set; }
        public string sellingprice { get; set; }
        public string sku { get; set; }
        public string[] photoids { get; set; }
    }

    public static class FBImportProductHelper
    {
        public static string ToFacebookPhotoUrl(this string id, string access_token)
        {
            return string.Format("{0}{1}/picture?type=normal&access_token={2}", GeneralConstants.FACEBOOK_GRAPH_HOST, id, access_token);
        }
    }
}