using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.facebook.import
{
    public class FBImportSingle
    {
        public string access_token { get; set; }
        public FBImportProduct product { get; set; }
    }
}