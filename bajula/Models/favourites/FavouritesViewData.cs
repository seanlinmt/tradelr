using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Libraries;

namespace tradelr.Models.favourites
{
    public class FavouritesViewData : BaseViewModel
    {
        public FavouritesViewData(BaseViewModel viewmodel) : base(viewmodel)
        {
        }

        public IEnumerable<FilterBoxListInfo> categoryList { get; set; }
    }
}