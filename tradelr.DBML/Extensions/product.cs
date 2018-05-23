using System.Linq;

namespace tradelr.DBML
{
    public partial class product
    {
        public bool HasStock(int quantity = 1)
        {
            if (!trackInventory || products_digitals != null)
            {
                return true;
            }
            var instock = product_variants.SelectMany(x => x.inventoryLocationItems).Sum(y => y.available);
            if (instock.Value >= quantity)
            {
                return true;
            }
            return false;
        }

        public bool IsInUse()
        {
            return product_variants.SelectMany(x => x.orderItems).Count() != 0 ||
                product_variants.Select(x => x.cartitems).Count() != 0;
        }
    }
}
