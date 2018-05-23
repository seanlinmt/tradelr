using System;
using System.Collections.Generic;

namespace tradelr.Models.review
{
    public class Review
    {
        public string comment { get; set; }
        public DateTime created { get; set; }

        public byte overall { get; set; }
        public byte willshopagain { get; set; }
        public byte delivery { get; set; }
        public byte support { get; set; }

        public string response { get; set; }
    }

    public static class ReviewHelper
    {
        public static IEnumerable<Review> ToModel(this IEnumerable<DBML.review> values)
        {
            foreach (var value in values)
            {
                yield return new Review()
                                 {
                                     created = value.created,
                                     comment = value.comment,
                                     overall = value.rating_overall,
                                     willshopagain = value.rating_willshopagain,
                                     delivery = value.rating_delivery,
                                     support = value.rating_support
                                 };
            }
        }
    }
}