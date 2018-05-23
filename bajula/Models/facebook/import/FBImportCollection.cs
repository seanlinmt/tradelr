using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.facebook.import
{
    public class FBImportCollection
    {
        public string id { get; set; } // albumid
        public string title { get; set; }
        public string access_token { get; set; }
        public IEnumerable<FBImportProduct> products { get; set; }
    }
}