using System;
using System.Collections.Generic;
using System.Linq;
using Ebay;
using Ebay.Enums;
using Ebay.Resources;
using eBay.Service.Core.Soap;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.geo;
using tradelr.Library.payment;
using clearpixels.Logging;
using tradelr.Models.products;

namespace tradelr.Models.export.ebay
{
    public class EbayExporter : ExportItem
    {
        private ebay_product ebayProduct { get; set; }
        private MASTERsubdomain sd { get; set; }

        private string token { get; set; }
        private string hostname { get; set; }

        private ItemService service { get; set; }

        private SiteCodeType ebay_site { get; set; }

        // payment
        private string PaypalID { get; set; }

        // shipping 
        private ShippingServiceOptionsTypeCollection shipping_domestic { get; set; }
        private InternationalShippingServiceOptionsTypeCollection shipping_international { get; set; }

        public double listingFees { get; set; }

        private EbayExporter(string hostname) : base(hostname)
        {
            this.hostname = hostname;
            ebayProduct = new ebay_product();
            shipping_domestic = new ShippingServiceOptionsTypeCollection();
            shipping_international = new InternationalShippingServiceOptionsTypeCollection();
        }

        public EbayExporter(SiteCodeType site, string hostname, string token, MASTERsubdomain sd) : this(hostname)
        {
            this.token = token;
            service = new ItemService(token);
            this.sd = sd;
            this.ebay_site = site;
        }

        public void BuildItem(ebay_product ep)
        {
            BuildItem(ep.products.First(), 
                ep.categoryid, 
                ep.quantity, 
                ep.condition,
                ep.returnPolicy.ToEnum<ReturnsAccepted>(),
                ep.duration, 
                ep.refundPolicy,
                ep.returnWithin,
                ep.includeAddress,
                ep.profileid.Value,
                ep.dispatchTime,
                ep.autorelist,
                Listing.GetTradelrSupportedType(ep.listingType.ToEnum<ListingTypeCodeType>()),
                ep.startPrice,
                ep.buynowPrice,
                ep.reservePrice);
        }

