using System;
using System.Collections.Generic;
using DotLiquid;
using tradelr.DBML;
using tradelr.Library;

namespace tradelr.Models.liquid.models.Blog
{
    public class Comment : Drop
    {
        public long id { get; set; }
        public string author { get; set; }
        public string email { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public string status { get; set; }
        public string url { get; set; }
    }

    public static class CommentHelper
    {
        public static IEnumerable<Comment> ToLiquidModel(this IEnumerable<article_comment> rows)
        {
            foreach (var row in rows)
            {
                yield return new Comment()
                                 {
                                     id = row.id,
                                     author = row.name,
                                     email = row.email,
                                     content = row.comment.ToHtmlParagraph(),
                                     created_at = row.created,
                                     status = row.ToStatus(),
                                     url = string.Format("/blogs/{0}/{1}-{2}#{3}",
                                                         row.article.blog.permalink,
                                                         row.article.id,
                                                         row.article.title.ToLower().ToSafeUrl(),
                                                         row.id)
                                 };
            }
        }

        private static string ToStatus(this article_comment row)
        {
            if (row.isSpam)
            {
                return "spam";
            }

            if (row.isApproved)
            {
                return "published";
            }

            return "unapproved";
        }
    }
}