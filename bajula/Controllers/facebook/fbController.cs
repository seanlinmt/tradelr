using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using clearpixels.Facebook;
using clearpixels.Facebook.OAuth;
using clearpixels.Facebook.Resources;
using clearpixels.OAuth;
using Facebook;
using tradelr.Common;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.DBML.Models;
using tradelr.Email;
using tradelr.Email.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Extensions;
using tradelr.Library;
using tradelr.Library.Caching.SimpleCache;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.activity;
using tradelr.Models.error;
using tradelr.Models.facebook;
using tradelr.Models.facebook.import;
using tradelr.Models.facebook.viewmodel;
using tradelr.Models.import;
using tradelr.Models.photos;
using tradelr.Models.products;
using tradelr.Models.users;
using Account = tradelr.Models.account.Account;
using Utility = tradelr.Crypto.Utility;

namespace tradelr.Controllers.facebook
{
    //[ElmahHandleError]
    public class fbController : baseController
    {
        public ActionResult Callback()
        {
            var oauthClient = new FacebookOAuthClient(FacebookApplication.Current) { RedirectUri = GetFacebookRedirectUri() };
            FacebookOAuthResult oAuthResult;
            if (oauthClient.TryParseResult(Request.Url, out oAuthResult))
            {
                if (oAuthResult.IsSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(oAuthResult.Code))
                    {
                        string returnUrl = "";
                        string domainName = null;
                        string planName = null;
                        string affiliate = null;
                        var state = new CallbackState();
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(oAuthResult.State))
                            {
                                state = (CallbackState)JsonSerializer.Current.DeserializeObject(Encoding.UTF8.GetString(OAuthFacebook.Base64UrlDecode(oAuthResult.State)), typeof(CallbackState));
                                // TODO: at the moment only check if there is token. Hack Bug
                                // we do this because for logins we are saving token in a separate domain
                                if (!string.IsNullOrEmpty(state.csrf_token) && !ValidateFacebookCsrfToken(state.csrf_token))
                                {
                                    // someone tried to hack the site.
                                    return RedirectToAction("Index", "Error");
                                }

                                if (!string.IsNullOrEmpty(state.return_url))
                                {
                                    returnUrl = state.return_url;
                                }

                                if (!string.IsNullOrEmpty(state.domain_name))
                                {
                                    domainName = state.domain_name;
                                }

                                if (!string.IsNullOrEmpty(state.plan_name))
                                {
                                    planName = state.plan_name;
                                }

                                if (!string.IsNullOrEmpty(state.affiliate))
                                {
                                    affiliate = state.affiliate;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Syslog.Write(ex);
                            // catch incase user puts his own state, 
                            // Base64UrlDecode might throw exception if the value is not properly encoded.
                            return RedirectToAction("Index", "Error");
                        }

                        try
                        {
                            var token = (IDictionary<string, object>)oauthClient.ExchangeCodeForAccessToken(oAuthResult.Code);
                            var token_key = (string) token["access_token"];
                            var token_expires = token.ContainsKey("expires") ? token["expires"].ToString() : "";
                            if (state.isRegistration && !string.IsNullOrEmpty(domainName))
                            {
                                var errorMessage = ProcessSuccesfulFacebookRegistrationCallback(token, domainName, planName, affiliate);

                                if (!string.IsNullOrEmpty(errorMessage))
                                {
                                    return Redirect(ErrorHelper.CreateErrorPage(errorMessage, "/register/" + planName));
                                }
                            }

                            if (state.isLogin || state.isLink)
                            {
                                var returnUri = new Uri(returnUrl);
                                var queryParameters = HttpUtility.ParseQueryString(returnUri.Query);
                                queryParameters.Add("token", token_key);
                                queryParameters.Add("expires", token_expires);
                                returnUrl = string.Format("{0}://{1}{2}{3}", returnUri.Scheme, returnUri.Host, returnUri.LocalPath, queryParameters.ToQueryString(true));
                            }

                            if (state.requestPageTokens && !string.IsNullOrEmpty(state.domain_name))
                            {
                                // obtain any other account tokens
                                var facebook = new FacebookService(token_key);
                                var accounts = facebook.Account.GetAccountTokens("me");
                                if (accounts != null && accounts.data != null)
                                {
                                    var domain =
                                        repository.GetSubDomains().SingleOrDefault(x => x.name == state.domain_name);
                                    if (domain != null)
                                    {
                                        foreach (var entry in accounts.data)
                                        {
                                            if (entry.name != null)
                                            {
                                                var ftoken = new facebook_token
                                                {
                                                    pageid = entry.id,
                                                    subdomainid = domain.id,
                                                    accesstoken = entry.access_token,
                                                    name = entry.name,
                                                    category = entry.category,
                                                    flags = (int)FacebookTokenSettings.NONE
                                                };
                                                repository.AddUpdateFacebookToken(ftoken);
                                            }
                                        }
                                    }
                                }
                            }

                            // save any changes
                            repository.Save();

                            // prevent open redirection attacks. make sure the returnUrl is trusted before redirecting to it
                            if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrl.Contains(GeneralConstants.SUBDOMAIN_HOST))
                            {
                                return Redirect(returnUrl);
                            }
                        }
                        catch (FacebookApiException ex)
                        {
                            // catch incase the user entered dummy code or the code expired.
                            Syslog.Write(ex);
                        }
                    }
                }
                else
                {
                    switch (oAuthResult.ErrorReason)
                    {
                        // permission request denied
                        case "user_denied":
                            return RedirectToAction("NoAuth", "Error");
                        default:
                            Syslog.Write(string.Format("Unhandled Facebook OAUTH {0} - {1}", oAuthResult.ErrorReason, oAuthResult.ErrorDescription));
                            break;
                    }
                }
            }
            return RedirectToAction("Index", "Error");
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult ClearToken()
        {
            try
            {
                repository.DeleteOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK);
                repository.DeleteFacebookTokens(subdomainid.Value);
                foreach (var page in MASTERdomain.facebookPages)
                {
                    repository.DeleteFacebookPage(page);
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return Json(false.ToJsonOKData());
            }
            return Json(true.ToJsonOKData());
        }

