using System.Collections.Generic;
using clearpixels.Facebook.Resources;

namespace tradelr.Areas.dashboard.Models.product.ebay
{
    /// <summary>
    /// DO NOT CHANGE THIS AS IT WILL AFFECT JSON OBJECT!!!!!!!!
    /// </summary>
    public class EbayCategoryCondition
    {
        public IEnumerable<IdName> categories { get; set; }  
        public IEnumerable<IdName> conditions { get; set; }
        public IEnumerable<IdName> durations { get; set; } 
    }
}