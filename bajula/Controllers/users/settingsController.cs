using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using tradelr.Common.Models;

using tradelr.Libraries;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.facebook;
using tradelr.Models.users;

namespace tradelr.Controllers.users
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    public class settingsController : baseController
    {
        public enum PostTarget
        {
            BLOGGER,
            EBAY,
            FACEBOOK,
            GBASE,
            TRADEME,
            TUMBLR,
            WORDPRESS
        }

        [HttpPost]
        public ActionResult fbalbum(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
                usr.settings ^= ((int) UserSettings.CREATE_ALBUM_FACEBOOK_OWN);
            }
            else
            {
                var token = MASTERdomain.facebook_tokens.SingleOrDefault(x => x.id.ToString() == id);
                if (token != null)
                {
                    token.flags ^= ((int)FacebookTokenSettings.CREATE_ALBUM);
                }
            }
            repository.Save();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult fbstream(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
                usr.settings ^= ((int)UserSettings.POST_TO_FACEBOOK_OWN);
            }
            else
            {
                var token =
                    MASTERdomain.facebook_tokens.SingleOrDefault(x => x.id.ToString() == id);
                if (token != null)
                {
                    token.flags ^= ((int)FacebookTokenSettings.POST_STREAM);
                }
            }
            repository.Save();
            return new EmptyResult();
        }

        public ActionResult haveLocation()
        {
            var sd = repository.GetSubDomains().Single(x => x.id == subdomainid.Value);
            if (sd.organisation.latitude.HasValue)
            {
                return Json(true.ToJsonOKData());
            }
            return Json(false.ToJsonOKData());
        }

        public void metric(int? id)
        {
            if (id.HasValue && id.Value == 1)
            {
                repository.SetMetric(sessionid.Value, true);
            }
            else
            {
                repository.SetMetric(sessionid.Value, false);
            }
        }

        public ActionResult saveLocation(decimal latitude, decimal longtitude)
        {
            bool ok = false;
            try
            {
                var org = repository.GetSubDomains().Single(x => x.id == subdomainid.Value).organisation;
                org.latitude = latitude;
                org.longtitude = longtitude;
                repository.Save();
                ok = true;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }
            return Json(ok.ToJsonOKData());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult offlineAccess()
        {
            try
            {
                repository.ToggleOfflineAccess(subdomainid.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("".ToJsonOKMessage());
        }

        /// <summary>
        /// sets the activity panel as the active panel
        /// </summary>
        /// <returns></returns>
        public void panelActivity(long? tabindex)
        {
            UserSettings? setting = null;
            if (tabindex.HasValue)
            {
                switch (tabindex.Value)
                {
                    case 0:
                        setting = UserSettings.PANEL_GETSTARTED_ACTIVE;
                        break;
                    case 1:
                        setting = UserSettings.PANEL_ACTIVITY_ACTIVE;
                        break;
                    case 2:
                        setting = UserSettings.PANEL_FACEBOOK_ACTIVE;
                        break;
                }
            }
            
            if (setting.HasValue)
            {
                repository.SetActivityPanel(sessionid.Value, setting.Value);
            }
        }

        public void postTo(PostTarget network)
        {
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            if (usr != null)
            {
                switch (network)
                {
                    case PostTarget.BLOGGER:
                        usr.settings ^= ((int)UserSettings.POST_TO_BLOGGER);
                        break;
                    case PostTarget.EBAY:
                        usr.settings ^= ((int)UserSettings.POST_TO_EBAY);
                        break;
                    case PostTarget.GBASE:
                        usr.settings ^= ((int)UserSettings.POST_TO_GOOGLE);
                        break;
                    case PostTarget.FACEBOOK:
                        usr.settings ^= ((int)UserSettings.POST_TO_FACEBOOK);
                        break;
                    case PostTarget.TRADEME:
                        usr.settings ^= ((int)UserSettings.POST_TO_TRADEME);
                        break;
                    case PostTarget.TUMBLR:
                        usr.settings ^= ((int)UserSettings.POST_TO_TUMBLR);
                        break;
                    case PostTarget.WORDPRESS:
                        usr.settings ^= ((int)UserSettings.POST_TO_WORDPRESS);
                        break;
                }
                repository.Save();
            }
        }
    }
}
