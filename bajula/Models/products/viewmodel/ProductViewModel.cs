using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Libraries;
using tradelr.Libraries.Helpers;

namespace tradelr.Models.products.viewmodel
{
    public class ProductViewModel : BaseViewModel
    {
        public bool editMode { get; set; }

        public Product product { get; set; }

        public bool isPostToBlogger { get; set; }
        public bool isPostToEbay { get; set; }
        public bool isPostToEtsy { get; set; }
        public bool isPostToFacebook { get; set; }
        public bool isPostToGoogle { get; set; }
        public bool isPostToTrademe { get; set; }
        public bool isPostToTumblr { get; set; }
        public bool isPostToWordpress { get; set; }
        public bool isMetric { get; set; }

        // ebay
        public bool isEbaySynced { get; set; }
        public bool hasPaypalAccount { get; set; }

        // facebook
        public List<CheckBoxListInfo> facebookStreams { get; set; }
        public List<CheckBoxListInfo> facebookAlbums { get; set; }

        // shipping
        public IEnumerable<SelectListItem> shippingProfiles { get; set; }
        public bool showShipwire { get; set; }

        // trademe
        public bool isTrademeSynced { get; set; }

        // inventory tracking
        public IEnumerable<SelectListItem> trackInventoryList { get; set; }
        

        public string distanceUnit { get; set; }
        public string weightUnit { get; set; }

        public SelectList mainCategoryList { get; set; }
        public SelectList subCategoryList { get; set; }
        public SelectList stockUnitList { get; set; }
        
        public IEnumerable<SelectListItem> collections { get; set; } 

        public ProductViewModel(BaseViewModel viewmodel): base(viewmodel)
        {
            var emptyList = new List<SelectListItem>();
            subCategoryList = emptyList.AsQueryable().ToSelectList("", "None", "");
            facebookStreams = new List<CheckBoxListInfo>();
            facebookAlbums = new List<CheckBoxListInfo>();
            collections = Enumerable.Empty<SelectListItem>();
        }
    }
}