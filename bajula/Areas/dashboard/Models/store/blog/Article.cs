using System.Collections.Generic;
using System.Linq;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library.Constants;
using tradelr.Models.liquid.models.Blog;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Models.store.blog
{
    public class Article
    {
        public string id { get; set; }
        public string title { get; set; }
        public string creator_name { get; set; }
        public string tags { get; set; }
        public string created { get; set; }
        public int comment_count { get; set; }
        public string content { get; set; }
        public bool isPublish { get; set; }
        public IEnumerable<ArticleComment> comments { get; set; }
        public string fullUrl { get; set; }
    }

    public static class ArticleHelper
    {
        public static Article ToModel(this article row, bool details = false)
        {
            var a = new Article()
            {
                id = row.id.ToString(),
                title = row.title,
                creator_name = row.user.ToFullName(),
                tags = string.Join(",", row.article_tags.Select(x => x.name)),
                created = row.created.ToString(GeneralConstants.DATEFORMAT_STANDARD),
                isPublish = row.published.HasValue,
                comment_count = row.article_comments.Count()
            };

            if(details)
            {
                a.content = row.content;
                a.comments = row.article_comments.ToModel();
                a.fullUrl = row.ToLiquidUrl();
            }
            return a;
        }

        public static IEnumerable<Article> ToModel(this IEnumerable<article> rows)
        {
            foreach (var row in rows)
            {
                yield return row.ToModel();
            }
        }
    }
}