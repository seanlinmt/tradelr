using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Libraries;

namespace tradelr.Models.inventory
{
    public class InventoryLocation
    {
        public long id { get; set;}
        public bool createmode { get; set; }
        public string title { get; set;}
        public List<InventoryLocationItem> locationItems { get; set; }

        public InventoryLocation()
        {
            locationItems = new List<InventoryLocationItem>();
        }
    }

    public static class InventoryHelper
    {
        public static InventoryLocation ToModel(this inventoryLocation value)
        {
            return new InventoryLocation()
                       {
                           id = value.id,
                           title = value.name
                       };
        }

        public static IEnumerable<InventoryLocation> ToFilterModel(this IQueryable<inventoryLocation> values)
        {
            foreach (var value in values)
            {
                yield return new InventoryLocation()
                                 {
                                     id = value.id,
                                     title = value.name
                                 };
            }
        }

        public static IEnumerable<FilterBoxListInfo> ToFilterList(this IEnumerable<InventoryLocation> values)
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
            }
            return data;
        }
    }
}