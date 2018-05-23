using System.Linq;
using tradelr.Common.Models.currency;
using tradelr.DBML;

namespace tradelr.Models.products
{
    public class ProductStore : ProductBase
    {
        public string details { get; set; }
        public Currency currency { get; set; }
        public long? parentCategory { get; set; } // will have a value if product is in a subcategory
        public long? category { get; set; }
        public string tags { get; set; }

        public string sellingPriceWithTax { get; set; }

        // dimensions
        public Dimension dimension { get; set; }

        public ProductStore()
        {
            dimension = new Dimension();
            
        }

        public void InitialisePrices(product p)
        {
            sellingPrice =
                p.sellingPrice.HasValue
                    ? p.sellingPrice.Value.ToString("n" + currency.decimalCount)
                    : "";
            sellingPriceWithTax =
                p.sellingPrice.HasValue
                    ? (p.tax.HasValue
                           ? (p.sellingPrice.Value*(p.tax.Value/100 + 1)).ToString("n" +
                                                                                   currency.
                                                                                       decimalCount)
                           : p.sellingPrice.Value.ToString("n" + currency.decimalCount))
                    : "";
            specialPrice =
                p.specialPrice.HasValue
                    ? (p.tax.HasValue
                           ? (p.specialPrice.Value*(p.tax.Value/100 + 1)).ToString("n" +
                                                                                   currency.
                                                                                       decimalCount)
                           : p.specialPrice.Value.ToString("n" + currency.decimalCount))
                    : null;
        }
    }
}
