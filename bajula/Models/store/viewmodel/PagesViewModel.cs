using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Areas.dashboard.Models.store;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.Areas.dashboard.Models.store.page;

namespace tradelr.Models.store.viewmodel
{
    public class PagesViewModel
    {
        public IEnumerable<Page> pages { get; set; }
        public IEnumerable<Blog> blogs { get; set; }
    }
}