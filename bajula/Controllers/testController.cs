using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Serialization;
using clearpixels.Facebook;
using clearpixels.OAuth;
using Ebay;
using FacebookToolkit.Rest;
using Google.GData.Client;
using GoogleBase;
using PhoneNumbers;
using Shipwire.tracking;
using TradeMe;
using TradeMe.models;
using TradeMe.services;
using api.trademe.co.nz.v1;
using eBay.Service.Core.Soap;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Email.Models;
using tradelr.Libraries.Facebook;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.ebay;
using tradelr.Models.store.customcss;
using tradelr.Models.transactions;
using tradelr.Models.users;
using tradelr.Models.yahoo;
using Category = api.trademe.co.nz.v1.Category;
using Exception = System.Exception;
using Image = System.Drawing.Image;

namespace tradelr.Controllers
{
    public class testController : baseController
    {
#if DEBUG
        public ActionResult crashme()
        {
            Syslog.Write("test");
            return Content("done");
        }

        public ActionResult crashmehandled()
        {
            try
            {
                var p = (long)Session["not"];
                return Json(p);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        public ActionResult m()
        {
            var o = repository.GetOrder(30);
            var viewloc = o.user1.organisation1.MASTERsubdomain.ToHostName().ToDomainUrl(o.ToOrderLink());

            // notify buyer that order has been shipped
            var emailContent = new OrderShippedEmailContent
            {
                orderNumber = o.orderNumber.ToString("D8"),
                shippingAddress = o.user1.organisation1.ToOrganisationAddress(false, true),
                sender = o.user.ToEmailName(false),
                viewloc = viewloc
            };

            string subject = "Order #" + emailContent.orderNumber + " has shipped";
            var msg = new Models.message.Message(o.user1, o.user, subdomainid.Value);
            var controller = new dummyController();
            msg.SendMessage(controller, repository, EmailViewType.ORDER_SHIPPED, emailContent, subject, viewloc);

            return new EmptyResult();
        }

        public ActionResult n()
        {
            var imageloc = repository.GetUserById(sessionid.Value).image.url;

            Image img = Image.FromFile(GeneralConstants.APP_ROOT_DIR + imageloc);
            var ms = new MemoryStream();
            img.Save(ms,ImageFormat.Gif);
            var bitmap = new Bitmap(ms);
            var palette = bitmap.Palette;
            //var palette = ImgHelper.GetHTMLPalette(bt);
            /*
            var histogram = new Dictionary<string, int>();
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    var color = bt.GetPixel(x, y);
                    var name = color.R.ToString("X") + color.G.ToString("X") + color.B.ToString("X");
                    int count;
                    if(!histogram.TryGetValue(name, out count))
                    {
                        histogram.Add(name, 0);
                        continue;
                    }
                    histogram[name] = count + 1;
                }
            }

            var sorted = histogram.OrderByDescending(x => x.Value).Select(x => x.Key);
            */
            var serializer = new JavaScriptSerializer();
            var data = serializer.Serialize(palette);

            return Content(data);
        }

        public ActionResult nn()
        {
            var css =
                "{\"background\":\"rgb(89,79,79)\",\"text\":\"rgb(84,121,128)\",\"link\":\"rgb(69,173,168)\",\"navigation\":\"rgb(157, 224, 173)\",\"border\":\"rgb(229,252,194)\"}";

            var serializer = new JavaScriptSerializer();
            var cssmodel = serializer.Deserialize<CustomCss>(css);
            var background = cssmodel.navigation.FromRGBToColor();
            double hue;
            double saturation;
            double value;
            ColourHelper.ColourToHSV(background, out hue, out saturation, out value);
            value = value*0.8;
            var darker = ColourHelper.ColourFromHSV(hue, saturation, value);

            return Content(serializer.Serialize(background) + serializer.Serialize(darker));
        }

        public ActionResult o()
        {
            const string xml = "<TrackingUpdateResponse><Status>Test</Status>" +
                               "<Order id=\"2986\" shipped=\"YES\" shipper=\"UPS GD\" shipperFullName=\"UPS Ground\" shipDate=\"2010-09-29 13:00:00\" expectedDeliveryDate=\"2010-09-30 12:00:00\" handling=\"1.00\" shipping=\"13.66\" packaging=\"0.00\" insurance=\"0.00\" total=\"14.66\">" +
                               "<TrackingNumber carrier=\"UPS\" href=\"http://wwwapps.ups.com/etracking/tracking.cgi?TypeOfInquiryNumber=T&amp;track=Track&amp;InquiryNumber1=1ZW682E90326614239\">1ZW682E90326614239</TrackingNumber>" +
                               "</Order>" +
                               "<Order id=\"2987\" shipped=\"YES\" shipper=\"UPS GD\" shipperFullName=\"UPS Ground\" shipDate=\"2010-09-29 13:00:00\" expectedDeliveryDate=\"2010-09-30 12:00:00\" handling=\"1.50\" shipping=\"9.37\" packaging=\"0.00\" insurance=\"0.00\" total=\"10.87\">" +
                               "<TrackingNumber carrier=\"UPS\" href=\"http://wwwapps.ups.com/etracking/tracking.cgi?TypeOfInquiryNumber=T&amp;track=Track&amp;InquiryNumber1=1ZW682E90326795080\">1ZW682E90326795080</TrackingNumber>" +
                               "</Order>" +
                               "<Order id=\"2988\" shipped=\"NO\" shipper=\"UPS GD\" shipperFullName=\"UPS Ground\" />" +
                               "<TotalOrders>3</TotalOrders>" +
                               "<TotalShippedOrders>2</TotalShippedOrders>" +
                               "<TotalProducts>3</TotalProducts>" +
                               "<Bookmark>2006-04-28 20:35:45</Bookmark></TrackingUpdateResponse>";
            TrackingUpdateResponse response;
            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof (TrackingUpdateResponse));
                response = serializer.Deserialize(reader) as TrackingUpdateResponse;
            }
            return Content("OK");
        }

