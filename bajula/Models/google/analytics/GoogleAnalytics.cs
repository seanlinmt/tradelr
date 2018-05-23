using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Google.GData.Analytics;
using tradelr.Library.Caching.SimpleCache;
using clearpixels.Logging;

namespace tradelr.Models.google.analytics
{
    public class GoogleAnalytics
    {
        private const string BaseUrl = "https://www.google.com/analytics/feeds/data";
        private const string Username = "tradelr.com@gmail.com";
        private const string Pasword = "tuaki1976";
#if DEBUG
        private const string TableId = "ga:24381606"; // stats for all sites including account holders
#else
        private const string TableId = "ga:38907631"; // stats for account holders only
#endif

        private DataFeed feed;

        public string FeedTitle { get; set; }
        public string FeedID { get; set; }
        public int TotalResults { get; set; }
        public int StartIndex { get; set; }
        public int ItemsPerPage { get; set; }

        public List<VisitorStat> stats { get; set; }

        private readonly AnalyticsService service;

        public GoogleAnalytics()
        {
            service = new AnalyticsService("tradelr");
            service.setUserCredentials(Username, Pasword);
            stats = new List<VisitorStat>();
        }

        public void GetVisitorStatistics(string hostname, DateTime startDate, DateTime endDate)
        {
            var query = new DataQuery(BaseUrl)
                            {
                                Ids = TableId,
                                Dimensions = "ga:date,ga:referralPath,ga:source,ga:keyword,ga:country",
                                Metrics = "ga:visits,ga:pageviews,ga:timeOnSite",
                                Segment = string.Format("dynamic::ga:hostname=={0}", hostname),
                                Sort = "ga:date",
                                GAStartDate = startDate.ToString("yyyy-MM-dd"),
                                GAEndDate = endDate.ToString("yyyy-MM-dd")
                            };

            Uri url = query.Uri;
            
            try
            {
                // try to get from the cache first
                var cachekey = string.Concat(hostname, startDate.ToShortDateString(), endDate.ToShortDateString());
                var data = SimpleCache.Get(cachekey, SimpleCacheType.GOOGLE_ANALYTICS);
                if (data == null)
                {
                    data = service.Query(query);
                    SimpleCache.Add(cachekey, data, SimpleCacheType.GOOGLE_ANALYTICS, DateTime.UtcNow.AddDays(1));
                }

                feed = (DataFeed)data;
                
                FeedTitle = feed.Title.Text;
                FeedID = feed.Id.Uri.Content;
                TotalResults = feed.TotalResults;
                StartIndex = feed.StartIndex;
                ItemsPerPage = feed.ItemsPerPage;

                foreach (DataEntry entry in feed.Entries)
                {
                    var stat = new VisitorStat();
                    foreach (var dimension in entry.Dimensions)
                    {
                        switch (dimension.Name)
                        {
                            case "ga:country":
                                stat.country = dimension.Value;
                                break;
                            case "ga:date":
                                stat.visitDate = DateTime.ParseExact(dimension.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                                break;
                            case "ga:referralPath":
                                stat.referrerPath = dimension.Value;
                                break;
                            case "ga:source":
                                stat.referrerHostname = dimension.Value;
                                break;
                            case "ga:keyword":
                                stat.keyword = dimension.Value;
                                break;
                        }
                    }
                    foreach (var metric in entry.Metrics)
                    {
                        switch (metric.Name)
                        {
                            case "ga:visits":
                                stat.visitCount = metric.IntegerValue;
                                break;
                            case "ga:pageviews":
                                stat.pageViews = metric.IntegerValue;
                                break;
                            case "ga:timeOnSite":
                                stat.timeOnSite = metric.FloatValue;
                                break;
                        }
                    }
                    stats.Add(stat);
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                Syslog.Write(string.Concat(ex.Message, ":", url.ToString()));
            }
        }
    }
}