using System;
using System.Linq;
using System.Web.Mvc;
using clearpixels.Facebook;
using clearpixels.OAuth;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Libraries.Helpers;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.activity;
using tradelr.Models.google.analytics;
using tradelr.Models.home;
using tradelr.Models.subdomain;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    public class dashboardController : baseController
    {
        [RoleFilter(role = UserRole.USER)]
        [TradelrHttps]
        public ActionResult Index()
        {
            // user is logged in
            var user = repository.GetUserById(sessionid.Value, subdomainid.Value);

            // check if account has been setup properly
            if (role.HasFlag(UserRole.CREATOR) && !MASTERdomain.currency.HasValue)
            {
                return RedirectToAction("setup", "register", new { Area = ""});
            }

            var activityGroups = new ActivityGroup
            {
                tradelr = new TradelrActivity()
                {
                    filterList =
                        typeof(ActivityType).ToSelectList().ToFilterList()
                }
            };

            if (role.HasFlag(UserRole.CREATOR))
            {
                // for admin
                // tradelr activity
                activityGroups.tradelr.activities = repository.GetActivities(subdomainid.Value)
                    .OrderByDescending(x => x.id)
                    .ToAdminList(sessionid.Value)
                    .Take(GeneralConstants.ACTIVITY_ROWS_PER_PAGE)
                    .ToModel();
            }
            else
            {
                // for user
                activityGroups.tradelr.activities =
                    repository.GetActivities(subdomainid.Value)
                        .OrderByDescending(x => x.id)
                        .ToUserList(sessionid.Value)
                        .Take(GeneralConstants.ACTIVITY_ROWS_PER_PAGE)
                        .ToModel();
            }


            // handle statistics
            if (role == UserRole.USER)
            {
                // show account stats
                Statistics statistics = null;
                if (baseviewmodel.permission.HasFlag(UserPermission.NETWORK_STORE | UserPermission.TRANSACTION_VIEW | UserPermission.INVENTORY_VIEW))
                {
                    var analytics = new GoogleAnalytics();
                    analytics.GetVisitorStatistics(accountHostname, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
                    statistics = new Statistics(analytics.stats, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
                    statistics.InitSalesAndProductsStatistics(repository, subdomainid.Value, sessionid.Value,
                                                              user.organisation1.MASTERsubdomain);
                }

                // this is for scenario where user is not registered but someone has send them
                // an invoice or purchase order
                var home = new HomeViewData(baseviewmodel)
                {
                    activities = activityGroups,
                    isAdmin = false,
                    stats = statistics
                };

                return View(home);
            }
            else // for admins, god and staff
            {
                // which activity panel?
                var paneltype = repository.GetActivityPanel(sessionid.Value);

                var analytics = new GoogleAnalytics();
                analytics.GetVisitorStatistics(accountHostname, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
                var statistics = new Statistics(analytics.stats, DateTime.UtcNow.AddMonths(-1), DateTime.UtcNow);
                statistics.InitSalesAndProductsStatistics(repository, subdomainid.Value, sessionid.Value,
                                                          user.organisation1.MASTERsubdomain);

                var home = new HomeViewData(baseviewmodel)
                {
                    stats = statistics, // from baseController
                    activities = activityGroups,
                    panelIndex = paneltype.ToIndex(),
                    isAdmin = true
                };

                return View(home);
            }
        }

        [RoleFilter(role = UserRole.USER)]
        [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
        public ActionResult Facebook()
        {
            var viewmodel = new FacebookActivityViewModel();
            var facebookToken = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (facebookToken != null)
            {
                // try to get necessary info from facebook using avaiable token
                var facebook = new FacebookService(facebookToken.token_key);
                var perms = facebook.People.GetPermissions("me");
                if (perms == null || 
                    perms.data.Count == 0 ||
                    perms.data[0].read_stream == 0 ||
                    perms.data[0].publish_stream == 0 ||
                    perms.data[0].manage_pages == 0)
                {
                    viewmodel.requireAdditionalPermission = true;
                }
                else
                {
                    viewmodel.pageList = facebookToken.MASTERsubdomain.facebook_tokens.ToFilterModel();
                }
            }
            else
            {
                viewmodel.requireToken = true;
            }
            return View(viewmodel);
        }


        [RoleFilter(role = UserRole.USER)]
        [HttpPost]
        [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
        public ActionResult StatisticsTime(int? month)
        {
            if (!month.HasValue)
            {
                Syslog.Write("Unknown statistic request");
                return Content("");
            }
            var analytics = new GoogleAnalytics();
            analytics.GetVisitorStatistics(accountHostname, DateTime.UtcNow.AddMonths(month.Value), DateTime.UtcNow);
            var viewdata = new VisitorStatistics(analytics.stats, DateTime.UtcNow.AddMonths(month.Value), DateTime.UtcNow);
            return View(viewdata);
        }
    }
}
