using System.ComponentModel;

namespace tradelr.Areas.dashboard.Models.store.navigation
{
    public enum LinkType
    {
        [Description("Blog")]
        BLOG = 1,
        [Description("Shop Frontpage")]
        FRONTPAGE,
        [Description("Product Collection")]
        COLLECTION,
        [Description("Page")]
        PAGE,
        [Description("Product")]
        PRODUCT,
        [Description("Search Page")]
        SEARCHPAGE,
        [Description("Web Address")]
        WEB
    }

    


}