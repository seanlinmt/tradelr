using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Libraries.Imaging;
using tradelr.Library.Constants;
using tradelr.Models.jqgrid;
using tradelr.Models.products;

namespace tradelr.Models.group
{
    public class GroupPricing
    {
        public class Prices
        {
            public long id { get; set; } // productid
            public decimal price { get; set; } // group price
        }

        public long groupid { get; set; }
        public Prices[] prices { get; set; }
    }

    public static class GroupPricingHelper
    {
        public static JqgridTable ToGroupPricingJqGrid(this IEnumerable<contactGroupPricing> rows)
        {
            Currency currency = null;

            var grid = new JqgridTable();
            foreach (var row in rows)
            {
                if (currency == null)
                {
                    currency = row.product.MASTERsubdomain.currency.ToCurrency();
                }
                var entry = new JqgridRow();
                entry.id = row.id.ToString();

                // handle external network locations
                string variantString = row.product.product_variants.ToJqgridModel(null);

                entry.cell = new object[]
                                 {
                                     row.id,
                                     "",
                                     row.product.thumb.HasValue
                                         ? Img.by_size(row.product.product_image.url, Imgsize.THUMB).ToHtmlImage()
                                         : GeneralConstants.PHOTO_NO_THUMBNAIL.ToHtmlImage(),
                                     row.product.ToProductTitle(),
                                     string.Format("<ul class='variant_jqgrid'>{0}</ul>",variantString),
                                     string.Concat(currency.symbol, row.price.ToString("n" + currency.decimalCount))
                                 };
                grid.rows.Add(entry);
            }
            return grid;

        }
    }
}