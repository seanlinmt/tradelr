using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using clearpixels.Facebook;
using clearpixels.Facebook.Resources;
using clearpixels.OAuth;
using Ebay;
using Google.GData.Client;
using TradeMe.services;
using api.trademe.co.nz.v1;
using tradelr.Common.Constants;
using tradelr.Controllers;
using tradelr.Libraries.ActionFilters;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Models.facebook;
using tradelr.Models.google.blog;
using tradelr.Models.google.gbase;
using tradelr.Models.networks.viewmodels;
using tradelr.Models.users;
using HttpUtility = System.Web.HttpUtility;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.NETWORK_SETTINGS)]
    [TradelrHttps]
    public class networksController : baseController
    {
        public ActionResult Index()
        {
            return View(baseviewmodel);
        }

        public ActionResult blogger()
        {
            var viewdata = new NetworkViewModel();

            viewdata.bloggerSessionKey = MASTERdomain.bloggerSessionKey;
            viewdata.blogList = MASTERdomain.googleBlogs.ToModel();

            var continueUrl = string.Concat(GeneralConstants.HTTP_HOST, "/callback?sd=", accountHostname, "&path=",
                                            HttpUtility.UrlEncode("/dashboard/blogger/saveToken"));
            viewdata.requestUrl = AuthSubUtil.getRequestUrl(continueUrl, GoogleConstants.FEED_BLOGGER, false, true);
            return View(viewdata);
        }

        public ActionResult ebay()
        {
            var viewmodel = new EbayNetworkViewModel();
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);
            if (token != null)
            {
                var ebayservice = new UserService(token.token_key);
                var user = ebayservice.GetUser();
                var username = user.UserID;
                var feedback = user.FeedbackScore;
                var feedbackicon = "";
                if (feedback >= 10 && feedback < 50)
                {
                    feedbackicon = "/Content/img/networks/ebay/yellow.gif";
                }
                else if (feedback >= 50 && feedback < 99)
                {
                    feedbackicon = "/Content/img/networks/ebay/blue.gif";
                }
                else if (feedback >= 100 && feedback < 499)
                {
                    feedbackicon = "/Content/img/networks/ebay/teal.gif";
                }
                else if (feedback >= 500 && feedback < 999)
                {
                    feedbackicon = "/Content/img/networks/ebay/purple.gif";
                }
                else if (feedback >= 1000 && feedback < 4999)
                {
                    feedbackicon = "/Content/img/networks/ebay/red.gif";
                }
                else if (feedback >= 5000 && feedback < 9999)
                {
                    feedbackicon = "/Content/img/networks/ebay/green.gif";
                }
                else if (feedback >= 10000 && feedback < 24999)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootyellow.gif";
                }
                else if (feedback >= 25000 && feedback < 49999)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootteal.gif";
                }
                else if (feedback >= 50000 && feedback < 99999)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootpurple.gif";
                }
                else if (feedback >= 100000 && feedback < 499999)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootred.gif";
                }
                else if (feedback >= 500000 && feedback < 999999)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootgreen.gif";
                }
                else if (feedback >= 1000000)
                {
                    feedbackicon = "/Content/img/networks/ebay/shootsilver.gif";
                }

                viewmodel.EbayProfileUrl =
                    string.Format(
                        "<span class='pr10 block larger pb10'><a target='_blank' href='{3}{0}'>{0} </a> (<img src='{1}' >{2})</span>",
                        username,
                        feedbackicon,
                        feedback,
                        GeneralConstants.DEBUG ? "http://myworld.sandbox.ebay.com/" : "http://myworld.ebay.com/");
            }

            if (MASTERdomain.ebay_lastsync.HasValue)
            {
                viewmodel.lastSync = MASTERdomain.ebay_lastsync.Value.ToString("s");
            }

            return View(viewmodel);
        }

        public ActionResult facebook()
        {
            var viewmodel = new NetworkViewModel()
                               {
                                   facebookStreams = new List<SelectListItem>()
                               };
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK, true);
            if (token != null)
            {
                // get tokens
                viewmodel.facebookStreams.Add(new SelectListItem() { Value = "", Text = "personal account" });
                var fbTokens = MASTERdomain.facebook_tokens;
                foreach (var fbToken in fbTokens)
                {
                    var val = fbToken.id.ToString();
                    var text = string.Format("{0} ({1})", fbToken.name, fbToken.category);
                    viewmodel.facebookStreams.Add(new SelectListItem() { Text = text, Value = val });
                }

                // get user profile
                var facebook = new FacebookService(token.token_key);
                var usr = facebook.People.GetUser("me");
                if (usr != null)
                {
                    var profileLink = usr.link;
                    var profilePhoto = string.Concat(GeneralConstants.FACEBOOK_GRAPH_HOST, usr.id, "/picture?type=small");
                    viewmodel.FacebookProfileUrl = string.Format("<a href='{0}' target='_blank'><img src='{1}'/></a>", profileLink, profilePhoto);
                    viewmodel.FacebookFeeds = token.MASTERsubdomain.facebook_tokens.ToModel();
                }
                else
                {
                    var fb_err = facebook.People.GetError();
                    if (fb_err != null && fb_err.type == ErrorType.OAuthException)
                    {
                        repository.DeleteOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK);
                        repository.DeleteFacebookTokens(subdomainid.Value);
                    }
                }
            }
            
            return View(viewmodel);
        }

        public ActionResult gbase()
        {
            int countryid = 0;
            if (MASTERdomain.gbaseid.HasValue)
            {
                countryid = MASTERdomain.googleBase.country;
            }

            var viewmodel = new GoogleBaseData();
            viewmodel.countries = GoogleBaseData.SupportedCountries.Select(x => new SelectListItem()
                                                                                    {
                                                                                        Selected = x.id == countryid,
                                                                                        Text = x.name,
                                                                                        Value = x.id.ToString()
                                                                                    });

            return View(viewmodel);
        }

        public ActionResult haveToken(OAuthTokenType type)
        {
            var oauth = MASTERdomain.oauth_tokens.SingleOrDefault(x => x.type == type.ToString() && x.authorised);

            if (oauth == null)
            {
                return Json(false.ToJsonOKData());
            }

            // check that token has not expired
            if (oauth.expires != null && DateTime.UtcNow > oauth.expires)
            {
                repository.DeleteOAuthToken(subdomainid.Value, type);
                return Json(false.ToJsonOKData());
            }

            return Json(true.ToJsonOKData());
        }

        public ActionResult shipwire()
        {
            var viewdata = new NetworkViewModel();
            return View(viewdata);
        }

        public ActionResult trademe()
        {
            var viewmodel = new TrademeNetworkViewModel();
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);
            if (token != null)
            {
                var tm = new MyTrademeService(token.token_key, token.token_secret);
                var member = tm.GetMemberSummary(new GetMemberSummaryRequest());

                if (member.GetMemberSummaryResult != null)
                {
                    viewmodel.trademeProfileUrl =
                    string.Format(
                        "<span class='pr10 block larger pb10'><a target='_blank' href='{0}/Members/Listings.aspx?member={1}'>{2} ({3})</a></span>",
                        GeneralConstants.DEBUG ? "http://www.tmsandbox.co.nz" : "http://www.trademe.co.nz",
                        member.GetMemberSummaryResult.MemberId,
                        member.GetMemberSummaryResult.Nickname,
                        member.GetMemberSummaryResult.TotalFeedback);
                }
                else
                {
                    // permission deleted on trademe site
                    repository.DeleteOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME);
                }
            }

            return View(viewmodel);
        }

        public ActionResult tumblr()
        {
            return View();
        }

        public ActionResult wordpress()
        {
            var viewdata = new NetworkViewModel();
            return View(viewdata);
        }
    }
}
