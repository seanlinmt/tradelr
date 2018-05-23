using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebay.Enums;
using clearpixels.Logging;
using eBay.Service.Core.Soap;

namespace Ebay.Resources
{
    /// <summary>
    /// this converts ebay listing to tradelr listing
    /// </summary>
    public class Listing
    {
        public string id { get; set; }

        public double productPrice { get; set; }  // this is for tradelr product entry
        public double startPrice { get; set; }
        public double? buynowPrice { get; set; }
        public double? reservePrice { get; set; }

        public string title { get; set; }
        public string description { get; set; }

        public string category1 { get; set; }
        public string category2 { get; set; }

        public int categoryid1 { get; set; }
        public int categoryid2 { get; set; }

        public List<ListingVariant> variants { get; set; }
        public IEnumerable<string> photoUrls { get; set; }

        public int condition { get; set; }
        public string returnPolicy { get; set; }
        public string returnWithin { get; set; }
        public string refundPolicy { get; set; }
        public string duration { get; set; }
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }

        public SiteCodeType siteid { get; set; }
        public ListingTypeCodeType listingType { get; set; } 

        public int quantity { get; set; }
        public int dispatchTime { get; set; }
        public bool isActive { get; set; }

        public Listing()
        {
            variants = new List<ListingVariant>();
            photoUrls = Enumerable.Empty<string>();
        }

        // used to import products from ebay
        public void Populate(ItemType item)
        {
            siteid = item.Site;
            listingType = item.ListingType;

            if (item.ReservePrice != null)
            {
                reservePrice = item.ReservePrice.Value;
            }

            if (item.StartPrice != null)
            {
                startPrice = item.StartPrice.Value;
            }

            if (item.BuyItNowPrice != null)
            {
                buynowPrice = item.BuyItNowPrice.Value;
            }

            if (item.ReservePrice != null && item.ReservePrice.Value != 0)
            {
                productPrice = item.ReservePrice.Value;
            }
            else if (item.StartPrice != null && item.StartPrice.Value != 0)
            {
                productPrice = item.StartPrice.Value;
            }
            else if (item.SellingStatus != null &&
                item.SellingStatus.CurrentPrice != null &&
                item.SellingStatus.CurrentPrice.Value != 0)
            {
                productPrice = item.SellingStatus.CurrentPrice.Value;
            }

            title = item.Title;
            id = item.ItemID;
            description = item.Description ?? "";
            if (item.PrimaryCategory != null)
            {
                category1 = item.PrimaryCategory.CategoryName;
                categoryid1 = int.Parse(item.PrimaryCategory.CategoryID);
            }
            if (item.SecondaryCategory != null)
            {
                category2 = item.SecondaryCategory.CategoryName;
                categoryid2 = int.Parse(item.SecondaryCategory.CategoryID);
            }
            if (item.Variations != null && item.Variations.Variation.Count > 1)
            {
                foreach (VariationType entry in item.Variations.Variation)
                {
                    var variant = new ListingVariant();
                    variant.sku = entry.SKU;
                    variant.quantity = entry.Quantity;
                    foreach (NameValueListType property in entry.VariationSpecifics)
                    {
                        variant.properties.Add(property.Name.ToLower(), string.Join(",", property.Value.ToArray()));
                    }
                    variants.Add(variant);
                }
            }
            else
            {
                var variant = new ListingVariant {sku = item.SKU, quantity = item.Quantity};
                variants.Add(variant);
            }

            if (item.PictureDetails != null && item.PictureDetails.PictureURL != null)
            {
                photoUrls = item.PictureDetails.PictureURL.ToArray();
            }

            condition = item.ConditionID;

            if (item.ReturnPolicy != null)
            {
                returnPolicy = item.ReturnPolicy.ReturnsAccepted;
                returnWithin = item.ReturnPolicy.ReturnsWithin;
                refundPolicy = item.ReturnPolicy.Refund;
            }
            else
            {
                returnPolicy = ReturnPolicy.ReturnsNotAccepted.ToString();
            }
            duration = item.ListingDuration;

            if (item.ListingDetails != null)
            {
                startTime = item.ListingDetails.StartTime;
                if (item.ListingDetails.EndTimeSpecified)
                {
                    endTime = item.ListingDetails.EndTime;
                }
            }

            quantity = item.Quantity;
            dispatchTime = item.DispatchTimeMax;
            if (item.SellingStatus != null)
            {
                isActive = item.SellingStatus.ListingStatus == ListingStatusCodeType.Active;
            }
        }

        public static ListingType GetTradelrSupportedType(ListingTypeCodeType ebayType)
        {
            switch (ebayType)
            {
                case ListingTypeCodeType.Chinese:
                case ListingTypeCodeType.Auction:
                    return ListingType.Chinese;
                case ListingTypeCodeType.FixedPriceItem:
                case ListingTypeCodeType.StoresFixedPrice:
                    return ListingType.FixedPriceItem;
                default:
                case ListingTypeCodeType.Unknown:
                case ListingTypeCodeType.Dutch:
                case ListingTypeCodeType.Live:
                case ListingTypeCodeType.AdType:
                case ListingTypeCodeType.PersonalOffer:
                case ListingTypeCodeType.Half:
                case ListingTypeCodeType.LeadGeneration:
                case ListingTypeCodeType.Express:
                case ListingTypeCodeType.Shopping:
                case ListingTypeCodeType.CustomCode:
                    Syslog.Write("Unsupported listing: " + ebayType);
                    return ListingType.FixedPriceItem;
                    break;
            }
        }
    }
}
