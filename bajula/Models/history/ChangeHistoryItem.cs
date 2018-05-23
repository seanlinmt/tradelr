using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Models.users;

namespace tradelr.Models.history
{
    public class ChangeHistoryItem
    {
        public string timeModified { get; set; }
        public string field { get; set; }
        public string oldValue { get; set; }
        public string newValue { get; set; }
        public string creator { get; set; }
    }

    public static class ChangeHistoryItemHelper
    {
        public static IEnumerable<ChangeHistoryItem> ToChangeModel(this IQueryable<changeHistoryItem> items)
        {
            foreach (var item in items)
            {
                yield return new ChangeHistoryItem()
                                 {
                                     timeModified = item.changeDate.ToLocalTime().ToString("dd MMM"),
                                     field = item.changedField,
                                     oldValue = item.oldValue,
                                     newValue = item.newValue,
                                     creator = item.user.ToEmailName(true)
                                 };
            }
        }
    }
}
