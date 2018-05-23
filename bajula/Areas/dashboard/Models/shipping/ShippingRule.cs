using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shipwire;
using Shipwire.order;
using Shipwire.rate;
using tradelr.Areas.dashboard.Models.shipping.viewmodel;
using tradelr.Common.Models.currency;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Library.geo;
using clearpixels.Logging;
using tradelr.Models.products;
using tradelr.Models.store;
using Exception = System.Exception;

namespace tradelr.Areas.dashboard.Models.shipping
{
    public class ShippingRule
    {
        public long profileid { get; set; }
        public string id { get; set; } // can be shipwire 1D,2D or long from tradelrdb
        public string name { get; set; } // name of rule UPS, USPS, DHL
        public string currency { get; set; }  // used by shipwire
        public string cost { get; set; }
        public string deliveryEstimate { get; set; } // text of how long it will take to deliver
        public string country { get; set; }
        public int? countryid { get; set; }
        public string state { get; set; }
        public RuleType ruleType { get; set; }
        public string matchValue { get; set; } // match for values above this
    }

    public static class ShippingMethodHelper
    {
        public static Cost GetCost(this RateResponse response, string shippingMethod)
        {
            response.ValidateResponse();

            var order = response.Order[0];
            var quote = order.Quotes.SingleOrDefault(x => x.method == shippingMethod);
            if (quote == null)
            {
                Syslog.Write("Unable to find quote for method " + shippingMethod);
                return null;
            }

            return quote.Cost;
        }

        public static string GetShippingServiceName(this RateResponse response, string shippingMethod)
        {
            var order = response.Order[0];
            var quote = order.Quotes.SingleOrDefault(x => x.method == shippingMethod);
            if (quote == null)
            {
                throw new Exception("Unable to find quote for method " + shippingMethod);
            }

            return quote.Service;
        }