        public ActionResult r()
        {
            var datauri = "data:image/jpeg;base64,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";
            var url = datauri.ToSavedImageUrl(1, subdomainid.Value);
            return new EmptyResult();
        }

        // this gets friend's details
        public ActionResult t()
        {
            var owner = db.GetUserById(sessionid.Value, subdomainid.Value);
            var connectSession = UtilFacebook.GetConnectSession();
            if (connectSession.IsConnected())
            {
                var api = new Api(connectSession);
                var friends = api.Friends.Get(long.Parse(owner.FBID));
                foreach (var friend in friends)
                {
                    var details = api.Users.GetInfo(friend);
                }
            }

            return new EmptyResult();
        }

        public ActionResult s()
        {
            var jsonstring =
                "{\"contacts\":{\"start\":0,\"count\":2,\"total\":2,\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contacts\",\"contact\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"isConnection\":false,\"id\":559,\"fields\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/nickname/562\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":562,\"type\":\"nickname\",\"value\":\"sean\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/email/563\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":563,\"type\":\"email\",\"value\":\"seanlinmt@gmail.com\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/559/name/561\",\"created\":\"2007-06-21T08:33:01Z\",\"updated\":\"2007-06-21T08:33:01Z\",\"id\":561,\"type\":\"name\",\"value\":{\"givenName\":\"sean\",\"middleName\":\"lin\",\"familyName\":\"\",\"prefix\":\"\",\"suffix\":\"\",\"givenNameSound\":\"\",\"familyNameSound\":\"\"},\"editedBy\":\"OWNER\",\"flags\":[]}],\"categories\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"isConnection\":false,\"id\":560,\"fields\":[{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/nickname/565\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":565,\"type\":\"nickname\",\"value\":\"clearpixels\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/email/566\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":566,\"type\":\"email\",\"value\":\"seanlinmt@clearpixels.co.nz\",\"editedBy\":\"OWNER\",\"flags\":[]},{\"uri\":\"http://social.yahooapis.com/v1/user/ZL2QUUTEDI3T7OGTJJ6PHWOKEM/contact/560/name/564\",\"created\":\"2010-06-26T06:51:15Z\",\"updated\":\"2010-06-26T06:51:15Z\",\"id\":564,\"type\":\"name\",\"value\":{\"givenName\":\"Sean\",\"middleName\":\"\",\"familyName\":\"Lin\",\"prefix\":\"\",\"suffix\":\"\",\"givenNameSound\":\"\",\"familyNameSound\":\"\"},\"editedBy\":\"OWNER\",\"flags\":[]}],\"categories\":[]}]}}";

            var serializer = new JavaScriptSerializer();
            var contacts = serializer.Deserialize<ContactsResult>(jsonstring);
            return new EmptyResult();
        }

        public ActionResult ttt()
        {
            var rootdir = GeneralConstants.APP_ROOT_DIR + "/Content/templates/store/themes";
            var dir = new DirectoryInfo(rootdir);
            foreach (var themedir in dir.GetDirectories())
            {
                if (themedir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    continue;
                }
                
                var thumbnail = rootdir + themedir.FullName + "thumb.jpg";
            }

            return Content("done");
        }

