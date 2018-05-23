using System;
using System.Linq;
using tradelr.Library.Caching;
using tradelr.Models.activity;
using tradelr.Models.users;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddActivity(long creator, ActivityMessage message, long subdomainid)
        {
            // if so check if it allows multiples don't need to check
            if (!message.AllowMultiple())
            {
                // check if db already contains entry
                var count = db.activities.Count(x => x.type == message.GetMessageType()
                                                     && x.appid == message.appid);

                if (count != 0)
                {
                    return;
                }
            }

            var activity = new activity
            {
                appid = message.appid,
                owner = creator,
                title = message.Format(),
                type = message.GetMessageType(),
                created = DateTime.UtcNow,
                targetUserid = message.targetUserid
            };
            db.activities.InsertOnSubmit(activity);

            CacheHelper.Instance.invalidate_dependency(DependencyType.activities, subdomainid.ToString());
        }

        public void DeleteActivity(long activityID, long owner)
        {
            var acts = db.activities.Where(x => x.owner == owner && x.id == activityID);
            db.activities.DeleteAllOnSubmit(acts);
            db.SubmitChanges();
        }

        public IQueryable<activity> GetActivities(long subdomainid)
        {
            return db.activities.Where(x => x.user.organisation1.subdomain == subdomainid);
        }

        public void SetActivityPanel(long owner, UserSettings setting)
        {
            var usr = db.users.Where(x => x.id == owner).SingleOrDefault();
            if (usr != null)
            {
                usr.settings = (usr.settings & (int)UserSettingsMask.ACTIVITY_MASK) | (int)setting;
                db.SubmitChanges();
            }
        }

        public UserSettings GetActivityPanel(long owner)
        {
            var usr = db.users.Where(x => x.id == owner).SingleOrDefault();
            return (UserSettings)(usr.settings & (int)~UserSettingsMask.ACTIVITY_MASK);
        }
    }
}