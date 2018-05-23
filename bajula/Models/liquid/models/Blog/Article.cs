using System;
using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.Areas.dashboard.Models.store.blog;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library;
using tradelr.Models.users;

namespace tradelr.Models.liquid.models.Blog
{
    public class Article : Drop
    {
        public long id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public DateTime? published_at { get; set; }
        public string url { get; set; }
        public List<Comment> comments { get; set; }
        public int comments_count { get { return comments.Count; }  }
        public string comment_post_url { get; set; }
        public bool comments_enabled { get; set; }
        public bool moderated { get; set; }
        public List<string> tags { get; set; }
    }

    public static class ArticleHelper
    {
        public static Article ToLiquidModel(this article row)
        {
            var a = new Article();

            a.id = row.id;
            a.title = row.title;
            a.author = row.user.ToFullName();
            a.content = row.content;
            a.created_at = row.created;
            a.published_at = row.published;
            a.url = row.ToLiquidUrl();
            a.comments = row.article_comments.Where(x => x.isApproved && !x.isSpam).ToLiquidModel().ToList();
            a.comment_post_url = string.Format("/blogs/{0}/{1}-{2}/comments", row.blog.permalink, row.id,
                                             row.title.ToLower().ToSafeUrl());

            var commentType = ((Commenting)row.blog.comments);

            a.comments_enabled = commentType != Commenting.OFF;
            a.moderated = commentType == Commenting.MODERATED;
            a.tags = row.article_tags.Select(x => x.name).ToList();

            return a;
        }

        public static IEnumerable<Article> ToLiquidModel(this IEnumerable<article> rows)
        {
            foreach (var row in rows)
            {
                yield return row.ToLiquidModel();
            }
        }

        public static string ToLiquidUrl(this article row)
        {
            return string.Format("/blogs/{0}/{1}-{2}", row.blog.permalink, row.id, row.title.ToLower().ToSafeUrl());
        }
    }
}