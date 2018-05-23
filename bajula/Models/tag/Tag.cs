using System;
using System.Collections.Generic;
using System.Linq;

namespace tradelr.Models.tag
{
    public class Tag
    {
        public string name { get; set; }
        public string classname { get; set; }
    }

    public static class TagHelper
    {
        public static SortedList<string, Tag> ToModel(this IEnumerable<DBML.tag> values)
        {
            var grouped = values.GroupBy(x => x.name);
            var tags = new SortedList<string, Tag>();
            if (grouped.Count() != 0)
            {
                var max = grouped.Max(x => x.Count());
                var min = grouped.Min(x => x.Count());
                foreach (var entry in grouped)
                {
                    // skip tags with no count
                    if (entry.Count() == 0)
                    {
                        continue;
                    }
                    var tag = new Tag()
                    {
                        classname = entry.Count().ToClassName(max, min),
                        name = entry.Key
                    };
                    tags.Add(tag.name, tag);
                }
            }

            return tags;
        }

        public static string ToClassName(this int tagcount, int tagMax, int tagMin)
        {
            // http://en.wikipedia.org/wiki/Tag_cloud
            if (tagcount <= tagMin)
            {
                return "tag_smaller";
            }
            var weight = (double)100 * (tagcount - tagMin) / (tagMax - tagMin);
            if (weight >= 80)
                return "tag_larger";

            if (weight >= 50)
                return "tag_large";

            if (weight >= 20)
                return "tag_normal";

            if (weight >= 10)
                return "tag_small";

            return "tag_smaller";
        }
    }
}