using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using tradelr.DBML;
using tradelr.Library;

namespace tradelr.Areas.dashboard.Models.store.navigation
{
    public class LinkList
    {
        public long id { get; set; } // new = 0
        public string title { get; set; }
        public string handle { get; set; }
        public IEnumerable<Link> links { get; set; }
        public bool permanent { get; set; }

        // used by the spare link entry
        public static IEnumerable<SelectListItem> referenceLinkTypeList = ((LinkType[])
                                                                           Enum.GetValues(typeof (LinkType)))
            .Select(x => new SelectListItem() {Text = x.ToDescriptionString(), Value = x.ToString()});

        public LinkList()
        {
            links = Enumerable.Empty<Link>();
        }
    }

    public static class LinkListHelper
    {
        public static LinkList ToModel(this linklist row)
        {
            return new LinkList()
                       {
                           handle = row.permalink,
                           id = row.id,
                           links = row.links.ToModel(),
                           permanent = row.permanent,
                           title = row.title
                       };
        }

        public static IEnumerable<LinkList> ToModel(this IEnumerable<linklist> rows)
        {
            foreach (var row in rows)
            {
                yield return row.ToModel();
            }
        }
    }
}