        public ActionResult createalbum()
        {
            var oauth =
                repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.FACEBOOK);
            var fb = new FacebookService(oauth.token_key);
            var id = fb.Media.CreateAlbum("me", "test album", "album description");
            return Content(id.ToString());
        }

        public ActionResult TestPhoneNumber()
        {
            var sb = new StringBuilder();
            var util = PhoneNumberUtil.GetInstance();
            foreach (var usr in db.users)
            {
                if (string.IsNullOrEmpty(usr.phoneNumber))
                {
                    continue;
                }

                var country = usr.organisation1.country.ToCountry();
                if (string.IsNullOrEmpty(country.code))
                {
                    continue;
                }

                var parsed = util.Parse(usr.phoneNumber, country.code);
                sb.AppendFormat("+{0}.{1}{2}", parsed.CountryCode, parsed.NationalNumber, Environment.NewLine);
            }
            return Content(sb.ToString());
        }

        public ActionResult TestGBaseDataFeed()
        {
            var service = new AccountForShoppingService("tradelr");
            service.setUserCredentials("tradelr.com@gmail.com", "tuaki1976");

            var query = new AccountQuery(GeneralConstants.GBASE_MASTERACCOUNTID);
            var feed = service.Query(query);

            var entry = new AccountEntry();
            entry.Title.Text = "test shop";
            entry.Content.Content = "my first test shop";
            entry.AdultContent = true;
            entry.InternalId = "123456";
            entry.ReviewsUrl = "http://test.tradelr.com";

            var createdid = service.Insert(feed, entry);

            return Content(createdid);
        }

        public ActionResult TestNameSplitting()
        {
            string name = "George P Burdell";
            var names = Utility.SplitFullName(name);

            return Content(string.Join(", ", names));
        }

