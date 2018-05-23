using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.facebook.viewmodel
{
    public class FBImportPhotoViewModel
    {
        public string id { get; set; }
        public List<string> ids { get; set; } // used when grouping multiple photos with the same sku

        public List<string> photo_links { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string sku { get; set; }
        public string details { get; set; }

        public FBImportPhotoViewModel()
        {
            photo_links = new List<string>();
            ids = new List<string>();
        }
    }
}