using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.activity;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class activityController : baseController
    {
        public ActionResult Destroy(string id)
        {
            repository.DeleteActivity(long.Parse(id),sessionid.Value);

            return Json("".ToJsonOKMessage());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="since_id">last activity id that is being displayed</param>
        /// <param name="max_id">max value of activity id</param>
        /// <param name="page"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult List(long? since_id, long? max_id, int? page, int? type)
        {
            var query = repository.GetActivities(subdomainid.Value);

            if (since_id.HasValue)
            {
                query = query.Where(x => x.id > since_id.Value);
            }

            if (max_id.HasValue)
            {
                query = query.Where(x => x.id < max_id.Value);
            }

            if (!page.HasValue)
            {
                page = 1;
            }

            if (type.HasValue)
            {
                var activityType = type.Value.ToEnum<ActivityType>().ToString();
                query = query.Where(x => x.type == activityType);
            }

            query = query.OrderByDescending(x => x.id);

            IEnumerable<Activity> activities;
            if ((role & UserRole.CREATOR) == 0)
            {
                activities = query
                    .ToUserList(sessionid.Value)
                    .Skip(GeneralConstants.ACTIVITY_ROWS_PER_PAGE * (page.Value - 1))
                    .Take(GeneralConstants.ACTIVITY_ROWS_PER_PAGE)
                    .ToModel();
            }
            else
            {
                activities = query
                        .ToAdminList(sessionid.Value)
                    .Skip(GeneralConstants.ACTIVITY_ROWS_PER_PAGE * (page.Value - 1))
                    .Take(GeneralConstants.ACTIVITY_ROWS_PER_PAGE)
                        .ToModel();
            }

            var view = this.RenderViewToString("~/Areas/dashboard/Views/dashboard/activityList.ascx", activities);
            
            return Json(view.ToJsonOKData());
        }
    }
}
