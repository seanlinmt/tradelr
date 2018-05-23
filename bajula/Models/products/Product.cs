using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Common;
using tradelr.Common.Constants;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.Constants;
using tradelr.Models.comments;
using tradelr.Models.inventory;
using tradelr.Models.jqgrid;
using tradelr.Models.networks;
using tradelr.Models.shipwire;

namespace tradelr.Models.products
{
    public class Product : ProductStore
    {
        public bool isOwner { get; set; }
        public bool isFBConnected { get; set; }
        public bool limitHit { get; set; }
        public string otherNotes { get; set; }
        public string supplierPrice { get; set; }
        public string stockUnitId { get; set; }
        public string stockUnit { get; set; }

        // photossss
        public List<Photo> productPhotos { get; set; }
        public long? mainPhoto { get; set; }

        // shipwire details
        public shipwire.Shipwire shipwireDetails { get; set; }
        public long? shippingProfileID { get; set; }

        // inventory
        public List<InventoryLocation> inventoryLocations { get; set; }
        public bool trackInventory { get; set; }

        // digital product
        public ProductDigital digital { get; set; }

        public Product()
        {
            digital = new ProductDigital();
            productPhotos = new List<Photo>();
            shipwireDetails = new shipwire.Shipwire();
            inventoryLocations = new List<InventoryLocation>();
        }
    }

    public static class ProductHelper
    {
        public static Product ToModel(this product p, long subdomainid, long productid, long? viewerid)
        {
            object data;
            // this can be null when client trying to obtain product using offline product id
            if (p == null)
            {
                return null;
            }
            var currency = p.MASTERsubdomain.currency.ToCurrency();
            var key = CacheHelper.GetKey(productid.ToString(), viewerid);
            if (!CacheHelper.Instance.TryGetCache(CacheItemType.products_single, key, out data))
            {
                data = new Product
                           {
                               isOwner = p.MASTERsubdomain.id == subdomainid,
                               isFBConnected = !string.IsNullOrEmpty(p.MASTERsubdomain.organisation.users.First().FBID),
                               id = p.id,
                               currency = currency,
                               variants = p.product_variants.ToModel(),
                               title = p.title,
                               details = p.details,
                               mainPhoto = p.thumb,
                               otherNotes = p.otherNotes,
                               supplierPrice =
                                   p.costPrice.HasValue ? p.costPrice.Value.ToString("n" + currency.decimalCount) : "",
                               stockUnitId = p.stockUnitId.HasValue ? p.stockUnitId.Value.ToString() : "",
                               category = p.category.HasValue ? p.category.Value : (long?) null,
                               parentCategory =
                                   p.category.HasValue
                                       ? (p.productCategory.parentID.HasValue ? p.productCategory.parentID : null)
                                       : null,
                               shippingProfileID = p.shippingProfileID,
                                       shipwireDetails = p.shipwireDetails.ToShipwire(),
                               stockUnit = p.stockUnitId.HasValue ? p.stockUnit.MASTERstockUnit.name : "",
                               tags = p.tags1 != null ? string.Join(",", p.tags1.Select(x => x.name.Replace("_", " "))):"",
                               tax = p.tax.HasValue? p.tax.Value.ToString(): "",
                               trackInventory = p.trackInventory,
                               digital = p.products_digitals.ToModel()
                           };
                ((Product)data).InitialisePrices(p);
                CacheHelper.Instance.Insert(CacheItemType.products_single, key, data);
                CacheHelper.Instance.add_dependency(DependencyType.products_single, productid.ToString(),
                                                    CacheItemType.products_single, key);
            }

            return (Product) data;
        }

        public static string ToProductTitle(this product row)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<a href='/dashboard/product/edit/{0}' class='pt' title='edit product'>{1}</a>", row.id, row.title);
            sb.Append("<div>");

            if ((row.flags & (int)ProductFlag.INACTIVE) != 0)
            {
                sb.Append("<img src='/Content/img/bullets/lock.png' title='private' />");
            }
            
            if (row.category.HasValue)
            {
                sb.AppendFormat("<span class='info_tag'>{0}</span>", row.productCategory.MASTERproductCategory.name);
            }

            if (row.sellingPrice.HasValue)
            {
                var currency = row.MASTERsubdomain.currency.ToCurrency();
                if (row.specialPrice.HasValue)
                {
                    sb.AppendFormat("<span class='sp strike'>{0}{1}</span><span class='sp'>{2}{3}</span>",
                                    currency.symbol,
                                    row.sellingPrice.Value.ToString("n" + currency.decimalCount), 
                                    currency.symbol,
                                    row.specialPrice.Value.ToString("n" + currency.decimalCount));
                }
                else
                {
                    sb.AppendFormat("<span class='sp'>{0}{1}</span>", currency.symbol,
                                row.sellingPrice.Value.ToString("n" + currency.decimalCount));
                }
            }
            sb.Append("</div>");

