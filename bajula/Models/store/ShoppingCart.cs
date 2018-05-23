using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using Shipwire;
using Shipwire.order;
using tradelr.Areas.dashboard.Models.shipping;
using tradelr.Common.Models.currency;
using tradelr.Crypto;
using tradelr.DBML;
using tradelr.Library.geo;
using tradelr.Models.liquid.models.Product;
using tradelr.Models.products;

namespace tradelr.Models.store
{
    public class ShoppingCart
    {
        public decimal shippingCost { get; set; }
        public string shippingMethod { get; set; }
        public string shipwireShippingName { get; set; }
        public string currencyCode { get; set; }
        public List<CheckoutItem> items { get; set; }
        private ITradelrRepository repository { get; set; }

        public ShoppingCart(string currencycode)
        {
            this.currencyCode = currencycode;
            repository = new TradelrRepository(new tradelrDataContext());
            items = new List<CheckoutItem>();
        }

        public bool CalculateShippingCost(IQueryable<product_variant> products, MASTERsubdomain sender, user receiver)
        {
            var shippingProfiles = products.Select(x => x.product.shippingProfile);
            var shippingAddress = receiver.organisation1.address1;
            if (shippingProfiles.UseShipwire())
            {
                var aes = new AESCrypt();
                var shipwireService = new ShipwireService(sender.shipwireEmail,
                                                          aes.Decrypt(sender.shipwirePassword, sender.id.ToString()));
                var address = new AddressInfo(string.Format("{0} {1}", shippingAddress.first_name, shippingAddress.last_name), shippingAddress.street_address,
                                              shippingAddress.city, shippingAddress.state,
                                              Country.GetCountry(shippingAddress.country.Value).name,
                                              shippingAddress.postcode, "", receiver.email);

                var shipwireOrder = new Order(address);
                foreach (var entry in products)
                {
                    var quantity = items.Single(x => x.id == entry.id).quantity;
                    var item = new OrderItem
                    {
                        Sku = entry.sku,
                        Quantity = quantity
                    };
                    shipwireOrder.AddItem(item);
                }

                shipwireService.CreateRateRequest(shipwireOrder);

                var rateResponse = shipwireService.SubmitRateRequest();
                var quote = rateResponse.GetCost(shippingMethod);
                
                if (quote == null)
                {
                    // failed to obtain shipping cost
                    return false;
                }
                if (currencyCode != "USD")
                {
                    shippingCost = CurrencyConverter.Instance.Convert("USD", currencyCode, quote.value);
                }
                else
                {
                    shippingCost = quote.value;
                }

                shipwireShippingName = rateResponse.GetShippingServiceName(shippingMethod);
            }
            else
            {
                IQueryable<shippingRule> rules = null;
                var statename = shippingAddress.state;
                var countryid = shippingAddress.country.Value;
                if (!string.IsNullOrEmpty(statename))
                {
                    // try get state match
                    rules =
                        shippingProfiles.SelectMany(x => x.shippingRules).Where(
                            x => x.state == statename && x.country == countryid);
                    // if none try get state-other match
                    if (!rules.Any())
                    {
                        rules =
                            shippingProfiles.SelectMany(x => x.shippingRules).Where(
                                x => x.state == "" && x.country == countryid);
                    }
                }

                if (rules == null || !rules.Any())
                {
                    rules = shippingProfiles.SelectMany(x => x.shippingRules).Where(x => x.country == countryid);
                    if (!rules.Any())
                    {
                        // look for any
                        rules = shippingProfiles.SelectMany(x => x.shippingRules).Where(x => !x.country.HasValue);
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
                            foreach (var p in products)
                            {
                                var quantity = items.Single(x => x.id == p.id).quantity;
                                var price = p.product.ToUserPrice(receiver.id);
                                if (price.HasValue)
                                {
                                    orderprice += (price.Value * quantity);
                                }
                            }
                            if (orderprice != 0)
                            {
                                var matchedRule =
                                    rules.Where(x => x.matchvalue < orderprice && x.name == shippingMethod).
                                        OrderByDescending(x => x.matchvalue).
                                        FirstOrDefault();
                                if (matchedRule == null)
                                {
                                    return false;
                                }
                                shippingCost = matchedRule.cost;
                            }
                            break;
                        case RuleType.WEIGHT:
                            decimal weight = 0;
                            var serializer = new JavaScriptSerializer();
                            var incompletecount = products.Count(x => x.product.dimensions == null);
                            if (incompletecount == 0)
                            {
                                foreach (var p in products)
                                {
                                    var dimension = serializer.Deserialize<Dimension>(p.product.dimensions);
                                    var quantity = items.Single(x => x.id == p.id).quantity;
                                    Debug.Assert(dimension != null && quantity != 0);
                                    if (dimension.weight == 0)
                                    {
                                        return false;
                                    }
                                    weight += (dimension.weight * quantity);
                                }

                                // only match if weight has been entered
                                if (weight != 0)
                                {
                                    var matchedRule = rules.Where(x => x.matchvalue < weight && x.name == shippingMethod).OrderByDescending(x => x.matchvalue).FirstOrDefault();
                                    if (matchedRule == null)
                                    {
                                        return false;
                                    }
                                    shippingCost = matchedRule.cost;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                }
                else
                {
                    // unable to find matching rule
                    return false;
                }
            }
            return true;
        }
    }
}