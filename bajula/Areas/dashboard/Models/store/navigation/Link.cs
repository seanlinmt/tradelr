using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;

namespace tradelr.Areas.dashboard.Models.store.navigation
{
    public class Link
    {
        public const string DEFAULT_FRONTPAGE = "/";
        public const string DEFAULT_404 = "/404";
        public const string DEFAULT_SEARCHPAGE = "/search";

        public string url_selected { get; set; }
        public string url_raw { get; set; }
        public string url_filter { get; set; }
        public SelectList typeList { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string permalink { get; private set; }
        private LinkType _type { get; set; }

        public LinkType link_type
        {
            get { return _type; }
            set { _type = value; }
        }

        public string type
        {
            get
            {
                return _type.ToString();
            }
            set
            {
                _type = (LinkType)Enum.Parse(typeof(LinkType), value, true);
            }
        }

        public Link()
        {
            typeList = new SelectList(LinkList.referenceLinkTypeList, "Value", "Text");
        }

        public Link(link row)
        {
            id = row.id.ToString();
            title = row.title;
            link_type = (LinkType)row.type;
            typeList = new SelectList(LinkList.referenceLinkTypeList, "Value", "Text", link_type);

            if (link_type != LinkType.WEB && !string.IsNullOrEmpty(row.url))
            {
                var segments = row.url.Split(new[] { '/' });
                if (segments.Length == 4)
                {
                    permalink = segments[2];
                    url_filter = segments[3].Replace("+", "");
                }

                if (segments.Length == 3)
                {
                    permalink = segments[2];
                }  
            }

            var sd = row.linklist.MASTERsubdomain;

            switch (link_type)
            {
                case LinkType.BLOG:
                    url_selected = sd.blogs.Where(x => x.permalink == permalink)
                                                .Select(x => x.id.ToString())
                                                .SingleOrDefault();
                    break;
                case LinkType.FRONTPAGE:
                    // do nothing
                    break;
                case LinkType.COLLECTION:
                    url_selected = sd.product_collections.Where(x => x.permalink == permalink)
                                                .Select(x => x.id.ToString())
                                                .SingleOrDefault();
                    break;
                case LinkType.PAGE:
                    url_selected = sd.pages.Where(x => x.permalink == permalink)
                                                .Select(x => x.id.ToString())
                                                .SingleOrDefault();
                    break;
                case LinkType.PRODUCT:
                    url_selected = permalink;
                    break;
                case LinkType.SEARCHPAGE:
                    break;
                case LinkType.WEB:
                    url_raw = row.url;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public static class LinkHelper
    {
        public static IEnumerable<Link> ToModel(this IEnumerable<link> rows)
        {
            foreach (var row in rows)
            {
                yield return new Link(row);
            }

        }
    }
}