using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Library.Imaging;
using tradelr.Common.Models.photos;
using tradelr.DBML;
using tradelr.Libraries;

namespace tradelr.Models.collections
{
    public static class CollectionHelper
    {
        public static IEnumerable<FilterBoxListInfo> ToFilterList(this IEnumerable<product_collection> values)
        {
            foreach (var value in values)
            {
                yield return new FilterBoxListInfo()
                                 {
                                     id = value.id.ToString(),
                                     title = value.name
                                 };
            }
        }
    }
}