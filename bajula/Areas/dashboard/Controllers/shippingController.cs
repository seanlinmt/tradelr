using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Ebay;
using eBay.Service.Core.Soap;
using tradelr.Areas.dashboard.Models.shipping;
using tradelr.Areas.dashboard.Models.shipping.viewmodel;
using tradelr.Common.Models.currency;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.JSON;
using tradelr.Models.products;
using tradelr.Models.shipping;
using tradelr.Models.store;
using tradelr.Models.transactions;
using tradelr.Models.users;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [RoleFilter(role = UserRole.USER)]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [TradelrHttps]
    public class shippingController : baseController
    {
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add(long id)
        {
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var settings = (UserSettings) usr.settings;
            var viewdata = new ShippingRuleViewModel
                               {
                                   isMetric = settings.HasFlag(UserSettings.METRIC_VIEW),
                                   currency = usr.organisation1.MASTERsubdomain.currency.ToCurrencyCode(),
                                   profileid = id
                               };

            return View(viewdata);
        }

        [HttpGet]
        public ActionResult AddProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(long profileid, decimal cost, int? country, decimal matchvalue, bool metric, string name, string rule_type,
            string states_other, string states_us, string states_canadian)
        {

            var rule = new shippingRule
                           {
                               country = country, 
                               cost = cost, 
                               secondaryCost = cost,
                               matchvalue = matchvalue, 
                               name = name,
                               profileid = profileid
                           };

            switch (country)
            {
                case 185: // USA
                    rule.state = states_us;
                    break;
                case 32: // Canada
                    rule.state = states_canadian;
                    break;
                default:
                    rule.state = states_other;
                    break;
            }

            switch (rule_type)
            {
                case "weight":
                    if (!metric)
                    {
                        rule.matchvalue = matchvalue.ConvertWeight(true);
                    }
                    else
                    {
                        rule.matchvalue = matchvalue;
                    }
                    rule.ruletype = (byte)RuleType.WEIGHT;
                    break;
                case "price":
                    rule.matchvalue = matchvalue;
                    rule.ruletype = (byte) RuleType.PRICE;
                    break;
                default:
                    return Json("Invalid rule".ToJsonFail());
            }

            try
            {
                // check if entry already exist
                if (repository.ExistShippingRule(rule))
                {
                    return Json("Rule already exist".ToJsonFail());
                }
                
                // check that the rule we're adding is the same for the current destination
                if (repository.ExistDifferentRuleType(rule))
                {
                    return Json("Different rule types for the same destination are not allowed".ToJsonFail());
                }

                repository.AddShippingRule(rule);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var settings = (UserSettings) usr.settings;
            var currency = usr.organisation1.MASTERsubdomain.currency.ToCurrency();

            var viewdata = rule.ToModel(currency,settings.HasFlag(UserSettings.METRIC_VIEW));

            return Json(viewdata.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult CreateProfile(string profileName)
        {
            try
            {
                var profile = new shippingProfile()
                {
                    subdomainid = subdomainid.Value,
                    title = profileName,
                    type = ShippingProfileType.FLATRATE.ToString(),
                    permanent = false
                };

                repository.AddShippingProfile(profile);

                return Json(new
                                {
                                    profile.id,
                                    profile.title
                                }.ToJsonOKData());
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
        }

        [HttpPost]
        public ActionResult Delete(long id, long profileid)
        {
            try
            {
                repository.DeleteShippingRule(id, profileid);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult DeleteProfile(long id)
        {
            var profile = MASTERdomain.shippingProfiles.Single(x => x.id == id);
            if (profile.permanent)
            {
                return Json("Default profile cannot be deleted".ToJsonFail());
            }

            try
            {
                repository.DeleteShippingProfile(id, subdomainid.Value);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult EbayProfile(long id)
        {
            var profile = MASTERdomain.ebay_shippingprofiles.SingleOrDefault(x => x.id == id);

            if (profile == null)
            {
                return Json("Shipping profile not found".ToJsonFail());
            }

            var viewmodel = profile.ToModel();

            var view = this.RenderViewToString("EbayProfile", viewmodel);

            return Json(view.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult EbayProfileAdd()
        {
            var viewmodel = EbayService.SupportedSites.Select(x => new SelectListItem()
                                                                       {
                                                                           Text = x.ToString(),
                                                                           Value = x.ToString()
                                                                       });

            return View("ebay_addProfile", viewmodel);
        }

        [HttpPost]
        public ActionResult EbayProfileCreate(string profileName, string siteid)
        {
            var profile = new ebay_shippingprofile();
            profile.title = profileName;
            profile.siteid = siteid;

            MASTERdomain.ebay_shippingprofiles.Add(profile);

            repository.Save("EbayProfileCreate");

            return Json(new {text = profile.title, value = profile.id}.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult EbayProfileDelete(long id)
        {
            var profile = MASTERdomain.ebay_shippingprofiles.SingleOrDefault(x => x.id == id);

            if (profile == null)
            {
                return SendJsonErrorResponse("Shipping profile not found");
            }

            db.ebay_shippingrules.DeleteAllOnSubmit(profile.ebay_shippingrules);


            return Json("Profile deleted successfully".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult EbayServices(bool isInternational, string siteid)
        {
            var services = db.ebay_shippingservices
                .Where(x => x.isInternational == isInternational && 
                    x.siteid == siteid && 
                    !x.requiresDimension &&
                    !x.requiresWeight);

            if (!services.Any())
            {
                return Json("No available shipping service found".ToJsonFail());
            }

            var viewmodel = new EbayServicesEditViewModel()
                                {
                                    servicesList = services
                                        .OrderBy(x => x.description)
                                        .Select(
                                            x => new SelectListItem() {Text = x.description, Value = x.id.ToString()}),
                                    locationList =
                                        db.ebay_shippinglocations
                                        .OrderBy(x => x.description)
                                        .Where(x => x.siteid == siteid).Select(x => new SelectListItem() { Text = x.description, Value = x.location}),
                                    isInternational = isInternational
                                };

            var view = this.RenderViewToString("EbayServicesEditRow", viewmodel);

            return Json(view.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult EbayRulesDelete(long id)
        {
            var rule = MASTERdomain.ebay_shippingprofiles.SelectMany(x => x.ebay_shippingrules).SingleOrDefault(y => y.id == id);
            if (rule != null)
            {
                if (rule.ebay_shippingrule_locations.Count != 0)
                {
                    db.ebay_shippingrule_locations.DeleteAllOnSubmit(rule.ebay_shippingrule_locations);
                }
                db.ebay_shippingrules.DeleteOnSubmit(rule);
                repository.Save("EbayRulesDelete");
            }
            return Json("".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult EbayServicesSave(long profileid, int serviceid, decimal cost, string destinations)
        {
            var service = db.ebay_shippingservices.SingleOrDefault(x => x.id == serviceid);

            if (service == null)
            {
                return SendJsonErrorResponse("Shipping service is invalid");
            }

            var profile = MASTERdomain.ebay_shippingprofiles.SingleOrDefault(x => x.id == profileid);

            if (profile == null)
            {
                return SendJsonErrorResponse("Shipping profile is invalid");
            }

            // check if already exist
            if (profile.ebay_shippingrules.Any(x => x.ebay_shippingservice.isInternational == service.isInternational &&
                                                        x.serviceid == serviceid))
            {
                return SendJsonErrorResponse("Shipping service already exists");
            }

            // check limits
            if (service.isInternational)
            {
                if (profile.ebay_shippingrules.Count(x => x.ebay_shippingservice.isInternational) == 5)
                {
                    return SendJsonErrorResponse("Maximum of 5 shipping services allowed");
                }
            }
            else
            {
                if (profile.ebay_shippingrules.Count(x => !x.ebay_shippingservice.isInternational) == 5)
                {
                    return SendJsonErrorResponse("Maximum of 5 shipping services allowed");
                }
            }

            // create rule
            var rule = new ebay_shippingrule
                           {
                               cost = cost, 
                               serviceid = serviceid
                           };

            profile.ebay_shippingrules.Add(rule);

            // create shiptodestination entries
            if (service.isInternational)
            {
                string[] destArray;
                if (string.IsNullOrEmpty(destinations))
                {
                    destArray = new[] { "Worldwide" };
                }
                else
                {
                    destArray = destinations.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                }

                var hashset = new HashSet<string>(destArray);

                foreach (var entry in hashset)
                {
                    var loc = db.ebay_shippinglocations.SingleOrDefault(x => x.location == entry && 
                                                                            x.siteid == profile.siteid);
                    if (loc != null)
                    {
                        var dest = new ebay_shippingrule_location();
                        dest.description = loc.description;
                        dest.location = loc.location;
                        rule.ebay_shippingrule_locations.Add(dest);
                    }
                }
            }

            // save
            repository.Save("EbayServicesSave");

            var currency = MASTERdomain.currency.ToCurrency();

            var viewmodel = rule.ToModel(currency);

            var view = this.RenderViewToString("EbayServicesSingleRow", viewmodel);

            return Json(view.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult EbaySiteProfile(string id)
        {
            var profiles = MASTERdomain.ebay_shippingprofiles.Where(x => x.siteid == id)
                .Select(x => new {name = x.title, value = x.id});

            return Json(profiles.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var rule = repository.GetShippingRule(id).SingleOrDefault();

            if (rule == null)
            {
                return SendJsonErrorResponse("Unable to find rule");
            }

            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var settings = (UserSettings)usr.settings;

            var viewdata = rule.ToModel(usr.organisation1.MASTERsubdomain.currency.ToCurrency(), settings.HasFlag(UserSettings.METRIC_VIEW), false);

            return View("Add", viewdata);
        }

        [PermissionFilter(permission = UserPermission.NONE)]
        public ActionResult HaveAddress()
        {
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            if (string.IsNullOrEmpty(usr.organisation1.address) || !usr.organisation1.country.HasValue)
            {
                return Json(false.ToJsonOKData());
            }
            return Json(true.ToJsonOKData());
        }

        [NoCache]
        [HttpGet]
        public ActionResult Index()
        {
            var viewdata = new ShippingViewModel(baseviewmodel)
                               {
                                   shippingProfiles = MASTERdomain.ToShippingProfiles(),
                                   ebay_sites = new[] {new SelectListItem() {Text = "Select site", Value = ""}}
                                       .Union(EbayService.SupportedSites.Select(x => new SelectListItem()
                                                                                         {
                                                                                             Text = x.ToString(),
                                                                                             Value = x.ToString()
                                                                                         }))
                               };
            return View(viewdata);
        }

        [JsonFilter(Param = "items", RootType = typeof(OrderItem[]))]
        [HttpPost]
        public ActionResult List(IEnumerable<OrderItem> items)
        {
            var receiver = repository.GetUserById(sessionid.Value, subdomainid.Value);

            // need to get dimensions
            var variants = repository.GetProductVariants(subdomainid.Value).Where(x => items.Select(y => y.id).Contains(x.id));

            // we have to create new checkoutItem list because we need to get dimension info from database
            var checkoutItems = new List<CheckoutItem>();

            // uses the first shippingProfile that we come across
            foreach (var product in variants)
            {
                var quantity = items.Single(x => x.id == product.id).quantity;
                var checkOutItem = product.ToCheckoutItem(quantity, sessionid);
                checkoutItems.Add(checkOutItem);
            }
            var shippingProfiles = variants.Select(x => x.product.shippingProfile).ToArray();
            var shippingMethods = checkoutItems.ToShippingMethods(receiver.organisation1.MASTERsubdomain,
                                                                  receiver.organisation1,
                                                                  shippingProfiles);
            return Json(shippingMethods.ToJsonOKData());
        }

        [HttpGet]
        public ActionResult Profile(long id)
        {
            var profile = repository.GetShippingProfiles(subdomainid.Value).Single(x => x.id == id);
            var sd = profile.MASTERsubdomain;
            var usr = repository.GetUserById(sessionid.Value, subdomainid.Value);
            var settings = (UserSettings)usr.settings;
            var viewdata = new ShippingProfile()
                              {
                                  currency = sd.currency.ToCurrency(),
                                  shippingGroups =
                                      repository.GetShippingRules(id, subdomainid.Value).ToModel(
                                          sd.currency.ToCurrency(), settings.HasFlag(UserSettings.METRIC_VIEW)),
                                  id = profile.id,
                                  type = profile.type.ToEnum<ShippingProfileType>(),
                                  shipwireEnabled = !string.IsNullOrEmpty(profile.MASTERsubdomain.shipwireEmail),
                                  IsPermanent = profile.permanent
                              };

            // set everywhere else cost
            string everywhereElseCost;
            viewdata.flatrateRules =
                repository.GetShippingRules(id, subdomainid.Value).ToFlatRateModel(
                    sd.currency.ToCurrency(), out everywhereElseCost);
            viewdata.everywhereElseCost = everywhereElseCost;
            if (!string.IsNullOrEmpty(viewdata.everywhereElseCost))
            {
                viewdata.applyEverywhereElseCost = true;
            }
            return View(viewdata);
        }

        [HttpPost]
        public ActionResult ProfileType(long id, ShippingProfileType type)
        {
            var profile = repository.GetShippingProfiles(subdomainid.Value).SingleOrDefault(x => x.id == id);
            profile.type = type.ToString();
            repository.Save();
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult Settings(decimal?[] shipping_cost, int?[] shipping_destination, long? shippingProfileID,
            ShippingProfileType shippingType)
        {
            if (shippingProfileID.HasValue)
            {
                // delete existing rules in profile
                if (shippingType == ShippingProfileType.FLATRATE)
                {
                    ShippingProfile.UpdateFlatrateShipping(shippingProfileID.Value, shipping_cost, shipping_destination, subdomainid.Value);
                }
            }
            
            return Json(OPERATION_SUCCESSFUL.ToJsonOKData());
        }

        [HttpPost]
        public ActionResult Update(long ruleid, string cost, decimal matchvalue, bool metric, string name)
        {
            var rule = repository.GetShippingRule(ruleid).SingleOrDefault();
            rule.cost = Decimal.Parse(cost,
                                        NumberStyles.AllowDecimalPoint |
                                        NumberStyles.AllowThousands);
            rule.matchvalue = matchvalue;
            rule.name = name;
            var ruletype = (RuleType) rule.ruletype;

            switch (ruletype)
            {
                case RuleType.WEIGHT:
                    if (!metric)
                    {
                        rule.matchvalue = matchvalue.ConvertWeight(true);
                    }
                    else
                    {
                        rule.matchvalue = matchvalue;
                    }
                    break;
                case RuleType.PRICE:
                    rule.matchvalue = matchvalue;
                    break;
                default:
                    return Json("Invalid rule".ToJsonFail());
            }

            try
            {
                repository.Save();
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }

            return Json("".ToJsonOKMessage());
        }

    }
}
