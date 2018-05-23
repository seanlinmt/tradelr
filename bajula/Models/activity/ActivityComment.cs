using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using clearpixels.Facebook.Resources;
using tradelr.Libraries.Imaging;
using tradelr.Library.Constants;

namespace tradelr.Models.activity
{
    // modifying the following will break facebook posting comments!!!!!
    public class ActivityComment
    {
        public string created { get; set; } // ToString("s") format
        public string commenter { get; set; }
        public string contactLink { get; set; }
        public string profile_url { get; set; }
        public string message { get; set; }
    }


    public static class ActivityCommentHelper
    {
        public static ActivityComment ToModel(this Comment fbcomment)
        {
            return new ActivityComment
            {
                commenter = fbcomment.from.name,
                contactLink = fbcomment.from.id,
                created = fbcomment.created_time.ToUniversalTime().ToString("s"),
                profile_url =
                    string.Concat(GeneralConstants.FACEBOOK_GRAPH_HOST, fbcomment.from.id,
                                  "/picture?type=small").ToHtmlImage("commentphoto"),
                message = fbcomment.message
            };
        }

        public static IEnumerable<ActivityComment> ToModel(this IEnumerable<Comment> fbcomments)
        {
            foreach (var fbcomment in fbcomments)
            {
                yield return fbcomment.ToModel();
            }
        }
    }

}