        // startprice is only for auction listings so it will be null for fixedprice items
        public void BuildItem(product p, int categoryid, int quantity, int? condition, ReturnsAccepted returns,
            string duration, string refund_policy, string return_within, bool ebay_includeAddress, long shippingprofile,
            int dispatch_time, bool autorelist, ListingType listingType, decimal? startPrice, decimal? buynowPrice, decimal? reservePrice)
        {
            InitValues(p, 1, sd);

            ebayProduct.categoryid = categoryid;
            ebayProduct.quantity = quantity;
            ebayProduct.condition = condition;
            ebayProduct.returnPolicy = returns.ToString();
            ebayProduct.duration = duration;
            ebayProduct.refundPolicy = refund_policy;
            ebayProduct.returnWithin = return_within;
            ebayProduct.includeAddress = ebay_includeAddress;
            ebayProduct.profileid = shippingprofile;
            ebayProduct.dispatchTime = dispatch_time;
            ebayProduct.listingType = listingType.ToString();
            ebayProduct.autorelist = autorelist;
            ebayProduct.startPrice = startPrice;
            ebayProduct.buynowPrice = buynowPrice;
            ebayProduct.reservePrice = reservePrice;

            service.BuildItem(ebay_site,
                              listingType.ToString().ToEnum<ListingTypeCodeType>(),
                              productname,
                              Description,
                              Currency.code.ToEnum<CurrencyCodeType>(),
                              buynowPrice.HasValue ? (double) buynowPrice.Value : (double?) null,
                              (double) (startPrice.HasValue ? startPrice.Value : SellingPrice),
                              reservePrice.HasValue ? (double) reservePrice.Value : (double?) null,
                              Country.name,
                              Country.code.ToEnum<CountryCodeType>(),
                              categoryid,
                              quantity,
                              condition,
                              returns,
                              duration,
                              refund_policy,
                              return_within,
                              dispatch_time,
                              sd.GetPaypalID());

            PaypalID = sd.GetPaypalID();

            // add address
            if (ebay_includeAddress)
            {
                var o = sd.organisation;
                service.AddSellerContactDetails(o.city.HasValue ? o.MASTERcity.name : "",
                    o.name,
                    Country.GetCountry(o.country.Value).code.ToEnum<CountryCodeType>(),
                    o.address,
                    o.phone,
                    o.postcode,
                    o.state.ToStateName(o.country.HasValue ? o.country.Value.ToString() : ""));
            }

            // add photos
            AddProductPhotos(p);

            // add variants (if any)
            AddProductVariants(p);

            // add shipping methods
            AddShippingDetails(p, shippingprofile);

            // add payment methods
            foreach (var entry in sd.paymentMethods)
            {
                var method = entry.method.ToEnum<PaymentMethod>();
                AddPaymentOptions(method);
            }
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/types/BuyerPaymentMethodCodeType.html
        private void AddPaymentOptions(PaymentMethod method)
        {
            // only paypal is accepted 
            switch (method)
            {
                case PaymentMethod.Paypal:
                    service.AddPaymentOption(BuyerPaymentMethodCodeType.PayPal, PaypalID);
                    break;
                default:
                    break;
            }
        }

        // todo: currently ebay will only recognise 1 photo. Will need to use themes if more than 1 photo is to be uploaded
        private void AddProductPhotos(product p)
        {
            service.AddPhotos(p.product_images.Select(x => hostName.ToDomainUrl(x.url, true)).ToArray());
        }

        private void AddProductVariants(product p)
        {
            // only add if there's more than 1 variant
            if (p.product_variants.Count > 1)
            {
                foreach (var entry in p.product_variants)
                {
                    var variant = new VariationType
                                      {
                                          SKU = entry.sku, 
                                          Quantity = ebayProduct.quantity
                                      };

                    var variants = new NameValueListTypeCollection();

                    if (!string.IsNullOrEmpty(entry.color))
                    {
                        variants.Add(new NameValueListType()
                                         {
                                             Name = "color",
                                             Value = new StringCollection(new[] {entry.color})
                                         });
                    }

                    if (!string.IsNullOrEmpty(entry.size))
                    {
                        variants.Add(new NameValueListType()
                        {
                            Name = "size",
                            Value = new StringCollection(new[] { entry.size })
                        });
                    }

                    variant.VariationSpecifics = variants;
                    service.AddProductVariant(variant);
                }
            }
        }

        private void AddShippingDetails(product p, long shipping_profileid)
        {
            // currently only support flat rate
            using (var repository = new TradelrRepository())
            {
                var profile = repository.GetEbayShippingProfile(shipping_profileid);
                if (profile != null)
                {
                    PopulateFlatRateShippingOptions(profile.ebay_shippingrules);
                }
            }

            service.AddShippingDetails(ShippingTypeCodeType.Flat, shipping_domestic, shipping_international);
        }

        private MeasureType GetEbayDistance(decimal value)
        {
            switch (ebay_site)
            {
                case SiteCodeType.US:
                    return new MeasureType()
                    {
                        measurementSystem = MeasurementSystemCodeType.English,
                        unit = "in",
                        Value = value.ConvertDistance(false)
                    };
                    break;
                case SiteCodeType.Malaysia:
                    return new MeasureType()
                    {
                        measurementSystem = MeasurementSystemCodeType.Metric,
                        unit = "cm",
                        Value = value
                    };
                    break;
                default:
                    throw new ArgumentException();
                    break;
            }
        }

        private MeasureType GetEbayWeight(decimal value)
        {
            switch (ebay_site)
            {
                case SiteCodeType.US:
                    return new MeasureType()
                    {
                        measurementSystem = MeasurementSystemCodeType.English,
                        unit = "lb",
                        Value = value.ConvertWeight(false)
                    };
                    break;
                case SiteCodeType.Malaysia:
                    return new MeasureType()
                    {
                        measurementSystem = MeasurementSystemCodeType.Metric,
                        unit = "kg",
                        Value = value
                    };
                    break;
                default:
                    throw new ArgumentException();
                    break;
            }
        }

        // get target ebay site, international if shipping rule country is outside target ebay site
        private void PopulateFlatRateShippingOptions(IEnumerable<ebay_shippingrule> rows)
        {
            // convert local to ebay currency enum
            var ebayCurrency = Currency.code.ToEnum<CurrencyCodeType>();

            // shipping options
            int domesticPriority = 1;
            int internationalPriority = 1;
            foreach (var row in rows)
            {
                if (row.ebay_shippingservice.isInternational)
                {
                    var shippingOption = new InternationalShippingServiceOptionsType();
                    shippingOption.ShippingService = row.ebay_shippingservice.servicetype;
                    shippingOption.ShippingServicePriority = internationalPriority++;

                    // shipping cost
                    shippingOption.ShippingServiceCost = new AmountType()
                    {
                        currencyID = ebayCurrency,
                        Value = (double)row.cost
                    };

                    shippingOption.ShipToLocation = new StringCollection(row.ebay_shippingrule_locations.Select(x => x.location).ToArray());

                    shipping_international.Add(shippingOption);
                }
                else
                {
                    var shippingOption = new ShippingServiceOptionsType();
                    shippingOption.ShippingService = row.ebay_shippingservice.servicetype;
                    shippingOption.ShippingServicePriority = domesticPriority++;

                    if (row.cost == 0)
                    {
                        shippingOption.FreeShipping = true;
                    }
                    else
                    {
                        // shipping cost
                        shippingOption.ShippingServiceCost = new AmountType()
                        {
                            currencyID = ebayCurrency,
                            Value = (double)row.cost
                        };
                    }
                    
                    shipping_domestic.Add(shippingOption);
                }
            }
        }

        // todo: need to support this
        private void PopulateCalculatedShippingOptions(IEnumerable<shippingRule> rows)
        {
            // convert local to ebay currency enum
            var ebayCurrency = Currency.code.ToEnum<CurrencyCodeType>();

            // shipping options
            foreach (var entry in rows)
            {
                var ruleCountry = entry.country.ToCountry();

                // international or domestic
                bool isInternational = true;
                switch (ebay_site)
                {
                    case SiteCodeType.US:
                        if (ruleCountry.code == "US")
                        {
                            isInternational = false;
                        }
                        break;
                    case SiteCodeType.Malaysia:
                        if (ruleCountry.code == "MY")
                        {
                            isInternational = false;
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }

                var rule = entry;

                if (isInternational)
                {
                    var shippingOption = new InternationalShippingServiceOptionsType();
                    shippingOption.ShippingService = rule.name;
                    shippingOption.ShipToLocation = new StringCollection(new[] { ruleCountry.code });

                    // shipping cost
                    shippingOption.ShippingServiceCost = new AmountType()
                    {
                        currencyID = ebayCurrency,
                        Value = (double)rule.cost
                    };

                    shippingOption.ShippingServicePriority = 1;

                    shipping_international.Add(shippingOption);
                }
                else
                {
                    var shippingOption = new ShippingServiceOptionsType();
                    shippingOption.ShippingService = rule.name;

                    if (rule.cost == 0)
                    {
                        shippingOption.FreeShipping = true;
                    }
                    else
                    {
                        // shipping cost
                        shippingOption.ShippingServiceCost = new AmountType()
                        {
                            currencyID = ebayCurrency,
                            Value = (double)rule.cost
                        };
                    }

                    shippingOption.ShippingServicePriority = 1;
                    shipping_domestic.Add(shippingOption);
                }
            }
        }

        public void AddEbayItem()
        {
            switch (ebayProduct.listingType.ToEnum<ListingType>())
            {
                case ListingType.Chinese:
                    AddAuctionItem();
                    break;
                case ListingType.FixedPriceItem:
                    AddFixedPriceItem();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AddFixedPriceItem()
        {
            var call = service.AddFixedPriceItem(ebay_site);

            // insert into database
            using (var repository = new TradelrRepository())
            {
                var product = repository.GetProduct(ProductId);
                product.ebay_product = ebayProduct;

                product.ebay_product.ebayid = call.ItemID;
                product.ebay_product.siteid = ebay_site.ToString();
                product.ebay_product.startTime = call.Item.ListingDetails.StartTime;
                product.ebay_product.endTime = call.Item.ListingDetails.EndTime;
                product.ebay_product.isActive = true;

                foreach (FeeType fee in call.FeeList)
                {
                    if (fee.Name == "ListingFee")
                    {
                        product.ebay_product.listingFees = Currency.symbol + fee.Fee.Value.ToString("n" + Currency.decimalCount);
                    }
                }

                repository.Save("AddEbayItem");
            }
        }

        private void AddAuctionItem()
        {
            var call = service.AddItem(ebay_site);

            // insert into database
            using (var repository = new TradelrRepository())
            {
                var product = repository.GetProduct(ProductId);
                product.ebay_product = ebayProduct;

                product.ebay_product.ebayid = call.ItemID;
                product.ebay_product.siteid = ebay_site.ToString();
                product.ebay_product.startTime = call.Item.ListingDetails.StartTime;
                product.ebay_product.endTime = call.Item.ListingDetails.EndTime;
                product.ebay_product.isActive = true;

                foreach (FeeType fee in call.FeeList)
                {
                    if (fee.Name == "ListingFee")
                    {
                        product.ebay_product.listingFees = Currency.symbol + fee.Fee.Value.ToString("n" + Currency.decimalCount);
                    }
                }

                repository.Save("AddEbayItem");
            }
        }

        public void UpdateEbayItem(string itemid)
        {
            // todo: get fields which were deletedd
            // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/ReviseItem.html#Request.DeletedField
            service.ReviseItem(itemid, Enumerable.Empty<string>(), ebay_site);

            // insert into database
            using (var repository = new TradelrRepository())
            {
                var product = repository.GetProduct(ProductId);
                product.ebay_product = ebayProduct;
                product.ebay_product.ebayid = itemid;

                repository.Save("Ebay.UpdateEbayItem");
            }
        }

        public bool VerifyItem()
        {
            try
            {
                switch (ebayProduct.listingType.ToEnum<ListingType>())
                {
                    case ListingType.Chinese:
                        {
                            var call = service.VerifyAddItem(ebay_site);

                            foreach (FeeType fee in call.FeeList)
                            {
                                if (fee.Name == "ListingFee")
                                {
                                    listingFees = fee.Fee.Value;
                                }
                            }
                        }
                        break;
                    case ListingType.FixedPriceItem:
                        {
                            var call = service.VerifyAddFixedPriceItem(ebay_site);

                            foreach (FeeType fee in call.FeeList)
                            {
                                if (fee.Name == "ListingFee")
                                {
                                    listingFees = fee.Fee.Value;
                                }
                            }
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                ErrorMessage = ex.Message;
                return false;
            }
            
            return true;
        }
    }
}