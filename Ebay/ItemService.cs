using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Ebay.Enums;
using Ebay.Resources;
using eBay.Service.Call;
using eBay.Service.Core.Soap;

namespace Ebay
{
    public class ItemService : EbayService
    {
        private ItemType item { get; set; }

        private ShippingDetailsType shippingDetails { get; set; }
        
        // payment options
        private BuyerPaymentMethodCodeTypeCollection paymentOptions { get; set; }

        public ItemService(string token)
            : base(token)
        {
            paymentOptions = new BuyerPaymentMethodCodeTypeCollection();
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/AddFixedPriceItem.html
        public AddFixedPriceItemCall AddFixedPriceItem(SiteCodeType siteid)
        {
            
            var call = new AddFixedPriceItemCall(api);
            call.Site = siteid;

            call.AddFixedPriceItem(item);

            return call;
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/AddItem.html
        public AddItemCall AddItem(SiteCodeType siteid)
        {
            var call = new AddItemCall(api);
            call.Site = siteid;

            call.AddItem(item);

            return call;
        }


        public void AddPaymentOption(BuyerPaymentMethodCodeType method, string paypalEmailAddress = "")
        {
            paymentOptions.Add(method);

            if (method == BuyerPaymentMethodCodeType.PayPal)
            {
                Debug.Assert(!string.IsNullOrEmpty(paypalEmailAddress));
                item.PayPalEmailAddress = paypalEmailAddress;
            }
        }

        // http://developer.ebay.com/devzone/xml/docs/reference/ebay/types/AddressType.html
        public void AddSellerContactDetails(string city, string companyname, CountryCodeType country, string streetaddress, string phone,
            string postcode, string state)
        {
            item.SellerContactDetails = new AddressType()
            {
                CityName = city,
                CompanyName = companyname,
                Country = country,
                Phone = phone,
                PostalCode = postcode,
                StateOrProvince = state,
                Street = streetaddress
            };
        }

        public void AddShippingDetails(ShippingTypeCodeType type,
            ShippingServiceOptionsTypeCollection domestic_options,
            InternationalShippingServiceOptionsTypeCollection international_options
            )
        {
            // shipping details
            shippingDetails = new ShippingDetailsType();

            shippingDetails.ApplyShippingDiscount = false;

            shippingDetails.PaymentInstructions = "";

            // shipping insurance??

            // Shipping type and shipping service options
            shippingDetails.ShippingType = type;

            // domestic shipping options
            shippingDetails.ShippingServiceOptions = domestic_options;

            // international shipping options
            shippingDetails.InternationalShippingServiceOption = international_options;

            item.ShippingDetails = shippingDetails;
        }

        public void AddProductVariant(VariationType variant)
        {
            if (item.Variations == null)
            {
                item.Variations = new VariationsType
                                      {
                                          Variation = new VariationTypeCollection()
                                      };
            }

            item.Variations.Variation.Add(variant);
        }

        // http://developer.ebay.com/DevZone/XML/docs/WebHelp/wwhelp/wwhimpl/common/html/wwhelp.htm?context=eBay_XML_API&file=DescribingListing-Working_with_Pictures_in_an_Item_Listing.html
        public void AddPhotos(string[] urls)
        {
            if (item.PictureDetails == null)
            {
                item.PictureDetails = new PictureDetailsType();
            }
            item.PictureDetails.PictureURL = new StringCollection(urls);
        }

        // startprice = starting price for auctions, selling price for fixeditems
        public ItemType BuildItem(SiteCodeType site, ListingTypeCodeType listingType, string title, string description, 
            CurrencyCodeType currency, double? buynowPrice, double startPrice, double? reservePrice, string location, CountryCodeType country,
            int categoryid, int quantity, int? conditionID, ReturnsAccepted returns, string listingDuration,
            string refund_policy, string return_within, int dispatch_time, string paypal_email)
        {
            item = new ItemType();

            item.Site = site;

            // item title
            item.Title = title;

            // item description
            // append powered by tradelr
            if (description.IndexOf("http://wwww.tradelr.com/Content/img/tradelr.png") == -1)
            {
                item.Description =
                string.Format("{0}<p>&nbsp;</p><p><center><img src='http://wwww.tradelr.com/Content/img/tradelr.png' /><div>powered by <a target='_blank' href='http://www.tradelr.com'>tradelr</a></div></center></p>",
                description);
            }
            else
            {
                item.Description = description;
            }

            // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/types/ListingTypeCodeType.html
            item.ListingType = listingType;

            // prices
            item.Currency = currency;
            item.StartPrice = new AmountType
                                  {
                                      Value = startPrice,
                                      currencyID = currency
                                  };

            if (buynowPrice.HasValue)
            {
                item.BuyItNowPrice = new AmountType
                                         {
                                             Value = buynowPrice.Value,
                                             currencyID = currency
                                         };
            }

            if (reservePrice.HasValue)
            {
                item.ReservePrice = new AmountType()
                                        {
                                            Value = reservePrice.Value,
                                            currencyID = currency
                                        };
            }

            // listing duration
            item.ListingDuration = listingDuration;

            // item location and country
            item.Location = location;
            item.Country = country;

            // listing category, Photography Software
            var category = new CategoryType();
            category.CategoryID = categoryid.ToString();
            item.PrimaryCategory = category;

            // item quality
            item.Quantity = quantity;

            // payment methods
            item.PaymentMethods = paymentOptions;

            // email is required if paypal is used as payment method
            item.PayPalEmailAddress = paypal_email;

            // http://developer.ebay.com/devzone/finding/callref/Enums/conditionIdList.html
            if (conditionID.HasValue)
            {
                item.ConditionID = conditionID.Value;
            }

            item.DispatchTimeMax = dispatch_time;

            // return policy
            var policy = new ReturnPolicyType
                             {
                                 ReturnsAcceptedOption = returns.ToString(), 
                                 ShippingCostPaidByOption = "Buyer"
                             };

            if (returns == ReturnsAccepted.ReturnsAccepted)
            {
                policy.Refund = refund_policy;
                policy.ReturnsWithinOption = return_within;
            }

            item.ReturnPolicy = policy;

            return item;
        }

        // http://developer.ebay.com/devzone/xml/docs/reference/ebay/EndFixedPriceItem.html
        public void EndFixedPriceItem(string itemid, EndReasonCodeType reason)
        {
            var call = new EndFixedPriceItemCall(api);

            call.EndFixedPriceItem(itemid, reason, "");
        }

        // http://developer.ebay.com/devzone/xml/docs/reference/ebay/getitem.html
        public ItemType GetItem(string itemid)
        {
            var call = new GetItemCall(api);

            call.DetailLevelList.Add(DetailLevelCodeType.ReturnAll);

            var item = call.GetItem(itemid);

            responseXML = call.SoapResponse;

            return item;
        }

        // http://developer.ebay.com/devzone/XML/docs/Reference/eBay/ReviseItem.html
        public void ReviseItem(string itemid, IEnumerable<string> deletedFields, SiteCodeType siteid)
        {
            var call = new ReviseItemCall(api);
            call.Site = siteid;

            item.ItemID = itemid;

            // need to identify which fields can be updated
            call.ReviseItem(item, new StringCollection(deletedFields.ToArray()), false);
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/VerifyAddItem.html
        public VerifyAddItemCall VerifyAddItem(SiteCodeType siteid)
        {
            var call = new VerifyAddItemCall(api);
            call.Site = siteid;

            call.VerifyAddItem(item);

            return call;
        }

        // http://developer.ebay.com/DevZone/xml/docs/Reference/ebay/VerifyAddFixedPriceItem.html
        public VerifyAddFixedPriceItemCall VerifyAddFixedPriceItem(SiteCodeType siteid)
        {
            var call = new VerifyAddFixedPriceItemCall(api);
            call.Site = siteid;

            call.VerifyAddFixedPriceItem(item);

            return call;
        }
    }
}
