using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Libraries.Helpers;
using tradelr.Library;
using tradelr.Models.liquid.models.Blog;

namespace tradelr.Areas.dashboard.Models.store.blog
{
    public class Blog
    {
        public string id { get; set; }
        public string title { get; set; }
        public string fullUrl { get; set; }
        public Commenting commentType { get; set; }
        public IEnumerable<Article> articles { get; set; }
        public IEnumerable<SelectListItem> commentsList { get; set; }
    }

    public static class BlogHelper
    {
        public static Blog ToModel(this DBML.blog row, bool details = false)
        {
            var b = new Blog()
                       {
                           id = row.id.ToString(),
                           title = row.title,
                           articles = row.articles.ToModel(),
                           commentsList = Enum.GetValues(typeof (Commenting)).Cast<Commenting>()
                                                .Select(x => new SelectListItem()
                                                                 {
                                                                     Text = x.ToDescriptionString(), 
                                                                     Value = x.ToInt().ToString(),
                                                                     Selected = x.ToInt() == row.comments
                                                                 })
                       };
            if (details)
            {
                b.fullUrl = row.ToLiquidUrl();
            }

            return b;
        }
    }
}