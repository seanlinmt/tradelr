using System.Collections.Generic;
using tradelr.Common.Models.currency;
using tradelr.DBML;

namespace tradelr.Models.offline.tables
{
    public class ProductColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }

        public long? categoryid { get; set; }
        public decimal? costPrice { get; set; }
        public string details { get; set; }
        public int flags { get; set; }
        public string notes { get; set; }
        public string paymentterms { get; set; }
        public decimal? sellingPrice { get; set; }
        public string shippingterms { get; set; }
        public string SKU { get; set; }
        public long? stockunitid { get; set; }
        public long? thumbnailid { get; set; }
        public string title { get; set; }
    }

    public static class ProductColhelper
    {
        public static ProductColumn ToSyncModel(this product p, CFlag flag, long? offlineid = null)
        {
            return new ProductColumn
                       {
                           categoryid = p.category,
                           cflag = flag,
                           id = offlineid,
                           thumbnailid = p.thumb,
                           //SKU = p.SKU,
                           title = p.title,
                           details = p.details,
                           sellingPrice = p.sellingPrice,
                           costPrice = p.costPrice,
                           notes = p.otherNotes,
                           stockunitid = p.stockUnitId,
                           flags = p.flags,
                           serverid = p.id
                       };
        }

        public static List<ProductColumn> ToSyncModel(this IEnumerable<product> products, CFlag flag, long? offlineid = null)
        {
            var result = new List<ProductColumn>();
            foreach (var p in products)
            {
                result.Add(p.ToSyncModel(flag, offlineid));
            }
            return result;
        }
    }
}