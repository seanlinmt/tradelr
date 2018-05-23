using System.Collections.Generic;
using System.Linq;

namespace tradelr.Models.review
{
    public class ReviewSummary
    {
        public string overall_average { get; set; }
        public int overall_1 { get; set; }
        public int overall_2 { get; set; }
        public int overall_3 { get; set; }
        public int overall_4 { get; set; }
        public int overall_5 { get; set; }
        
        public string willshopagain_average { get; set; }
        public int willshopagain_1 { get; set; }
        public int willshopagain_2 { get; set; }
        public int willshopagain_3 { get; set; }
        public int willshopagain_4 { get; set; }
        public int willshopagain_5 { get; set; }
        
        public string delivery_average { get; set; }
        public int delivery_1 { get; set; }
        public int delivery_2 { get; set; }
        public int delivery_3 { get; set; }
        public int delivery_4 { get; set; }
        public int delivery_5 { get; set; }
        
        public string support_average { get; set; }
        public int support_1 { get; set; }
        public int support_2 { get; set; }
        public int support_3 { get; set; }
        public int support_4 { get; set; }
        public int support_5 { get; set; }
        
        public IEnumerable<Review> reviews { get; set; }

        public ReviewSummary(IEnumerable<Review> reviews)
        {
            this.reviews = reviews;
            if (reviews.Count() != 0)
            {
                overall_average = reviews.Average(x => x.overall).ToString("N2");
                delivery_average = reviews.Average(x => x.delivery).ToString("N2");
                support_average = reviews.Average(x => x.support).ToString("N2");
                willshopagain_average = reviews.Average(x => x.willshopagain).ToString("N2");

                support_1 = reviews.Where(x => x.support == 1).Count();
                support_2 = reviews.Where(x => x.support == 2).Count();
                support_3 = reviews.Where(x => x.support == 3).Count();
                support_4 = reviews.Where(x => x.support == 4).Count();
                support_5 = reviews.Where(x => x.support == 5).Count();

                delivery_1 = reviews.Where(x => x.delivery == 1).Count();
                delivery_2 = reviews.Where(x => x.delivery == 2).Count();
                delivery_3 = reviews.Where(x => x.delivery == 3).Count();
                delivery_4 = reviews.Where(x => x.delivery == 4).Count();
                delivery_5 = reviews.Where(x => x.delivery == 5).Count();

                overall_1 = reviews.Where(x => x.overall == 1).Count();
                overall_2 = reviews.Where(x => x.overall == 2).Count();
                overall_3 = reviews.Where(x => x.overall == 3).Count();
                overall_4 = reviews.Where(x => x.overall == 4).Count();
                overall_5 = reviews.Where(x => x.overall == 5).Count();

                willshopagain_1 = reviews.Where(x => x.willshopagain == 1).Count();
                willshopagain_2 = reviews.Where(x => x.willshopagain == 2).Count();
                willshopagain_3 = reviews.Where(x => x.willshopagain == 3).Count();
                willshopagain_4 = reviews.Where(x => x.willshopagain == 4).Count();
                willshopagain_5 = reviews.Where(x => x.willshopagain == 5).Count();
            }
        }
    }

    public static class ReviewSummaryHelper
    {
        public static ReviewSummary ToSummary(this IEnumerable<Review> reviews)
        {
            return new ReviewSummary(reviews);
        }
    }
}