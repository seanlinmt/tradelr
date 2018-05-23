using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tradelr.Common.Constants;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Models.jqgrid;

namespace tradelr.Models.coupons
{
    public class Coupon : CouponBasic
    {
        public string id { get; set; }
        public bool hasDuration { get; set; }
        public Currency currency { get; set; }
        public bool minimumPurchaseOnly { get; set; }
        public string minimumPurchase { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string value { get; set; }
        public string maxImpressions { get; set; }
    }

    public static class CouponHelper
    {

        public static decimal ToDiscountAmount(this cart kart, decimal cartTotal)
        {
            decimal discount = 0;
            var c = kart.MASTERsubdomain.coupons.Where(x => string.Compare(x.code, kart.coupon, true) == 0 && !x.expired).SingleOrDefault();
            if (c != null)
            {
                // 1: no min value = normal calculation
                // 2: has min value and conditions met = normal calculation
                // 3: has min value and conditions not met = 0

                // 3:
                if (c.minimumPurchase.HasValue && c.minimumPurchase.Value > cartTotal)
                {
                    // do nothing
                }
                else
                {
                    // 1: 2:
                    if (c.couponValue.HasValue)
                    {
                        discount = c.couponValue.Value;
                    }
                    else
                    {
                        discount = c.couponPercentage.Value * discount / 100;
                    }

                    if (discount > cartTotal)
                    {
                        return cartTotal;
                    }
                    return discount;
                }
            }
            return 0;
        }

        public static JqgridTable ToCouponsJqGrid(this IEnumerable<coupon> rows)
        {
            var grid = new JqgridTable();
            Currency currency = null;
            foreach (var row in rows)
            {
                if (currency == null)
                {
                    currency = row.MASTERsubdomain.currency.ToCurrency();
                }

                var entry = new JqgridRow
                {
                    id = row.id.ToString(),
                    cell = new object[] 
                                           {
                                               row.id,
                                               row.code,
                                               row.ToValueString(currency),
                                               row.ToDescriptionString(currency),
                                               row.startDate.ToString(GeneralConstants.DATEFORMAT_GRID),
                                               row.expiryDate.HasValue? row.expiryDate.Value.ToString(GeneralConstants.DATEFORMAT_GRID):"",
                                               row.impressions,
                                               row.expired?"<span class='error_post'>inactive</span>":"<span class='ok_post'>active</span>"
                                           }
                };
                grid.rows.Add(entry);
                grid.records++;
            }

            return grid;
        }

        public static IEnumerable<CouponBasic> ToModel(this IEnumerable<coupon> rows)
        {
            Currency currency = null;
            foreach (var coupon in rows)
            {
                if (currency == null)
                {
                    currency = coupon.MASTERsubdomain.currency.ToCurrency();
                }

                yield return new CouponBasic()
                                 {
                                     code = coupon.code,
                                     description = coupon.ToDescriptionString(currency)
                                 };
            }
        }

        private static string ToValueString(this coupon c, Currency currency)
        {
            if (c.couponValue.HasValue)
            {
                return string.Concat(currency.symbol, c.couponValue.Value.ToString("n" + currency.decimalCount));
            }
            if (c.couponPercentage.HasValue)
            {
                return string.Format("{0}%",c.couponPercentage.Value.ToString("n2"));
            }
            return "No Value";
        }

        private static string ToDescriptionString(this coupon c, Currency currency)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<div class='bold'>{0}</div>", c.description);
            if (c.maxImpressions.HasValue)
            {
                sb.AppendFormat("<span class='info_tag' title='max impressions'>max {0} impressions</span>",
                                c.maxImpressions.Value);
            }
            else
            {
                sb.Append("<span class='info_tag' title='unlimited use'>unlimited</span>");
            }

            if (c.minimumPurchase.HasValue)
            {
                sb.AppendFormat("<span class='info_tag'>min {0}{1}</span>", currency.symbol,
                                c.minimumPurchase.Value.ToString("n" + currency.decimalCount));
            }

            

            return sb.ToString();
        }
    }
}