        public ActionResult Login(string redirect)
        {
            // try to get from db first
            var oauthClient = new FacebookOAuthClient(FacebookApplication.Current) { RedirectUri = GetFacebookRedirectUri() };

            var relativeUrl = "/login";
            if (!string.IsNullOrEmpty(redirect))
            {
                relativeUrl = string.Concat(relativeUrl,"?redirect=", HttpUtility.UrlEncode(redirect));
            }

            dynamic parameters = new ExpandoObject();
            var state = new CallbackState()
                            {
                                return_url = accountHostname.ToDomainUrl(relativeUrl),
                                isLogin = true
                            };
            
            parameters.state = OAuthFacebook.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Current.SerializeObject(state)));

            return Redirect(oauthClient.GetLoginUrl(parameters).AbsoluteUri);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult GetToken(string perms)
        {
            var oauthClient = new FacebookOAuthClient(FacebookApplication.Current) { RedirectUri = GetFacebookRedirectUri() };

            dynamic parameters = new ExpandoObject();
            parameters.scope = "publish_stream";
            var state = new CallbackState()
            {
                return_url = accountHostname.ToDomainUrl("/fb/saveToken"),
                isLink = true,
                domain_name = MASTERdomain.name
            };
            parameters.state = OAuthFacebook.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Current.SerializeObject(state)));

            return Redirect(oauthClient.GetLoginUrl(parameters).AbsoluteUri);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Friends(string q)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData(), JsonRequestBehavior.AllowGet);
            }
            var facebook = new FacebookService(token.token_key);

            // see if this is cached
            var cachekey = "friends" + token.token_key;
            var cached = SimpleCache.Get(cachekey, SimpleCacheType.FACEBOOK) as ResponseCollection<IdName>;
            if (cached == null)
            {
                var response = facebook.People.GetFriends("me");
                SimpleCache.Add(cachekey, response, SimpleCacheType.FACEBOOK);
                cached = response;
            }

            var friends = cached.data.Where(x => x.name.StartsWith(q, true, CultureInfo.InvariantCulture)).Select(x => new { x.id, x.name });
            return Json(friends.ToList(), JsonRequestBehavior.AllowGet);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult GetComments(string postid)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }

            var facebook = new FacebookService(token.token_key);
            try
            {
                var comments = facebook.Feed.GetComments(postid);
                return Json(comments.data.ToModel().ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        /*
        [RoleFilter(role = UserRole.USER)]
        public ActionResult Import(string id)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK, true);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }
            var facebook = new FacebookService(token.token_key);
            // get albums
            var albums = facebook.Media.GetAlbums("me");
            var collections = new List<FacebookCollection>();
            switch (id)
            {
                case "phototype":
                    foreach (var album in albums.data)
                    {
                        // create product collection
                        var collection = new FacebookCollection
                                             {
                                                 name = album.name, 
                                                 details = album.description
                                             };
                        var photosInAlbum = facebook.Media.GetPhotosInAlbum(album.id);
                        foreach (var photo in photosInAlbum.data)
                        {
                            var product = new FacebookProduct();
                            product.title = photo.name;

                        }

                        collections.Add(collection);
                    }
                    break;
                case "albumtype":
                    break;
                default:
                    return SendJsonErrorResponse("Unknown import method");
            }
        }
        */
        /// <summary>
        /// get facebook stream
        /// </summary>
        /// <returns></returns>
        [RoleFilter(role = UserRole.USER)]
        public ActionResult List(string id, long? until, long? since)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }

            FacebookService facebook;
            ResponseCollection<Post> response;
            if (!string.IsNullOrEmpty(id))
            {
                var pagetoken = token.MASTERsubdomain.facebook_tokens.SingleOrDefault(x => x.pageid == id);
                if (pagetoken == null)
                {
                    return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
                }
                facebook = new FacebookService(pagetoken.accesstoken);
                response = facebook.Feed.GetFeed(id, since, until);
            }
            else
            {
                facebook = new FacebookService(token.token_key);
                response = facebook.Feed.GetHomeFeed(since, until);
            }

            // errrr
            if (response == null)
            {
                var error = facebook.Feed.GetError();
                if (error == null)
                {
                    return Json("Unknown error".ToJsonFail());
                }
                switch (error.type)
                {
                    case ErrorType.OAuthException:
                        return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
                    default:
                        return Json("Unknown error".ToJsonFail());
                }
            }

            var posts = response.data.ToModel();

            var view = this.RenderViewToString("~/Areas/dashboard/Views/dashboard/activityList.ascx", posts);

            return Json(view.ToJsonOKData());
        }


        [RoleFilter(role = UserRole.USER)]
        [HttpPost]
        [JsonFilter(Param = "collection", RootType = typeof(FBImportCollection))]
        public ActionResult ImportCollection(FBImportCollection collection)
        {
            var importer = new ProductImport();
            var pinfos = new List<ProductInfo>();
            var resultlist = new List<ImportResult>();
            foreach (var product in collection.products)
            {
                // check if product has already been imported
                var fbid = product.id;
                var count =
                    repository.GetProducts(subdomainid.Value).SelectMany(x => x.facebook_imports).Where(
                        y => y.facebookID == fbid).Count();
                if (count != 0)
                {
                    var result = new ImportResult
                                     {
                                         id = product.id.ToString(),
                                         message = "already imported",
                                         success = false
                                     };
                    resultlist.Add(result);
                }
                else
                {
                    var pinfo = importer.ImportFacebook(product, collection.access_token, subdomainid.Value);
                    pinfos.Add(pinfo);
                    var result = new ImportResult
                    {
                        id = product.id.ToString(),
                        success = true
                    };
                    resultlist.Add(result);
                }
            }

            // add collection
            var pcollection = new product_collection {name = collection.title, subdomainid = subdomainid.Value};
            if (resultlist.Select(x => x.success).Count() > 0)
            {
                // some products to add so we add collection
                var collectionid = repository.AddProductCollection(pcollection, subdomainid.Value);

                // update products to add with collection id
                foreach (var productInfo in pinfos)
                {
                    var pmember = new productCollectionMember();
                    pmember.collectionid = collectionid;
                    productInfo.p.productCollectionMembers.Add(pmember);
                }
            }

            repository.AddProducts(pinfos, subdomainid.Value);
            return Json(resultlist.ToJsonOKData());
        }

        /// <summary>
        /// our attempt to automatically detect products on facebook and import them
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [JsonFilter(Param = "sproduct", RootType = typeof(FBImportSingle))]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult ImportProduct(FBImportSingle sproduct)
        {
            // check if product has already been imported
            var count =
                repository.GetProducts(subdomainid.Value).SelectMany(x => x.facebook_imports).Count(y => y.facebookID == sproduct.product.id);
            if (count != 0)
            {
                return Json("Product has already been imported".ToJsonFail());
            }
            var importer = new ProductImport();
            var pinfo = importer.ImportFacebook(sproduct.product, sproduct.access_token, subdomainid.Value);
            repository.AddProduct(pinfo, subdomainid.Value);
            return Json(sproduct.product.id.ToJsonOKData());
        }

        [HttpPost]
        [JsonFilter(Param = "albumwithtoken", RootType = typeof(AlbumsWithToken))]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult ImportRow(AlbumsWithToken albumwithtoken)
        {
            if (albumwithtoken == null || albumwithtoken.albums == null)
            {
                return Content("");
            }
            var sb = new StringBuilder();
            foreach (var album in albumwithtoken.albums)
            {
                var sku = album.description.ExtractProductSKU();
                var viewmodel = new FBImportAlbumViewModel
                                    {
                                        id = album.id,
                                        token = albumwithtoken.access_token,
                                        photo_count = album.count,
                                        name = album.name,
                                        details = album.description,
                                        price = album.description.ExtractProductPrice(),
                                        sku = !string.IsNullOrEmpty(sku) ? sku : album.id
                                    };
                viewmodel.photo_links.Add(string.Format("{0}{1}/picture?access_token={2}&type=small",
                                                        GeneralConstants.FACEBOOK_GRAPH_HOST, album.id,
                                                        albumwithtoken.access_token));

                sb.Append(this.RenderViewToString(TradelrControls.facebookImportRow.ToDescriptionString(), viewmodel));
            }
            
            return Content(sb.ToString());
        }

        [HttpPost]
        [JsonFilter(Param = "photos", RootType = typeof(Photo[]))]
        [RoleFilter(role = UserRole.USER)]
        public ActionResult ImportRowContent(IEnumerable<Photo> photos)
        {
            var sb = new StringBuilder();
            var viewmodels = new List<FBImportPhotoViewModel>();
            foreach (var photo in photos)
            {
                var sku = photo.name.ExtractProductSKU();
                var title = photo.name.GetFirstLine();
                var viewmodel = new FBImportPhotoViewModel
                                    {
                                        id = photo.id,
                                        details = photo.name,
                                        price = photo.name.ExtractProductPrice(),
                                        sku = !string.IsNullOrEmpty(sku) ? sku : photo.id,
                                        name = !string.IsNullOrEmpty(sku) ? title.Replace(sku, "") : title
                                    };
                viewmodel.photo_links.Add(photo.images[photo.images.Length - 1].source);
                viewmodels.Add(viewmodel);
            }

            // group similar photos together by sku
            var groups = viewmodels.GroupBy(x => x.sku);
            foreach (IGrouping<string,FBImportPhotoViewModel> group in groups)
            {
                var sku = group.Key;
                var similarskus = viewmodels.Where(x => x.sku == sku);
                FBImportPhotoViewModel similarmodel = null;
                foreach (var similarsku in similarskus)
                {
                    if (similarmodel == null)
                    {
                        similarmodel = new FBImportPhotoViewModel
                        {
                            id = similarsku.id,
                            details = similarsku.details,
                            price = similarsku.price,
                            sku = similarsku.sku,
                            name = similarsku.name,
                            photo_links = similarsku.photo_links
                        };
                    }
                    else
                    {
                        similarmodel.photo_links.AddRange(similarsku.photo_links);
                    }
                    similarmodel.ids.Add(similarsku.id);
                }
                sb.Append(this.RenderViewToString(TradelrControls.facebookImportRowContent.ToDescriptionString(), similarmodel));
            }
            
            return Content(sb.ToString());
        }

        public ActionResult Permissions(string scope)
        {
            // connect with facebook if we have no token
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                //we need to get user to connect with facebook first
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData(), JsonRequestBehavior.AllowGet);
            }

            var oauthClient = new FacebookOAuthClient(FacebookApplication.Current) { RedirectUri = GetFacebookRedirectUri() };
            
            dynamic parameters = new ExpandoObject();
            parameters.scope = scope;
            var state = new CallbackState()
                            {
                                return_url = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : accountHostname.ToDomainUrl("/dashboard"), 
                                requestPageTokens = true,
                                domain_name = accountSubdomainName
                            };
            parameters.state = OAuthFacebook.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Current.SerializeObject(state)));

            return Redirect(oauthClient.GetLoginUrl(parameters).AbsoluteUri);
        }

        public ActionResult Register(string name, string plan, string affiliate)
        {
            if (!repository.IsDomainAvailable(name))
            {
                return Redirect(ErrorHelper.CreateErrorPage(name + " is not available. Please select another name.", Request.UrlReferrer.AbsoluteUri));
            }

            var oauthClient = new FacebookOAuthClient(FacebookApplication.Current) { RedirectUri = GetFacebookRedirectUri() };

            dynamic parameters = new ExpandoObject();

            // TODO: to be handled separately
            parameters.scope = "email,publish_stream";
            var returnUrl = name.ToTradelrDomainUrl("/login");

            // add csrf_token to prevent cross site forger attacks
            // pass returnUrl as state, so the callback know which page to redirect when the oauth login is successful
            // to make the querystring ?state=value safe, encode the value of state using Base64UrlEncode.
            var state = new CallbackState()
                            {
                                csrf_token = Utility.CalculateMD5Hash(Guid.NewGuid().ToString()),
                                return_url = returnUrl,
                                domain_name = name,
                                plan_name = plan,
                                isRegistration = true,
                                affiliate = affiliate
                            };

            parameters.state = OAuthFacebook.Base64UrlEncode(Encoding.UTF8.GetBytes(JsonSerializer.Current.SerializeObject(state)));
            SetFacebookCsrfToken(state.csrf_token);
            
            return Redirect(oauthClient.GetLoginUrl(parameters).AbsoluteUri);
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult SaveToken(string token, string expires)
        {
            var facebook = new FacebookService(token);
            var fbUsr = facebook.People.GetUser("me");

            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            usr.FBID = fbUsr.id;

            DateTime expiresOn = !string.IsNullOrEmpty(expires) ? DateTime.UtcNow.AddSeconds(Convert.ToDouble(expires)) : DateTime.MaxValue;
            var oauthdb = new oauth_token
            {
                token_key = token,
                token_secret = "",
                type = OAuthTokenType.FACEBOOK.ToString(),
                subdomainid = subdomainid.Value,
                appid = sessionid.Value.ToString(),
                authorised = true,
                expires = expiresOn
            };
            repository.AddOAuthToken(oauthdb);

            repository.Save();

            return Redirect("/dashboard/networks#facebook");
        }

        
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Profile(string id)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json("<a href='/dashboard/networks#facebook'>Please connect with facebook first</a>".ToJsonFail());
            }

            var facebook = new FacebookService(token.token_key);
            var usr = facebook.People.GetUser(id);

            var viewdata = usr.ToModel();
            var view = this.RenderViewToString(TradelrControls.facebook_profile.ToDescriptionString(), viewdata);
            var title = string.Format("<b class='larger'>{0}</b> <span class='notbold'>{1}</span>", viewdata.fullName, viewdata.screenName);
            return Json(view.ToJsonOKData(title));
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Search(string q, string type, string friendid, int offset = 0, int limit = 25)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }
            var facebook = new FacebookService(token.token_key);
            switch (type)
            {
                case "post":
                    var response_post = facebook.Search.SearchPosts(q, friendid);
                    if (response_post == null)
                    {
                        return Json("".ToJsonOKData());
                    }
                    var posts = response_post.data.ToModel();
                    var view_posts = this.RenderViewToString("~/Areas/dashboard/Views/dashboard/activityList.ascx", posts);
                    return Json(view_posts.ToJsonOKData());
                case "user":
                    var response_user = facebook.Search.SearchUsers(q);
                    if (response_user == null)
                    {
                        return Json("".ToJsonOKData());
                    }
                    
                    var view_users = this.RenderViewToString("~/Areas/dashboard/Views/activity/facebookUserList.ascx", response_user.data);
                    return Json(view_users.ToJsonOKData());
                case "page":
                    var response_page = facebook.Search.SearchPages(q, limit, offset);
                    if (response_page == null)
                    {
                        return Json("".ToJsonOKData());
                    }

                    var view_pages = this.RenderViewToString("~/Areas/dashboard/Views/activity/facebookPageList.ascx", response_page.data);
                    return Json(view_pages.ToJsonOKData());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Send(string data, string pageid)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }

            var facebook = new FacebookService(token.token_key);
            try
            {
                var message = data;
                // need to extract the first link
                var urls = data.UrlsFromText();
                if (urls == null || urls.Length == 0)
                {
                    facebook.Feed.PostToHomeFeed(pageid, message);
                }
                else
                {
                    facebook.Feed.PostToHomeFeed(pageid, message, urls[0]);
                }
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("".ToJsonOKMessage());
        }

        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult SendComment(string data, string postid)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, sessionid.Value.ToString(), OAuthTokenType.FACEBOOK);
            if (token == null)
            {
                return Json(JavascriptReturnCodes.NOTOKEN.ToJsonOKData());
            }

            var facebook = new FacebookService(token.token_key);
            try
            {
                var message = HttpUtility.HtmlEncode(data);
                var commentid = facebook.Feed.PostComment(postid, message);
                // get comment
                var comment = facebook.Feed.GetSingleComment(commentid.id);
                return Json(comment.ToModel().ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        /// <summary>
        /// given pageid, returns access_token. normally used for making js calls to graph api
        /// </summary>
        /// <param name="ids">comma separated ids</param>
        /// <returns></returns>
        [RoleFilter(role = UserRole.CREATOR)]
        public ActionResult Tokens(string id)
        {
            /*
#if DEBUG
            // 100001422423614 : lady-store macau
            // 100001610031102 :dot dot closet
            const string TESTSTOREID = "100001422423614";
            var debugresult = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK, true).token_key;
            return Json(new { id = TESTSTOREID, token = debugresult }.ToJsonOKData());
#endif
             * */
            if (string.IsNullOrEmpty(id))
            {
                var access_token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK, true).token_key;
                return Json(new { id = "me", token = access_token }.ToJsonOKData());
            }
            else
            {
                var result =
                    MASTERdomain.facebook_tokens.Where(x => x.id.ToString() == id).
                        Select(x => new { id = id, token = x.accesstoken }).Single();
                return Json(result.ToJsonOKData());
            }

        }

        private void SetFacebookCsrfToken(string csrfToken)
        {
            Session["fb_csrf_token"] = csrfToken;
        }

        private bool ValidateFacebookCsrfToken(string csrfToken)
        {
            var result = Session["fb_csrf_token"] != null && (string)Session["fb_csrf_token"] == csrfToken;
            Session["fb_csrf_token"] = null;
            return result;
        }

        private Uri GetFacebookRedirectUri()
        {
            return new Uri(string.Format("{0}/fb/callback", GeneralConstants.HTTP_SECURE));
        }

        private string ProcessSuccesfulFacebookRegistrationCallback(IDictionary<string, object> result, string domainName, string planName, string affiliate)
        {
            var accessToken = (string)result["access_token"];

            // incase the expires on is not present, it means we have offline_access permission
            DateTime expiresOn = result.ContainsKey("expires") ? DateTime.UtcNow.AddSeconds(Convert.ToDouble(result["expires"])) : DateTime.MaxValue;

            // create subdomain
            var facebook = new FacebookService(accessToken);
            var fbUsr = facebook.People.GetUser("me");
            var errorMessage = "";
            if (fbUsr == null)
            {
                errorMessage = "Could not obtain facebook authorization";
                Syslog.Write(errorMessage);
                return errorMessage;
            }

            // verify that email has not been used to register another account
            if (repository.GetUsersByEmail(fbUsr.email).SingleOrDefault(x => (x.role & (int)UserRole.CREATOR) != 0) != null)
            {
                errorMessage = string.Format("{0} is in use. Please choose a different email address.", fbUsr.email);
                Syslog.Write(errorMessage);
                return errorMessage;
            }

            // CHECK THAT FBID NOT ALREADY BEEN USED
            if (repository.GetUserByFBID(fbUsr.id).SingleOrDefault(x => (x.role & (int)UserRole.CREATOR) != 0) != null)
            {
                errorMessage = "Facebook ID is in use. ";
                Syslog.Write(errorMessage + fbUsr.id);
                return errorMessage;
            }
            
            var account = new Account(repository, fbUsr.email, domainName, planName.ToEnum<AccountPlanType>(), affiliate);
            errorMessage = account.CreateAccountWithFacebookLogin(fbUsr);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            try
            {
                // generate photo
                new Thread(() => account.usr.externalProfilePhoto.ReadAndSaveFromUrl(account.mastersubdomain.id, account.usr.id, account.usr.id, PhotoType.PROFILE)).Start();

                // add access token
                var oauthdb = new oauth_token
                {
                    token_key = accessToken,
                    token_secret = "",
                    type = OAuthTokenType.FACEBOOK.ToString(),
                    subdomainid = account.mastersubdomain.id,
                    appid = account.usr.id.ToString(),
                    authorised = true,
                    expires = expiresOn
                };

                repository.AddOAuthToken(oauthdb);

                // send confirmation email
                var viewdata = new ViewDataDictionary()
                                   {
                                       {"host", domainName.ToTradelrDomainUrl("")},
                                       {"confirmCode", account.usr.confirmationCode},
                                       {"email", account.usr.email}
                                   };
                EmailHelper.SendEmailNow(EmailViewType.ACCOUNT_CONFIRMATION, viewdata, "New Account Details and Email Verification Link",
                               account.usr.email, account.usr.ToFullName(), null);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return "An error has occurred creating your account";
            }

            return "";
        }
    }
}
