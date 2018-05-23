using System.Linq;

namespace tradelr.DBML
{
    public partial class cart
    {
        public bool isDigitalOrder()
        {
            return cartitems.All(x => x.product_variant.product.products_digitals != null);
        }
    }
}
