using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Models.products;

namespace tradelr.DBML.Helper
{
    public static class InventoryHelper
    {
        public static IEnumerable<product> IsActive(this IEnumerable<product> rows)
        {
            return rows.Where(x => (x.flags & (int)(ProductFlag.INACTIVE | ProductFlag.ARCHIVED)) == 0);
        }

        public static IQueryable<product> IsActive(this IQueryable<product> rows)
        {
            return rows.Where(x => (x.flags & (int)(ProductFlag.INACTIVE | ProductFlag.ARCHIVED)) == 0);
        }

        public static IQueryable<product_collection> IsVisible(this IQueryable<product_collection> rows)
        {
            return rows.Where(x => (x.settings & (int) CollectionSettings.VISIBLE) != 0);
        }
    }
}
