using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.DBML;

namespace tradelr.Models.liquid.models.Blog
{
    public class Blog : Drop
    {
        public Blog(blog blog)
        {
            id = blog.id;
            title = blog.title;
            handle = blog.permalink;
            url = blog.ToLiquidUrl();
            articles = blog.articles.Where(x => x.published.HasValue).ToLiquidModel().ToList();
            comments_enabled = (Commenting)blog.comments != Commenting.OFF;
            moderated = (Commenting)blog.comments == Commenting.MODERATED;
        }

        public long id { get; set; }
        public string title { get; set; }
        public string handle { get; set; }
        public string url { get; set; }
        public List<Article> articles { get; set; }
        public int articles_count { get { return articles.Count; } }
        public bool comments_enabled { get; set; }
        public bool moderated { get; set; }
    }

    public static class BlogHelper
    {
        public static string ToLiquidUrl(this blog row)
        {
            return "/blogs/" + row.permalink.ToLower();
        }
    }
}