using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace tradelr.Areas.dashboard.Models.store.blog
{
    public class ArticleViewModel
    {
        public Article article { get; set; }
        public IEnumerable<SelectListItem> blogList { get; set; }
        public bool editMode { get; set; }
    }
}