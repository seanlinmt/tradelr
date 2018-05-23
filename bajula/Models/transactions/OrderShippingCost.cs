using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Models.currency;
using tradelr.DBML;

namespace tradelr.Models.transactions
{
    public class OrderShippingCost
    {
        public long id { get; set; }
        public string shippingMethod { get; set; }
        public string shippingCost { get; set; }
    }

    public static class OrderShippingCostHelper
    {
        public static OrderShippingCost ToShippingCostModel(this order value)
        {
            var currency = value.currency.ToCurrency();
            return new OrderShippingCost()
                       {
                           id = value.id,
                           shippingMethod = value.shippingMethod,
                           shippingCost = value.shippingCost.HasValue? value.shippingCost.Value.ToString("n" + currency.decimalCount):""
                       };
        }
    }
}