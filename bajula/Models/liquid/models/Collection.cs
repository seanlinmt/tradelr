using System.Collections.Generic;
using System.Linq;
using DotLiquid;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Models.liquid.models.Product;

namespace tradelr.Models.liquid.models
{
    public class Collection : Drop
    {
        public string title { get; set; }
        public string handle { get; set; }
        public string description { get; set; }
        public List<Product.Product> products { get; set; }
        public int products_count
        {
            get { return products.Count(); }
        }
        public int all_products_count { get; set; }
        public IEnumerable<TitleUrl> all_tags { get; set; }
        public string next_product { get; set; }
        public string previous_product { get; set; }
        public string url { get; set; }

        public Collection(string name, string handle, MASTERsubdomain sd)
        {
            title = name;
            this.handle = handle;
            url = "/collections/" + handle;
            all_tags = sd.tags.OrderBy(x => x.name).GroupBy(x => x.handle).Select(y => new TitleUrl()
                                                                    {
                                                                        is_link = true,
                                                                        title = y.First().name,
                                                                        url = y.First().handle
                                                                    });
        }

        public Collection(IQueryable<product_collection> collections, long? viewerid)
            : this("Products", "all", collections.First().MASTERsubdomain)
        {
            description = "";
            all_products_count = collections.SelectMany(x => x.productCollectionMembers).Distinct().Count();
            products = collections.SelectMany(x => x.productCollectionMembers)
                .Select(x => x.product)
                .IsActive()
                .Distinct()
                .ToLiquidModel(viewerid, "")
                .OrderByDescending(y => y.id)
                .ToList();
        }

        public Collection(product_collection collection, IEnumerable<string> match_tags, long? viewerid) 
            : this(collection.name, collection.permalink, collection.MASTERsubdomain)
        {
            description = collection.details;
            all_products_count = collection.productCollectionMembers.AsQueryable()
                .Select(x => x.product)
                .IsActive()
                .Count();

            if (match_tags != null)
            {
                products = collection.productCollectionMembers
                    .AsQueryable()
                    .Select(x => x.product)
                    .Where(y => y.tags1.Select(z => z.handle).Intersect(match_tags).Count() != 0)
                    .IsActive()
                    .ToLiquidModel(viewerid, collection.permalink)
                    .OrderByDescending(y => y.id)
                    .ToList();
            }
            else
            {
                products = collection.productCollectionMembers
                    .AsQueryable()
                    .Select(x => x.product)
                    .IsActive()
                    .ToLiquidModel(viewerid, collection.permalink)
                    .OrderByDescending(y => y.id)
                    .ToList();
            }
        }
    }
}