using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using clearpixels.OAuth;
using Ebay;
using OpenSRS;
using OpenSRS.Services;
using eBay.Service.Core.Soap;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.DBML;
using tradelr.DBML.Lucene;
using tradelr.DBML.Models;
using tradelr.Libraries.ActionFilters;
using tradelr.Libraries.Affiliate;
using tradelr.Libraries.file;
using tradelr.Libraries.scheduler;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.account;
using tradelr.Models.activity;
using tradelr.Models.address;
using tradelr.Models.counter;
using tradelr.Models.photos;
using tradelr.Models.products;
using tradelr.Models.subdomain;
using tradelr.Models.transactions;
using tradelr.Models.users;
using Admin = tradelr.Models.admin.Admin;
using Utility = tradelr.Crypto.Utility;

namespace tradelr.Controllers
{
    
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.GOD)]
    public class secretController : baseController
    {
        public ActionResult Index()
        {
            var viewmodel = new Admin(baseviewmodel)
                               {
                                   cacheTimer10Second = HttpRuntime.Cache[CacheTimerType.Seconds10.ToString()] != null,
                                   cacheTimer1Min = HttpRuntime.Cache[CacheTimerType.Minute1.ToString()] != null,
                                   cacheTimer5Min = HttpRuntime.Cache[CacheTimerType.Minute5.ToString()] != null,
                                   cacheTimer10Min = HttpRuntime.Cache[CacheTimerType.Minute10.ToString()] != null,
                                   cacheTimer60Min = HttpRuntime.Cache[CacheTimerType.Minute60.ToString()] != null,
                                   mailQueueLength = repository.GetMails().Count(),
                                   userCount = db.users.Count(),
                                   creatorCount = db.users.Count(x => (x.role & (int)UserRole.CREATOR) != 0),
                                   supportCount = db.adminSupportMessages.Count(),
                                   productCount = db.products.Count(),
                                   orderCount = db.orders.Count()
                               };
            var idlist = HttpContext.Application["sessionidList"] as HashSet<string>;
            if (idlist != null)
            {
                viewmodel.sessionCount = idlist.Count;
            }

            // get list of countries
            var countrygroups = db.MASTERsubdomains.GroupBy(x => x.organisation.country).OrderBy(y => y.Key);
            foreach (var countrygroup in countrygroups)
            {
                var c = string.Format("{0} {1}", countrygroup.Key.HasValue?Country.GetCountry(countrygroup.Key.Value).name:"unknown", countrygroup.Count());
                viewmodel.registeredCountries.Add(c);
            }


            return View(viewmodel);
        }
#if RACKSPACE
        public ActionResult generateCloudFiles()
        {
            var images = repository.GetImagesAll();
            foreach (var image in images)
            {
                Img.UploadImageSizes(image.url);
            }

            return Content("Done");
        }
#endif

        public ActionResult createfeedback()
        {
            foreach (var transaction in db.transactions)
            {
                if (transaction.order.status == OrderStatus.SHIPPED.ToString())
                {
                    var fb = new review
                    {
                        created = DateTime.UtcNow,
                        rating_overall = 3,
                        rating_willshopagain = 3,
                        rating_delivery = 3,
                        rating_support = 3,
                        pending = true,
                        subdomainid = transaction.order.user1.organisation1.subdomain
                    };
                    repository.AddReview(fb);

                    // update transaction
                    transaction.reviewid = fb.id;
                }
            }
            db.SubmitChanges();

            return Content("DONE");
        }

        public ActionResult deletesubdomain(long id)
        {
            try
            {
                repository.DeleteSubdomain(id);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("".ToJsonOKMessage());
        }

        public ActionResult demo_create()
        {
            var account = new Account(repository, "support@tradelr.com", "1234%^&*", "1234%^&*", "demo", AccountPlanType.ULTIMATE, "");
            var status = account.CreateAccountWithLoginPassword();
            if (!status.success)
            {
                return Json(status);
            }

            // add currency and timezoneinfo
            var sd = repository.GetSubDomains().Single(x => x.name == "demo");
            sd.organisation.name = "Demo Company";
            sd.currency = 432; // USD
            sd.flags |= (int)SubdomainFlags.STORE_ENABLED;

            var creator = sd.organisation.users.First();
            creator.timezone = "Eastern Standard Time";
            creator.role = (int)UserRole.ADMIN;

            repository.Save("demo_create");

            // add test org
            var o = new organisation
            {
                subdomain = sd.id,
                address = "12 Demo Road",
                phone = "012 345678",
                name = "Acme & Co.",
                fax = "87 654321",
                postcode = "12345",
            };
            var addedOrgID = repository.AddOrganisation(o);

            // add user
            var u = new user
                        {
                            created = DateTime.UtcNow,
                            email = "demo@tradelr.com",
                            passwordHash = Utility.ComputePasswordHash("demo@tradelr.comdemodemo"),
                            firstName = "demo",
                            lastName = "account",
                            organisation = addedOrgID,
                            viewid = Utility.GetRandomString(),
                            permissions = (int) (UserPermission.NETWORK_STORE |
                                                 UserPermission.INVENTORY_ADD |
                                                 UserPermission.INVENTORY_MODIFY |
                                                 UserPermission.INVENTORY_VIEW |
                                                 UserPermission.INVOICES_ADD |
                                                 UserPermission.TRANSACTION_MODIFY |
                                                 UserPermission.TRANSACTION_VIEW |
                                                 UserPermission.ORDERS_ADD |
                                                 UserPermission.CONTACTS_ADD |
                                                 UserPermission.CONTACTS_MODIFY |
                                                 UserPermission.CONTACTS_VIEW ),
                            role = (int) UserRole.USER
                        };
#if !DEBUG
            // not created when debugging because we want to test product import

            u.timezone = "Eastern Standard Time";
            
#else
            u.permissions |= (int)UserPermission.NETWORK_SETTINGS;
#endif

            repository.AddUser(u);

            // log activity
            repository.AddActivity(u.id,
                    new ActivityMessage(u.id, u.id,
                        ActivityMessageType.CONTACT_NEW,
                         new HtmlLink(u.ToEmailName(true), u.id).ToContactString()), sd.id);

            // update total contacts count
            repository.UpdateCounters(sd.id, 1, CounterType.CONTACTS_PRIVATE);

            // import demo products
            List<ProductInfo> productsList;
            using (var fs =
                    new FileStream(
                        GeneralConstants.APP_ROOT_DIR + "/Content/templates/demo/demo_products.xls",
                        FileMode.Open, FileAccess.Read))
            {
                var importer = new ProductImport();
                productsList = importer.Import(fs, u.id, sd.id);
            }
            repository.AddProducts(productsList, sd.id);

            return Json("Demo account created".ToJsonOKMessage());
        }

        public ActionResult demo_delete()
        {
            var sd = repository.GetSubDomains().SingleOrDefault(x => x.name == "demo");
            if (sd == null)
            {
                return Json("demo account already deleted".ToJsonOKMessage());
            }
            repository.DeleteSubdomain(sd.id);
            return Json("demo account deleted".ToJsonOKMessage());
        }

        public ActionResult InstallMobileTheme()
        {
            foreach (var sd in db.MASTERsubdomains)
            {
                var handler = new ThemeHandler(sd, true);
                var source = handler.GetMobileThemeRepositorySourceDir();

                handler.CopyThemeToUserThemeDirectory(source);
            }

            return Content("done");
        }

        /// <summary>
        /// delete unverified accounts
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public ActionResult DeleteAccountsWithUnverifiedCreatorEmail(bool? commit)
        {
            var unverified =
                db.MASTERsubdomains.Where(
                    x => (x.organisation.users.First().role & (int)UserRole.UNVERIFIED) != 0 &&
                    x.organisation.users.First().created < DateTime.UtcNow.AddDays(-7));


            if (!commit.HasValue || !commit.Value)
            {
                return Content(string.Join(",", unverified.Select(x => string.Concat(x.organisation.users.First().id, ":",x.organisation.users.First().email)).ToArray()));
            }

            var failed = new List<long>();
            foreach (var sd in unverified)
            {
                try
                {
                    repository.DeleteSubdomain(sd.id);
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                    failed.Add(sd.id);
                }
                
            }

            return Content(string.Join(",", failed));
        }

        public ActionResult convertImagesToSafeNames()
        {
            var images = db.images;
            foreach (var image in images)
            {
                var regex = new Regex(@"^([0-9_])+\.jpg");
                var fileName = Path.GetFileName(image.url);
                var match = regex.Match(fileName);
                if (!match.Success)
                {
                    var path = Path.GetDirectoryName(image.url);
                    var oldfile = GeneralConstants.APP_ROOT_DIR + image.url;
                    var newfilename = path + "\\" + DateTime.UtcNow.Ticks + ".jpg";
                    var newfile = GeneralConstants.APP_ROOT_DIR + newfilename;
                    var exists = System.IO.File.Exists(oldfile);
                    if (exists)
                    {
                        try
                        {
                            System.IO.File.Move(oldfile, newfile);
                            image.url = newfilename.Replace(@"\", "/");
                        }
                        catch (Exception ex)
                        {
                            Syslog.Write(ex);
                        }
                    }
                }
            }
            db.SubmitChanges();
            return new EmptyResult();
        }
        
        public ActionResult getsubdomainid(long id)
        {
            var usr = repository.GetUserById(id);
            return Content(usr.organisation1.subdomain.ToString());
        }

        public ActionResult deleteorder(string orders)
        {
            if (orders.Contains("-"))
            {
                var segments = orders.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
                var start = segments[0];
                var end = segments[1] + 1;
                for (long i = start; i < end; i++)
                {
                    repository.DeleteOrder(i);
                }
            }
            else
            {
                repository.DeleteOrder(long.Parse(orders));
            }

            return Json("Order deleted".ToJsonOKMessage());
        }

        public ActionResult deleteproductimages()
        {
            var images = repository.GetImagesAll().Where(x => x.imageType == PhotoType.PRODUCT.ToString());
            db.images.DeleteAllOnSubmit(images);
            db.SubmitChanges();
            return Content("");
        }

        public ActionResult SetAddressAndBillingForContactsWIthout()
        {
            var builder = new StringBuilder();

            foreach (var org in db.organisations)
            {
                var usr = org.users.FirstOrDefault();
                if (usr == null)
                {
                    builder.AppendFormat("{0}{1}", org.id, Environment.NewLine);
                    continue;
                }

                var addressHandler = new AddressHandler(org, repository);
                addressHandler.CopyShippingAndBillingAddressFromOrgAddress(usr.firstName??"", usr.lastName??""); 
            }

            db.SubmitChanges();

            return Content(builder.ToString());
        }

        public ActionResult RegisterDomain(string d)
        {
            // now we start moving the domain
            var opensrs = new Domain();
            var respcode = opensrs.RegisterDomain(d, true, 1, OpenSRSService.TechnicalContact);

            return Content("done");
        }

        public ActionResult GenerateAffiliateID()
        {
            foreach (var d in db.MASTERsubdomains)
            {
                d.affiliateID = AffiliateUtil.GenerateAffiliateID();
                repository.Save();
            }

            return Content("done");
        }

        public ActionResult InitTrialExpiry()
        {
            foreach (var d in db.MASTERsubdomains)
            {
                if (d.accountTypeStatus.HasValue && ((AccountPlanPaymentStatus)d.accountTypeStatus).HasFlag(AccountPlanPaymentStatus.TRIAL))
                {
                    d.trialExpiry = d.organisation.users.First().created.AddDays(30);
                }
            }
            repository.Save();
            return Content("done");
        }

        // following is out of date as we have moved images to new location
        public ActionResult CleanUpUnReferencedImages(bool? commit)
        {
            var sb = new StringBuilder();

            // go through each folder in the image directory
            var path = GeneralConstants.APP_ROOT_DIR + "Uploads/photos";

            var dir = new DirectoryInfo(path);
            foreach (var entry in dir.GetDirectories())
            {
                // find photo entry in db
                var dirname = entry.Name;

                var files = entry.GetFiles();
                foreach (var file in files)
                {
                    var filepath = string.Format("/Uploads/photos/{0}/{1}", dirname, file.Name);
                    var found = db.product_images.Count(x => x.url == filepath) != 0;
                    if (found)
                    {
                        continue;
                    }
                    found = db.images.Count(x => x.url == filepath) != 0;
                    if (found)
                    {
                        continue;
                    }

                    sb.AppendFormat("{0}, ", filepath);

                    // not in db so we delete
                    if (commit.HasValue && commit.Value)
                    {
                        file.Delete();
                    }
                }
            }

            return Content(sb.ToString());
        }

        

        public ActionResult GetDiskUsage(string id)
        {
            var sd = repository.GetSubDomains().SingleOrDefault(x => x.name == id);

            if (sd == null)
            {
                return Content("not found");
            }

            var path_root = string.Format("{0}Uploads/files/{1}", GeneralConstants.APP_ROOT_DIR, sd.uniqueid);

            var image_path = path_root + "/images";
            var media_path = path_root + "/media";
            var theme_main_path = path_root + "/theme";
            var theme_mobile_path = path_root + "/mobile_theme";

            var result = string.Format("images:{0}, media:{1}, theme:{2}",
                                       UtilFile.GetDirectorySize(image_path),
                                       UtilFile.GetDirectorySize(media_path),
                                       UtilFile.GetDirectorySize(theme_main_path) +
                                       UtilFile.GetDirectorySize(theme_mobile_path));
            return Content(result);
        }

        public ActionResult AppendHtmlParagraph(bool? commit)
        {
            foreach (var p in db.products)
            {
                p.details = p.details.ToHtmlParagraph();
            }

            if (commit.HasValue && commit.Value)
            {
                db.SubmitChanges();
            }

            return Content("done");
        }

        #region ebay methods
        public ActionResult PopulateNewEbaySiteDetails(SiteCodeType site)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);

            var service = new CategoryService(token.token_key);

            // get categories
            if (db.ebay_categories.Count(x => x.siteid == site.ToString()) == 0)
            {
                var collections = service.GetCategories(site);

                foreach (CategoryType category in collections)
                {
                    var entry = new ebay_category();
                    entry.siteid = site.ToString();
                    entry.categoryid = int.Parse(category.CategoryID);
                    entry.level = category.CategoryLevel;
                    entry.name = category.CategoryName;
                    entry.parentid = int.Parse(category.CategoryParentID[0]);
                    entry.leaf = category.LeafCategory;

                    db.ebay_categories.InsertOnSubmit(entry);
                }

                repository.Save();
            }

            var resp = service.GetEbayDetails(site);

            // populate dispatch time
            if (db.ebay_dispatchtimes.Count(x => x.siteid == site.ToString()) == 0)
            {
                foreach (DispatchTimeMaxDetailsType entry in resp.DispatchTimeMaxDetails)
                {
                    var time = new ebay_dispatchtime();
                    time.siteid = site.ToString();
                    time.name = entry.Description;
                    time.dispatchTime = entry.DispatchTimeMax;
                    db.ebay_dispatchtimes.InsertOnSubmit(time);
                }

                repository.Save();
            }

            // populate shipping locations
            if (db.ebay_shippinglocations.Count(x => x.siteid == site.ToString()) == 0)
            {
                foreach (ShippingLocationDetailsType entry in resp.ShippingLocationDetails)
                {
                    var loc = new ebay_shippinglocation();
                    loc.description = entry.Description;
                    loc.location = entry.ShippingLocation;
                    loc.siteid = site.ToString();
                    db.ebay_shippinglocations.InsertOnSubmit(loc);
                }
                repository.Save();
            }

            // populate shipping services
            if (db.ebay_shippingservices.Count(x => x.siteid == site.ToString()) == 0)
            {
                // shipping details
                foreach (ShippingServiceDetailsType entry in resp.ShippingServiceDetails)
                {
                    var ship = new ebay_shippingservice();
                    ship.siteid = site.ToString();
                    ship.description = entry.Description;
                    ship.servicetype = entry.ShippingService;
                    ship.isInternational = entry.InternationalService;
                    ship.requiresDimension = entry.DimensionsRequired;
                    ship.requiresWeight = entry.WeightRequired;

                    db.ebay_shippingservices.InsertOnSubmit(ship);
                }
                repository.Save();
            }

            // now go through and populate category related services
            foreach (var category in db.ebay_categories.AsQueryable().Where(x => x.leaf))
            {
                if (category.done.HasValue && category.done.Value)
                {
                    continue;
                }
                var call = service.GetCategoryFeatures(category.categoryid, category.siteid.ToEnum<SiteCodeType>());

                var features = call.CategoryList;

                if (features.Count > 1)
                {
                    Syslog.Write(string.Format("{0} more than 1 features", category.categoryid));
                }

                // get listing duration
                foreach (ListingDurationReferenceType type in call.SiteDefaults.ListingDuration)
                {
                    foreach (ListingDurationDefinitionType def in call.FeatureDefinitions.ListingDurations.ListingDuration)
                    {
                        if (def.durationSetID == type.Value)
                        {
                            foreach (string entry in def.Duration)
                            {
                                var duration = new ebay_listingduration();
                                duration.listingtypeid = type.type.ToString();
                                duration.duration = entry;
                                if (!category.ebay_listingdurations.Any(x => x.listingtypeid == duration.listingtypeid &&
                                    x.duration == duration.duration))
                                {
                                    category.ebay_listingdurations.Add(duration);
                                }
                            }
                        }
                    }
                }

                CategoryFeatureType feature = features[0];

                // if condition is available
                if (feature.ConditionEnabledSpecified && (feature.ConditionEnabled == ConditionEnabledCodeType.Enabled || feature.ConditionEnabled == ConditionEnabledCodeType.Required))
                {
                    //iterate through each condition node
                    foreach (ConditionType condition in feature.ConditionValues.Condition)
                    {
                        var con = new ebay_condition();
                        con.name = condition.DisplayName;
                        con.value = condition.ID;
                        if (!category.ebay_conditions.Any(x => x.name == con.name && x.value == con.value && x.ebayrowid == category.id))
                        {
                            category.ebay_conditions.Add(con);
                            db.SubmitChanges();
                        }
                    }
                }

                if (feature.ReturnPolicyEnabled)
                {
                    category.requiresReturnPolicy = true;
                }
                else
                {
                    category.requiresReturnPolicy = false;
                }
                category.done = true;
                db.SubmitChanges();
            }

            return Content("done");
        }

        public ActionResult FixEbayConditions()
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);

            var service = new CategoryService(token.token_key);

            var tobechecked = db.ebay_categories.Where(x => x.leaf && !x.ebay_conditions.Any());
            foreach (var category in tobechecked)
            {
                var call = service.GetCategoryFeatures(category.categoryid, category.siteid.ToEnum<SiteCodeType>());

                var features = call.CategoryList;

                CategoryFeatureType feature = features[0];

                if (features.Count > 1)
                {
                    return Content("more feature: " + category.categoryid);
                }

                // if condition is available
                if (feature.ConditionEnabledSpecified && (feature.ConditionEnabled == ConditionEnabledCodeType.Enabled || feature.ConditionEnabled == ConditionEnabledCodeType.Required))
                {
                    //iterate through each condition node
                    foreach (ConditionType condition in feature.ConditionValues.Condition)
                    {
                        var con = new ebay_condition();
                        con.name = condition.DisplayName;
                        con.value = condition.ID;
                        if (!category.ebay_conditions.Any(x => x.name == con.name && x.value == con.value && x.ebayrowid == category.id))
                        {
                            category.ebay_conditions.Add(con);
                            db.SubmitChanges();
                        }
                    }
                }
            }

            return Content("done");
        }
        #endregion

        public ActionResult FixTags()
        {
            foreach (var tag in db.tags)
            {
                tag.handle = tag.name.ToPerma();
            }

            foreach (var articleTag in db.article_tags)
            {
                articleTag.handle = articleTag.name.ToPerma();
            }

            repository.Save();

            return Content("done");
        }

        private ActionResult removeDuplicateCollectionMembers(bool? commit)
        {
            var distinctrows = db.productCollectionMembers.AsEnumerable()
                .Distinct(new MyRowComparer()).Select(x => x.id).ToArray();

            if (commit.HasValue && commit.Value)
            {
                var todelete = db.productCollectionMembers.Where(x => !distinctrows.Contains(x.id));
                db.productCollectionMembers.DeleteAllOnSubmit(todelete);
                db.SubmitChanges();
            }

            return Content(string.Join(",", distinctrows));
        }

        public ActionResult UpdateCollectionSettingsToPermanent()
        {
            foreach (var collection in db.product_collections)
            {
                if (collection.name == "Frontpage")
                {
                    collection.settings |= (int)CollectionSettings.PERMANENT;
                }
            }

            db.SubmitChanges();

            return Content("done");
        }

        public ActionResult UpdateShippingProfileId()
        {
            foreach (var sd in db.MASTERsubdomains)
            {
                var prof = sd.shippingProfiles.SingleOrDefault(x => x.title == "Default");

                if (prof == null)
                {
                    continue;
                }

                foreach (var product in sd.products)
                {
                    if (product.shippingProfileID == 0)
                    {
                        product.shippingProfileID = prof.id;
                    }
                }
            }

            db.SubmitChanges();

            return Content("done");
        }

#if LUCENE
        public ActionResult ReIndexLucene()
        {
            LuceneUtil.Instance.ReIndex();

            return Content("done");
        }
#endif
    }


    public class MyRowComparer : IEqualityComparer<productCollectionMember>
    {

        public bool Equals(productCollectionMember x, productCollectionMember y)
        {
            return x.collectionid == y.collectionid && x.productid == y.productid;
        }

        public int GetHashCode(productCollectionMember obj)
        {
            return obj.collectionid.GetHashCode() ^ obj.productid.GetHashCode();
        }
    }

}