using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DBML;

namespace tradelr.Areas.dashboard.Models.product.trademe
{
    public class TrademeShippingCost
    {
        public long id { get; set; }
        public string cost { get; set; }   // todo: NZ dollars always. no?
        public string description { get; set; }
    }

    public static class TrademeShippingCostHelper
    {
        public static IEnumerable<TrademeShippingCost> ToModel(this IEnumerable<trademe_shippingcost> rows)
        {
            foreach (var row in rows)
            {
                yield return new TrademeShippingCost()
                                 {
                                     id = row.id,
                                     cost = row.cost.ToString("n" + 2),
                                     description = row.description
                                 };
            }
        }
    }
}