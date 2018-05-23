using System.Collections.Generic;
using System.Linq;
using System.Text;
using tradelr.Common;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Libraries;
using tradelr.Library;

namespace tradelr.Models.products
{
    public class ProductCategory
    {
        public long id { get; set; }
        public long? parent { get; set; }
        public string title { get; set; }
        public string details { get; set; }
        public List<ProductCategory> subcategories { get; set; }
        public ProductCategory()
        {
            subcategories = new List<ProductCategory>();
        }
    }

    public static class ProductCategoryHelper
    {
        public static string[] ToArray(this productCategory category)
        {
            if (category == null)
            {
                return new string[0];
            }
            var categories = new List<string>();
            if (category.parentID.HasValue)
            {
                string parentCategoryName = category.productCategory1.MASTERproductCategory.name;
                categories.Add(parentCategoryName);

                var categoryName = category.MASTERproductCategory.name;
                categories.Add(categoryName);

                return categories.ToArray();
            }
            categories.Add(category.MASTERproductCategory.name);
            return categories.ToArray();
        }

        public static string ToGoogleProductType(this productCategory category)
        {
            StringBuilder sb = new StringBuilder();
            if (category == null)
            {
                return sb.ToString();
            }

            if (category.parentID.HasValue)
            {
                string parentCategoryName = category.productCategory1.MASTERproductCategory.name;
                sb.Append(parentCategoryName);

                var categoryName = category.MASTERproductCategory.name;
                sb.Append(string.Concat(" > ", categoryName));

                return sb.ToString();
            }
            sb.Append(category.MASTERproductCategory.name);
            return sb.ToString();
        }

        public static IEnumerable<FilterBoxListInfo> ToFilterList(this IEnumerable<ProductCategory> values)
        {
            var data = new List<FilterBoxListInfo>();
            foreach (var value in values)
            {
                data.Add(new FilterBoxListInfo()
                             {
                                 id = value.id.ToString(),
                                 title = value.title,
                                 isSub = false
                             });

                // add sub categories
                foreach (var sub in value.subcategories)
                {
                    data.Add(new FilterBoxListInfo()
                                 {
                                     id = sub.id.ToString(),
                                     title = sub.title,
                                     isSub = true
                                 });
                }
            }
            return data;
        }

        public static ProductCategory ToModel(this productCategory obj)
        {
            return new ProductCategory
            {
                details = obj.details,
                title = obj.MASTERproductCategory.name,
                id = obj.id
            };
        }

        public static List<ProductCategory> ToModel(this IEnumerable<productCategory> values)
        {
            var categoriesList = new List<ProductCategory>();
            foreach (var value in values)
            {
                var pc = new ProductCategory()
                {
                    id = value.id,
                    title = value.MASTERproductCategory.name,
                    parent = value.parentID
                };
                foreach (var category in value.productCategories)
                {
                    pc.subcategories.Add(category.ToModel());
                }
                categoriesList.Add(pc);
            }
            return categoriesList;
        }

        public static IEnumerable<ProductCategory> ToFilterModel(this IQueryable<productCategory> values)
        {
            // main cat at the top
            var data = new List<ProductCategory>();
            foreach (var value in values.OrderBy(x => x.parentID).ThenBy(x => x.MASTERproductCategory.name))
            {
                var parentid = value.parentID;
                if (parentid != null)
                {
                    data.Where(x => x.id == parentid)
                        .Single()
                        .subcategories.Add(new ProductCategory()
                             {
                                 details = value.details,
                                 id = value.id,
                                 title =
                                     value.MASTERproductCategory
                                     .name
                             });
                }
                else
                {
                    data.Add(new ProductCategory()
                                 {
                                     details = value.details,
                                     id = value.id,
                                     title = value.MASTERproductCategory.name
                                 });
                }
            }
            return data;
        }
    }
}
