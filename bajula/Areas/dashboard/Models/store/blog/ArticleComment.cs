using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;

namespace tradelr.Areas.dashboard.Models.store.blog
{
    public class ArticleComment
    {
        public long id { get; set; }
        public string creator_name { get; set; }
        public string creator_email { get; set; }
        public string posted_time { get; set; }
        public string comment { get; set; }
        public bool isReviewed { get; set; }
        public bool isSpam { get; set; }
    }

    public static class ArticleCommentHelper
    {
        public static ArticleComment ToModel(this article_comment row)
        {
            return new ArticleComment()
            {
                comment = row.comment.ToHtmlParagraph(),
                id = row.id,
                creator_name = row.name,
                creator_email = row.email,
                posted_time = row.created.ToString(GeneralConstants.DATEFORMAT_TIME),
                isReviewed = row.isApproved || row.isSpam,
                isSpam = row.isSpam
            };
        }

        public static IEnumerable<ArticleComment> ToModel(this IEnumerable<article_comment> rows)
        {
            foreach(var row in rows)
            {
                yield return row.ToModel();
            }
        }
    }
}