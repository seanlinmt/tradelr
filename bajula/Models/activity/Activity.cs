using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using clearpixels.Facebook.Resources;
using tradelr.Common.Library.Imaging;
using tradelr.Libraries.Imaging;
using tradelr.Library.Constants;
using tradelr.Models.users;

namespace tradelr.Models.activity
{   
    public class Activity
    {
        public string id { get; set; }
        public string ownerName { get; set; }
        public string contactLink { get; set; }
        public DateTime created { get; set; }
        public string title { get; set; }
        public string profile_url { get; set; }
        public long? targetUserid { get; set; }  // who this is for
        public ActivityType type { get; set; }
        public ActivitySource source { get; set; }
        public string sourceIcon { get; set; }
        public string media { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        public int commentsCount { get; set; }

        public IEnumerable<ActivityComment> comments { get; set; }

        public Activity()
        {
            comments = new List<ActivityComment>();
        }
    }

    public static class ActivityHelper
    {
        public static IQueryable<DBML.activity> ToAdminList(this IQueryable<DBML.activity> values, long userid)
        {
            return values.Where(x => x.targetUserid == userid);
        }
        public static IQueryable<DBML.activity> ToUserList(this IQueryable<DBML.activity> values, long userid)
        {
            // no target id means it's a broadcast
            return values.Where(x => x.targetUserid == userid || !x.targetUserid.HasValue);
        }

        private static string ToIcon(this ActivitySource source)
        {
            string retString = "";
            switch (source)
            {
                case ActivitySource.FACEBOOK:
                    retString = "<img src=\"/Content/img/streamsource/facebook.png\" />";
                    break;
                case ActivitySource.TRADELR:
                    retString = "<img src=\"/Content/img/streamsource/tradelr.png\" />";
                    break;
                default:
                    break;
            }
            return retString;
        }

        // for tradelr activities
        public static IEnumerable<Activity> ToModel(this IEnumerable<DBML.activity> values)
        {
            var activitiesList = new List<Activity>();
            foreach (var value in values)
            {
                var act = new Activity()
                {
                    id = value.id.ToString(),
                    targetUserid = value.targetUserid,
                    created = value.created,
                    contactLink = string.Concat(GeneralConstants.URL_SINGLE_CONTACT, value.owner),
                    title = string.Concat("<b>",value.title,"</b>"),
                    ownerName = value.user.ToEmailName(true),
                    profile_url = value.user.GetProfilePhoto(Imgsize.THUMB),
                    source = ActivitySource.TRADELR,
                    sourceIcon = ActivitySource.TRADELR.ToIcon()
                };
                activitiesList.Add(act);
            }
            return activitiesList;
        }

        // for facebook status
        public static IEnumerable<Activity> ToModel(this IEnumerable<Post> values)
        {
            var activitiesList = new List<Activity>();
            foreach (var value in values)
            {
                var act = new Activity()
                {
                    id = value.id,
                    created = value.created_time.ToUniversalTime(),
                    ownerName = value.from.name,
                    profile_url = string.Concat(GeneralConstants.FACEBOOK_GRAPH_HOST, value.from.id, "/picture?type=small").ToHtmlImage(),
                    source = ActivitySource.FACEBOOK,
                    sourceIcon = ActivitySource.FACEBOOK.ToIcon(),
                    contactLink = value.from.id
                };
                switch (value.type)
                {
                    case "checkin":
                    case "photo":
                    case "video":
                    case "link":
                    case "swf":
                        act.title = value.name;
                        act.media = string.Format("<a href=\"{0}\" target=\"_blank\"><img src=\"{1}\"/></a>", value.link, value.picture);
                        act.caption = value.caption;
                        act.description = value.message;
                        break;
                    case "status":
                        act.title = value.message;
                        break;
                    default:
                        throw new NotImplementedException(value.type);
                }

                // handle comments
                if (value.comments != null && value.comments.data != null)
                {
                    act.comments = value.comments.data.ToModel();
                    act.commentsCount = value.comments.count;
                }
                activitiesList.Add(act);
            }
            return activitiesList;
        }
    }
}
