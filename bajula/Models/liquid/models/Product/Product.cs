using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using DotLiquid;
using tradelr.DBML;
using tradelr.Library;
using tradelr.Library.Constants;
using tradelr.Models.products;

namespace tradelr.Models.liquid.models.Product
{
    public class Product : Drop
    {
        public long id { get; set; }
        public string title { get; set; }

        public decimal? price { get; set; }   // min(sellingprice, grouprpice, special price)
        public decimal? selling_price { get; set; }
        
        public string url { get; set; }
        public string description { get; set; }
        public string content
        {
            get { return description; }
        }
        public string handle { get; set; }
        public List<string> options { get; set; }
        public IEnumerable<Variant> variants { get; set; }
        public bool available { get; set; }
        public string[] tags { get; set; } // todo
        
        public List<string> images { get; set; }
        public string default_image { get; set; }

        // proprietary
        public bool @new { get; set; }

        [ScriptIgnore]
        public decimal? weight { get; set; }
        [ScriptIgnore]
        public decimal? height { get; set; }
        [ScriptIgnore]
        public decimal? width { get; set; }
        [ScriptIgnore]
        public decimal? length { get; set; }

        public Product()
        {
            options = new List<string>();
        }
    }

    public static class ProductHelper
    {
        public static IEnumerable<Product> ToLiquidModel(this IQueryable<product> rows, long? viewerid, string collectionHandle)
        {
            foreach (var row in rows.OrderBy(x => x.id))
            {
                yield return row.ToLiquidModel(viewerid, collectionHandle);
            }
        }

        /// <summary>
        /// returns minimum price with tax
        /// </summary>
        /// <param name="row"></param>
        /// <param name="viewerid"></param>
        /// <returns></returns>
        public static decimal? ToUserPrice(this product row, long? viewerid)
        {
            decimal? groupPrice = null;
            if (viewerid.HasValue)
            {
                var groupPricing = row.contactGroupPricings.FirstOrDefault(x => x.contactGroup.contactGroupMembers.Count(y => y.userid == viewerid.Value) != 0);

                if (groupPricing != null)
                {
                    groupPrice = groupPricing.price;
                }
            }

            // handle prices
            var prices = new List<decimal?> { row.sellingPrice, groupPrice, row.specialPrice };
            var minprice = prices.Min();
            if (row.tax.HasValue && minprice.HasValue)
            {
                minprice = minprice*(row.tax.Value/100 + 1);
            }
            return minprice;
        }

        public static decimal? ToUserSellingPrice(this product row)
        {
            if (row.tax.HasValue && row.sellingPrice.HasValue)
            {
                return row.sellingPrice * (row.tax.Value / 100 + 1);
            }
            return row.sellingPrice;
        }

        public static Product ToLiquidModel(this product row, long? viewerid, string collectionHandle)
        {
            var P = new Product()
                        {
                            id = row.id,
                            handle = row.id.ToString(),
                            title = row.title,
                            description = row.details,
                            price = row.ToUserPrice(viewerid),
                            selling_price = row.ToUserSellingPrice(),
                            @new = (DateTime.UtcNow - row.created).Days <= 7,
                            tags =
                                row.tags1 != null
                                    ? row.tags1.Select(x => x.name.Replace("_"," ")).ToArray()
                                    : null,
                            images = row.product_images.Select(x => x.url).ToList(),
                            default_image = row.thumb.HasValue
                                                 ? row.product_image.url
                                                 : ""
                        };

            // handle url
            if (!string.IsNullOrEmpty(collectionHandle))
            {
                P.url = P.ToLiquidProductInCollectionUrl(collectionHandle);
            }
            else
            {
                P.url = row.ToLiquidProductUrl();
            }

            // handle variants
            P.variants = row.product_variants.ToLiquidVariantModel(P.price).OrderByDescending(x => x.available).ToArray();

            // handle options
            if (row.product_variants.Any(x => !String.IsNullOrEmpty(x.color)))
            {
                P.options.Add("color");
            }

            if (row.product_variants.Any(x => !String.IsNullOrEmpty(x.size)))
            {
                P.options.Add("size");
            }

            // handle product availability
            P.available = row.HasStock();

            // if there's no image then add the no thumbnail image
            if (P.images.Count == 0)
            {
                P.images.Add("");
            }

            return P;
        }

        public static string ToLiquidProductUrl(this product p)
        {
            return String.Concat("/products/", p.id, "/", p.title.ToSafeUrl());
        }

        public static string ToLiquidProductInCollectionUrl(this Product p, string collectionHandle)
        {
            return String.Format("/collections/{0}/products/{1}/{2}", collectionHandle, p.id, p.title.ToSafeUrl());
        }

        
    }
}