        public static IEnumerable<ShippingRule> ToShippingMethods(this List<CheckoutItem> checkoutItems,
            MASTERsubdomain sender, organisation receiver, shippingProfile[] shippingProfiles)
        {
            IEnumerable<ShippingRule> shippingMethods = Enumerable.Empty<ShippingRule>();
            if (shippingProfiles.Count() != 0 &&
                receiver.address1 != null &&
                    receiver.address1.country.HasValue)
            {
                // try get shipping rates)
                var shippingAddress = receiver.address1;
                if (shippingProfiles.UseShipwire())
                {
                    var aes = new AESCrypt();
                    var shipwireService = new ShipwireService(sender.shipwireEmail,
                                                              aes.Decrypt(sender.shipwirePassword, sender.id.ToString()));
                    
                    var address =
                        new AddressInfo(string.Format("{0} {1}", shippingAddress.first_name, shippingAddress.last_name),
                                        shippingAddress.street_address,
                                        shippingAddress.city, shippingAddress.state,
                                        Country.GetCountry(shippingAddress.country.Value).name,
                                        shippingAddress.postcode, "", "");

                    var shipwireOrder = new Order(address);
                    foreach (var entry in checkoutItems)
                    {
                        var item = new Shipwire.order.OrderItem
                        {
                            Sku = entry.SKU,
                            Quantity = entry.quantity
                        };
                        shipwireOrder.AddItem(item);
                    }

                    shipwireService.CreateRateRequest(shipwireOrder);

                    var rateResponse = shipwireService.SubmitRateRequest();
                    shippingMethods = rateResponse.ToModel(sender.organisation.MASTERsubdomain.currency.ToCurrency());
                }
                else
                {
                    shippingRule[] rules = null;
                    var statename = shippingAddress.state;
                    var countryid = shippingAddress.country.Value;
                    if (!string.IsNullOrEmpty(statename))
                    {
                        // try get state match
                        rules =
                            shippingProfiles.SelectMany(x => x.shippingRules).Where(
                                x => x.state == statename && x.country == countryid).ToArray();
                        // if none try get state-other match
                        if (rules.Count() == 0)
                        {
                            rules =
                                shippingProfiles.SelectMany(x => x.shippingRules).Where(
                                    x => x.state == "" && x.country == countryid).ToArray();
                        }
                    }

                    if (rules == null || rules.Count() == 0)
                    {
                        rules = shippingProfiles.SelectMany(x => x.shippingRules).Where(x => x.country == countryid).ToArray();
                        if (rules.Count() == 0)
                        {
                            rules = shippingProfiles.SelectMany(x => x.shippingRules).Where(x => !x.country.HasValue).ToArray();
                        }
                    }

                    // what type of rule
                    var rule = rules.FirstOrDefault();
                    if (rule != null)
                    {
                        var ruletype = (RuleType)rule.ruletype;
                        switch (ruletype)
                        {
                            case RuleType.PRICE:
                                decimal orderprice = 0;
                                foreach (var item in checkoutItems)
                                {
                                    var sellingprice = item.UnitPrice;
                                    orderprice += (sellingprice * item.quantity);
                                }
                                if (orderprice != 0)
                                {
                                    var keys =
                                        rules.Where(x => x.matchvalue < orderprice).GroupBy(x => x.name).Select(
                                            y => y.Key);
                                    var matchedrules = new List<ShippingRule>();
                                    foreach (var key in keys)
                                    {
                                        var result =
                                            rules.Where(x => x.matchvalue < orderprice && x.name == key).
                                                OrderByDescending(x => x.matchvalue).FirstOrDefault();
                                        if (result != null)
                                        {
                                            matchedrules.Add(result.ToModel());
                                        }
                                    }
                                    shippingMethods = matchedrules;
                                }
                                break;
                            case RuleType.WEIGHT:
                                decimal weight = 0;
                                
                                // only calculate weight if all items have weight dimensions
                                var incompletecount = checkoutItems.Where(x => x.dimension == null ||
                                                                  x.dimension.weight == 0).Count();
                                if (incompletecount == 0)
                                {
                                    foreach (var item in checkoutItems)
                                    {
                                        Debug.Assert(item.dimension != null && item.quantity != 0);
                                        weight += (item.dimension.weight * item.quantity);
                                    }
                                    // only match if weight has been entered
                                    if (weight != 0)
                                    {
                                        var keys =
                                            rules.Where(x => x.matchvalue < weight).GroupBy(x => x.name).Select(
                                                y => y.Key);
                                        var matchedrules = new List<ShippingRule>();
                                        foreach (var key in keys)
                                        {
                                            var result =
                                                rules.Where(x => x.matchvalue < weight && x.name == key).OrderByDescending(
                                                    x => x.matchvalue).FirstOrDefault();
                                            if (result != null)
                                            {
                                                matchedrules.Add(result.ToModel());
                                            }
                                        }
                                        shippingMethods = matchedrules;
                                    }
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                    }
                }
            }
            return shippingMethods;
        }

        public static ShippingRule ToModel(this shippingRule value)
        {
            return new ShippingRule()
            {
                cost = value.cost.ToString("n2"),
                id = value.name, // use name so we can use it to identify shipping method
                name = value.name
            };
        }

        public static List<ShippingRule> ToFlatRateModel(this IQueryable<shippingRule> methods, Currency currency, out string everywhereElseCost)
        {
            var ordered = methods.OrderBy(x => x.country);
            var rules = new List<ShippingRule>();
            everywhereElseCost = "";
            foreach (var entry in ordered)
            {
                // skip states and non zero match values
                if (!string.IsNullOrEmpty(entry.state) || entry.matchvalue != 0)
                {
                    continue;
                }
                
                // exclude everywhere else entry
                if (!entry.country.HasValue)
                {
                    everywhereElseCost = entry.cost.ToString("n" + currency.decimalCount);
                    continue;
                }

                var country = Country.GetCountry(entry.country.Value);
                var rule = new ShippingRule
                               {
                                   id = entry.id.ToString(),
                                   cost = entry.cost.ToString("n" + currency.decimalCount),
                                   country = country.name,
                                   countryid = entry.country.Value,
                                   currency = currency.code
                               };
                rules.Add(rule);
            }
            return rules;
        }

        public static List<ShippingGroup> ToModel(this IQueryable<shippingRule> rules, Currency currency, bool ismetric)
        {
            // sort first
            var ordered = rules.OrderBy(x => x.country).ThenBy(x => x.state);

            var groups = new List<ShippingGroup>();
            int? countryid = null;
            string statename = "";
            ShippingGroup countrygroup = null;
            ShippingGroup stategroup = null;
            foreach (var rule in ordered)
            {
                string countryname = "Everywhere Else";
                if (rule.country.HasValue)
                {
                    var topcountry = Country.GetCountry(rule.country.Value);
                    countryname = topcountry.name;
                }
                
                // if countryid is null then this is the first time
                // if not the same then is new country group
                if (!countryid.HasValue || 
                    rule.country != countryid.Value)
                {
                    /////////// new country
                    countrygroup = new ShippingGroup();
                    countrygroup.name = countryname;
                    groups.Add(countrygroup);

                    statename = "";
                }
                countryid = rule.country;

                if (string.IsNullOrEmpty(rule.state))
                {
                    ////////////// country level
                    var shippingRule = new ShippingRule
                                   {
                                       id = rule.id.ToString(),
                                       cost = rule.cost.ToString("n" + currency.decimalCount),
                                       countryid = rule.country,
                                       country = countryname,
                                       matchValue = rule.matchvalue.ToString("n2"),
                                       name = rule.name,
                                       ruleType = (RuleType) rule.ruletype
                                   };

                    shippingRule.matchValue = rule.matchvalue.ToMatchValueDisplay(shippingRule.ruleType, currency, ismetric);
                    countrygroup.AddRule(shippingRule);
                }
                else
                {
                    if (string.IsNullOrEmpty(statename) ||
                        rule.state != statename)
                    {
                        ////// new state
                        stategroup = new ShippingGroup();
                        stategroup.name = rule.state.ToStateName(countryid.Value.ToString());
                        statename = rule.state;
                        countrygroup.AddSubGroup(stategroup);
                    }
                    /////////// state level
                    var shippingRule = new ShippingRule
                                   {
                                       id = rule.id.ToString(),
                                       cost = rule.cost.ToString("n" + currency.decimalCount),
                                       countryid = rule.country,
                                       state = rule.state,
                                       matchValue = rule.matchvalue.ToString("n2"),
                                       name = rule.name,
                                       ruleType = (RuleType) rule.ruletype
                                   };
                    shippingRule.country = "Everywhere Else";
                    if (rule.country.HasValue)
                    {
                        shippingRule.country = Country.GetCountry(rule.country.Value).name;
                    }
                    shippingRule.matchValue = rule.matchvalue.ToMatchValueDisplay(shippingRule.ruleType, currency, ismetric);
                    stategroup.AddRule(shippingRule);
                }
            }
            return groups;
        }

        public static IEnumerable<ShippingRule> ToModel(this RateResponse response, Currency currency)
        {
            response.ValidateResponse();

            var rules = new List<ShippingRule>();
            
            var order = response.Order[0];
            foreach (var quote in order.Quotes)
            {
                var method = new ShippingRule
                                 {
                                     id = quote.method,
                                     name = quote.Service,
                                     cost = quote.Cost.value.ToString(),
                                     currency = quote.Cost.currency
                                 };
                if (currency.code != "USD")
                {
                    method.cost =
                        CurrencyConverter.Instance.Convert("USD", currency.code, quote.Cost.value).ToString("n" +
                                                                                                            currency.
                                                                                                                decimalCount);
                }
                
                var min = quote.DeliveryEstimate.Minimum.value;
                var max = quote.DeliveryEstimate.Maximum.value;
                var timeunit = quote.DeliveryEstimate.Minimum.units;
                if (timeunit == "days")
                {
                    timeunit = "day";
                }
                else
                {
                    Syslog.Write(string.Format("Unknown timeunit: {0}", timeunit));
                }

                if (min == max)
                {
                    if (min == 1)
                    {
                        method.deliveryEstimate = string.Format("1 {0}", timeunit.Substring(0,timeunit.Length));
                    }
                    else
                    {
                        method.deliveryEstimate = string.Format("{0} {1}s", min, timeunit);
                    }
                }
                else
                {
                    method.deliveryEstimate = string.Format("{0} to {1} {2}s", min, max, timeunit);
                }

                rules.Add(method);
            }
            return rules;
        }

        private static string ToMatchValueDisplay(this decimal value, RuleType type, Currency currency, bool ismetric, bool showSymbols = true)
        {
            if (type == RuleType.PRICE)
            {
                if (showSymbols)
                {
                    return currency.symbol + value.ToString("n" + currency.decimalCount);
                }
                return value.ToString("n" + currency.decimalCount);
            }

            if (ismetric)
            {
                if (showSymbols)
                {
                    return value.ToString("n2") + "kg";
                }
                return value.ToString("n2");
            }

            if (showSymbols)
            {
                return value.ConvertWeight(false).ToString("n2") + "lb";
            }
            return value.ConvertWeight(false).ToString("n2");
        }

        public static ShippingRuleViewModel ToModel(this shippingRule rule, Currency currency, bool ismetric, bool showSymbols = true)
        {
            var viewData = new ShippingRuleViewModel()
                       {
                           id = rule.id.ToString(),
                           cost = rule.cost.ToString("n" + currency.decimalCount),
                           countryid = rule.country,
                           state = rule.state.ToStateName(rule.country.ToString()),
                           name = rule.name,
                           ruleType = (RuleType)rule.ruletype,
                           isMetric = ismetric,
                           currency = currency.symbol
                       };
            
            viewData.country = rule.country.HasValue ? 
                                            Country.GetCountry(rule.country.Value).name : "Everywhere Else";
            viewData.matchValue = rule.matchvalue.ToMatchValueDisplay(viewData.ruleType, currency, ismetric, showSymbols);
            
            return viewData;
        }
    }
}
