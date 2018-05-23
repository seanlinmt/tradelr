using System.Collections.Generic;
using tradelr.DBML;
using tradelr.Libraries;

namespace tradelr.Models.activity
{
    public class FacebookActivityViewModel
    {
        public bool requireAdditionalPermission { get; set; }
        public bool requireToken { get; set; }
        public IEnumerable<FilterBoxListInfo> pageList { get; set; }
    }

    public static class FacebookActivityHelper
    {
        public static IEnumerable<FilterBoxListInfo> ToFilterModel(this IEnumerable<facebook_token> values)
        {
            foreach (var value in values)
            {
                yield return new FilterBoxListInfo
                                 {
                                     id = value.pageid,
                                     title = value.name,
                                     allowDelete = false
                                 };
            }
        }
    }
}