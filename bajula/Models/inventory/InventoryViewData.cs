using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Libraries;
using tradelr.Models.users;

namespace tradelr.Models.inventory
{
    public class InventoryViewData : BaseViewModel
    {
        public IEnumerable<FilterBoxListInfo> locationList { get; set; }
        public IEnumerable<FilterBoxListInfo> categoryList { get; set; }
        public IEnumerable<FilterBoxListInfo> collectionsList { get; set; }
        public IEnumerable<FilterBoxListInfo> supplierList { get; set; }
        public IEnumerable<FilterBoxListInfo> supplierCategoryList { get; set; }
        
        public InventoryViewData(BaseViewModel baseviewdata)
            : base(baseviewdata)
        {

        }
    }
}