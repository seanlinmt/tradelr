using System;
using DotLiquid;
using tradelr.Areas.dashboard.Models.store.navigation;

namespace tradelr.Models.liquid.models.LinkList
{
    public class Link : Drop
    {
        public string url { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public object @object { get; set; }
        public bool active { get; set; }
    }

    public static class LinkHelper
    {
        public static string ToLinkType(this LinkType type)
        {
            var ret = "";
            switch (type)
            {
                case LinkType.BLOG:
                    ret = "blog_link";
                    break;
                case LinkType.FRONTPAGE:
                    ret = "relative_link";
                    break;
                case LinkType.COLLECTION:
                    ret = "collection_link";
                    break;
                case LinkType.PAGE:
                    ret = "page_link";
                    break;
                case LinkType.PRODUCT:
                    ret = "product_link";
                    break;
                case LinkType.SEARCHPAGE:
                    ret = "relative_link";
                    break;
                case LinkType.WEB:
                    ret = "http_link";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            return ret;
        }
    }
}