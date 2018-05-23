using System.Linq;

namespace tradelr.DBML
{
    public partial class product_variant
    {
        public bool HasStock(int quantity = 1)
        {
            if (!product.trackInventory || product.products_digitals != null)
            {
                return true;
            }
            var instock = ToQuantity();
            if (instock >= quantity)
            {
                return true;
            }
            return false;
        }

        public int ToQuantity()
        {
            var instock = inventoryLocationItems.Sum(y => y.available);
            return instock.Value;
        }

        public bool IsDigital()
        {
            return product.products_digitals != null;
        }
    }
}