        public ActionResult GetEbaySiteDetails(SiteCodeType site)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);

            var service = new CategoryService(token.token_key);

            var resp = service.GetEbayDetails(site);
            
            return Content("done");
        }

        public ActionResult TestEbayCat(int id, string site)
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);

            var service = new CategoryService(token.token_key);
            var features = service.GetCategoryFeatures(id, site.ToEnum<SiteCodeType>());

            return Content("done");
        }

        public ActionResult TestEbayCategories()
        {
            var token = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.EBAY, true);

            var service = new CategoryService(token.token_key);

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

        public ActionResult ParseTrademeCategories()
        {
            var file = System.IO.File.OpenText("D:\\code\\tradelr\\bajula\\TradeMe\\xml\\Categories.json");
            var text = file.ReadToEnd();
            file.Close();

            var serializer = new JavaScriptSerializer();
            var cat = serializer.Deserialize<Category>(text);

            foreach (var entry in cat.Subcategories)
            {
                ParseCategory(entry, null);
            }

            return Content("done");
        }

        private void ParseCategory(Category entry, string parentid)
        {
            var segments = entry.Number.Split(new[] {'-'}, StringSplitOptions.RemoveEmptyEntries);

            var category = new trademe_category();
            category.name = entry.Name;
            category.id = segments.Last();
            category.parentid = !string.IsNullOrEmpty(parentid) ? parentid : category.id;
            if (entry.Subcategories == null || entry.Subcategories.Length == 0)
            {
                category.isLeaf = true;
            }
            else
            {
                category.isLeaf = false;
            }

            db.trademe_categories.InsertOnSubmit(category);
            db.SubmitChanges();

            if (!category.isLeaf)
            {
                foreach (var subcategory in entry.Subcategories)
                {
                    ParseCategory(subcategory, category.id);
                }
            }
        }

        public ActionResult GetTrademeListingDuration()
        {
            var oauth = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);

            var service = new CatalogueService(oauth.token_key, oauth.token_secret);

            foreach (var category in db.trademe_categories)
            {
                if (!category.isLeaf)
                {
                    continue;
                }

                var duration = service.GetCategoryDuration(new GetCategoryDurationRequest(category.id));
                if (duration.GetCategoryDurationResult.Durations == null)
                {
                    continue;
                }

                if (db.trademe_listingdurations.Any(x => x.categoryid == category.id))
                {
                    continue;
                }

                category.default_listingduration = (byte)duration.GetCategoryDurationResult.Default.ToInt();

                foreach (var entry in duration.GetCategoryDurationResult.Durations)
                {
                    var ld = new trademe_listingduration();
                    ld.categoryid = category.id;
                    ld.duration = (byte)entry.ToInt();
                    db.trademe_listingdurations.InsertOnSubmit(ld);
                }

                db.SubmitChanges();
            }

            return Content("done");
        }

        public ActionResult TestTrademe()
        {
            var file = System.IO.File.OpenRead("D:\\code\\tradelr\\bajula\\bajula\\Uploads\\trademephotoupload.txt");
            string xmlToSend = "";
            using (var sr = new StreamReader(file))
            {
                //xmlToSend = sr.ReadToEnd();
            }

            var oauth = repository.GetOAuthToken(subdomainid.Value, OAuthTokenType.TRADEME, true);


            var requestUrl = "https://api.tmsandbox.co.nz/v1/MyTradeMe/Watchlist/All.XML";

            var webRequest = WebRequest.Create(requestUrl) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ContentType = "text/xml";

            if (string.IsNullOrEmpty(xmlToSend))
            {
                webRequest.ContentLength = 0;
            }
            else
            {
                byte[] dataAsBytes = (new UTF8Encoding()).GetBytes(xmlToSend);
                webRequest.ContentLength = dataAsBytes.Length;

                using (var newStream = webRequest.GetRequestStream())
                {
                    // Send the data.
                    newStream.Write(dataAsBytes, 0, dataAsBytes.Length);
                    newStream.Close();
                }
            }

            var oauthheader = OAuthUtil.GenerateHeader(new Uri(requestUrl), OAuthClient.OAUTH_TRADEME_CONSUMER_KEY,
                                                       OAuthClient.OAUTH_TRADEME_CONSUMER_SECRET, oauth.token_key,
                                                       oauth.token_secret, webRequest.Method);
            webRequest.Headers.Add(oauthheader);

            WebResponse resp;
            try
            {
                resp = webRequest.GetResponse();

                var serializer = new XmlSerializer(typeof(PhotoResponse), "http://api.tmsandbox.co.nz/v1");
                var obj = (PhotoResponse)serializer.Deserialize(resp.GetResponseStream());

            }
            catch (WebException ex)
            {
                resp = ex.Response;
                if (resp != null)
                {
                    using (var sr = new StreamReader(resp.GetResponseStream()))
                    {
                        var resp_error = sr.ReadToEnd();
                        Syslog.Write(resp_error);
                    }
                }
            }

            return Content("done");
        }


        public void FindConflictingReferences()
        {
            var assemblies = GetAllAssemblies(@"C:\code\tradelr\bajula\bajula\bin");

            var references = GetReferencesFromAllAssemblies(assemblies);

            var groupsOfConflicts = FindReferencesWithTheSameShortNameButDiffererntFullNames(references);

            foreach (var group in groupsOfConflicts)
            {
                Console.Out.WriteLine("Possible conflicts for {0}:", group.Key);
                foreach (var reference in group)
                {
                    Console.Out.WriteLine("{0} references {1}",
                                          reference.Assembly.Name.PadRight(25),
                                          reference.ReferencedAssembly.FullName);
                }
            }
        }

        private IEnumerable<IGrouping<string, Reference>> FindReferencesWithTheSameShortNameButDiffererntFullNames(List<Reference> references)
        {
            return from reference in references
                   group reference by reference.ReferencedAssembly.Name
                       into referenceGroup
                       where referenceGroup.ToList().Select(reference => reference.ReferencedAssembly.FullName).Distinct().Count() > 1
                       select referenceGroup;
        }

        private List<Reference> GetReferencesFromAllAssemblies(List<Assembly> assemblies)
        {
            var references = new List<Reference>();
            foreach (var assembly in assemblies)
            {
                foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
                {
                    references.Add(new Reference
                    {
                        Assembly = assembly.GetName(),
                        ReferencedAssembly = referencedAssembly
                    });
                }
            }
            return references;
        }

        private List<Assembly> GetAllAssemblies(string path)
        {
            var files = new List<FileInfo>();
            var directoryToSearch = new DirectoryInfo(path);
            files.AddRange(directoryToSearch.GetFiles("*.dll", SearchOption.AllDirectories));
            files.AddRange(directoryToSearch.GetFiles("*.exe", SearchOption.AllDirectories));
            return files.ConvertAll(file => Assembly.LoadFile(file.FullName));
        }

        private class Reference
        {
            public AssemblyName Assembly { get; set; }
            public AssemblyName ReferencedAssembly { get; set; }
        }
#endif
    }
}