            return sb.ToString();
        }

        public static string ToProductUrl(this product p)
        {
            return string.Concat(GeneralConstants.URL_SINGLE_PRODUCT_SHOW, p.id, "/", p.title.ToSafeUrl());
        }

        public static string ToProductEditUrl(this product p)
        {
            return string.Format("/dashboard/product/edit/{0}", p.id);
        }

        public static IEnumerable<Product> ToModel(this IEnumerable<product> values)
        {
            Currency currency = null;
            foreach (var value in values)
            {
                if (currency == null)
                {
                    currency = value.MASTERsubdomain.currency.ToCurrency();
                }
                yield return new Product
                {
                    id = value.id,
                    variants = value.product_variants.ToModel(),
                    title = value.title,
                    sellingPrice = value.sellingPrice.HasValue ? (value.specialPrice ?? value.sellingPrice).Value.ToString("n" + currency.decimalCount) : "",
                    tax = value.tax.HasValue? value.tax.Value.ToString(): "",
                    thumbnailUrl = value.thumb.HasValue ? Img.by_size(value.product_image.url, Imgsize.THUMB).ToHtmlImage() : GeneralConstants.PHOTO_NO_THUMBNAIL.ToHtmlImage(),
                };
            }
        }

        public static JqgridTable ToProductJqGrid(this IEnumerable<product> rows, inventoryLocation location)
        {
            var grid = new JqgridTable();
            foreach (var row in rows)
            {
                var entry = new JqgridRow();
                entry.id = row.id.ToString();

                var items = row.product_variants.SelectMany(x => x.inventoryLocationItems);
                if (location != null)
                {
                    items = items.Where(y => y.locationid == location.id);
                }
                
                int? inventoryLevel = items.All(x => x.available == null)
                                          ? null
                                          : items.Sum(y => y.available);

                // handle external network locations
                string variantString = "";
                if (location != null)
                {
                    switch (location.name)
                    {
                        case Networks.LOCATIONNAME_GBASE:
                            variantString = row.gbase_product.ToJqgridModel();
                            break;
                        case Networks.LOCATIONNAME_EBAY:
                            variantString = row.ebay_product.ToJqgridModel();
                            break;
                        default:
                            variantString = row.product_variants.ToJqgridModel(location.id);
                            break;
                    }
                }
                else
                {
                    variantString = row.product_variants.ToJqgridModel(null);
                }

                entry.cell = new object[]
                                 {
                                     row.id,
                                     "",
                                     row.thumb.HasValue
                                         ? Img.by_size(row.product_image.url, Imgsize.THUMB).ToHtmlImage()
                                         : GeneralConstants.PHOTO_NO_THUMBNAIL.ToHtmlImage(),
                                     row.ToProductTitle(),
                                     string.Format("<table class='jqgrid_variants'>{0}</table>",variantString),
                                     string.Format("<div class='bold larger'><a class='variant_transactions_link' href='/dashboard/product/transactions/{0}'>{1}</a></div>", row.id, inventoryLevel.ToInventoryLevelString()),
                                     row.hits
                                 };
                grid.rows.Add(entry);
            }
            return grid;
        }

        public static string ToInventoryLevelString(this int? total)
        {
            if (total.HasValue)
            {
                return total.Value.ToString();
            }

            return "∞";
        }

        private static string ToJqgridModel(this ebay_product value)
        {
            return string.Format("<tr><td colspan='5'><strong>{0}</strong> <a href='{1}' target='_blank'>view</a></td></tr>", value.isActive?"Active":"Ended", value.ToExternalLink());
        }

        private static string ToJqgridModel(this gbase_product value)
        {
            var sb = new StringBuilder();
            sb.Append("<tr>");
            var gbaseflag = (InventoryItemFlag)value.flags;
            if (gbaseflag.HasFlag(InventoryItemFlag.DRAFT))
            {
                sb.AppendFormat("<td colspan='5'><strong>draft</strong></td>");
            }
            else
            {
                sb.AppendFormat("<td colspan='5'><strong>active</strong> <a href='{0}' target='_blank'>view external site</a></td>", value.externallink);
            }
            sb.Append("</tr>");
            return sb.ToString();
        }

        private static decimal? ToSellingPrice(this product p)
        {
            var prices = new List<decimal?> { p.specialPrice, p.sellingPrice };

            var price = prices.Min();

            if (p.tax.HasValue)
            {
                price = price*(1 + p.tax.Value/100);
            }

            if (price == 0)
            {
                return null;
            }

            return price;
        }

        public static string ToSellingPrice(this product p, Currency currency)
        {
            var price = p.ToSellingPrice();
            if (!price.HasValue)
            {
                return "";
            }
            return string.Concat(currency.symbol, price.Value.ToString("n" + currency.decimalCount));
        }
    }
}
