using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Models.photos;

namespace tradelr.Models.facebook.viewmodel
{
    public class FBImportAlbumViewModel : FBImportPhotoViewModel
    {
        public string token { get; set; }
        public int photo_count { get; set; }
    }
}