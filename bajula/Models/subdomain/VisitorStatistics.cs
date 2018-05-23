using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using tradelr.Models.google.analytics;

namespace tradelr.Models.subdomain
{
    public class VisitorStatistics
    {
        private const int REFERRERCOUNT = 6;

        private Dictionary<DateTime, int> visitGraphData { get; set; } // for graph
        public string graphdata { get; set; }
        public string graphticksdata { get; set; }
        public int pageViews { get; set; }
        public decimal pagesPerVisit { get; set; }
        public int visitorsTotal { get; set; }
        public int visitorsPerDay { get; set; }
        public string averageTimeSpent { get; set; }

        public string referrerStats { get; set; }
        public string trafficOverviewStats { get; set; }
        public string searchKeywordStats { get; set; }
        public string countriesStats { get; set; }

        private int directTraffic { get; set; }
        private int referrerTraffic { get; set; }
        private int googleTraffic { get; set; }

        public VisitorStatistics(IEnumerable<VisitorStat> stats, DateTime date_start, DateTime date_end)
        {
            pageViews = stats.Sum(x => x.pageViews);
            visitorsTotal = stats.Sum(x => x.visitCount);
            if (pageViews > 0 && visitorsTotal > 0)
            {
                pagesPerVisit = Decimal.Round((decimal)pageViews/visitorsTotal,1);
                visitorsPerDay = visitorsTotal / ((date_end - date_start).Days);

                var timespent = new TimeSpan(0, 0, (int)Math.Round(stats.Sum(x => x.timeOnSite) / visitorsTotal));
                if (timespent.Minutes < 1)
                {
                    averageTimeSpent = string.Format("<strong>{0}</strong> seconds", timespent.Seconds);
                }
                else
                {
                    averageTimeSpent = string.Format("<strong>{0}</strong> minutes <strong>{1}</strong> seconds",
                                                     timespent.Minutes, timespent.Seconds);
                }
            }
            else
            {
                averageTimeSpent = "NO DATA";
            }

            var referrer = new Dictionary<string, int>();
            var keywords = new Dictionary<string, int>();
            var countries = new Dictionary<string, int>();
            visitGraphData = new Dictionary<DateTime, int>();
            foreach (var stat in stats)
            {
                var visits = stat.visitCount;

                // graph
                int visitCount;
                if (!visitGraphData.TryGetValue(stat.visitDate, out visitCount))
                {
                    visitGraphData.Add(stat.visitDate, visits);
                }
                else
                {
                    visitGraphData[stat.visitDate] = visits + visitCount;
                }

                // traffic
                if (stat.referrerHostname.IndexOf("direct") != -1)
                {
                    directTraffic += visits;
                }
                else if (stat.referrerHostname.IndexOf("google") != -1)
                {
                    googleTraffic += visits;
                }
                else
                {
                    referrerTraffic += visits;
                }

                // referrer
                if (stat.referrerPath.IndexOf("not set") == -1)
                {
                    var refererPath = string.Format("<a target='_blank' href='http://{0}{1}'>{0}{1}</a>", stat.referrerHostname, stat.referrerPath);
                    int referrerCount;
                    if (!referrer.TryGetValue(refererPath, out referrerCount))
                    {
                        referrer.Add(refererPath, visits);
                    }
                    else
                    {
                        referrer[refererPath] = referrerCount + visits;
                    }
                }

                // countries
                int countryCount;
                if (!countries.TryGetValue(stat.country, out countryCount))
                {
                    countries.Add(stat.country, visits);
                }
                else
                {
                    countries[stat.country] = countryCount + visits;
                }

                // keyword
                if (stat.keyword.IndexOf("not set") == -1)
                {
                    int keywordCount;
                    if (!keywords.TryGetValue(stat.keyword, out keywordCount))
                    {
                        keywords.Add(stat.keyword, visits);
                    }
                    else
                    {
                        keywords[stat.keyword] = keywordCount + visits;
                    }
                }
            }

            // process graph data
            var sb_graph = new StringBuilder();
            var sb_graph_ticks = new StringBuilder();
            sb_graph_ticks.Append("[");
            sb_graph.Append("[");
            date_start = new DateTime(date_start.Year, date_start.Month, date_start.Day);
            for (int i = 0; i < (date_end - date_start).Days; i++)
            {
                var date = date_start.AddDays(i);
                int visits;
                if (!visitGraphData.TryGetValue(date, out visits))
                {
                    visits = 0;
                }
                if (i != 0)
                {
                    sb_graph_ticks.Append(",");
                    sb_graph.Append(",");
                }
                sb_graph_ticks.AppendFormat("\"{0}\"", date.ToString("ddd dd MMM"));
                sb_graph.AppendFormat("[{0}, {1}]", i, visits);
            }
            sb_graph_ticks.Append("]");
            sb_graph.Append("]");
            graphdata = sb_graph.ToString();
            graphticksdata = sb_graph_ticks.ToString();

            // process referrers
            var sb_referrer = new StringBuilder();
            sb_referrer.Append("<table>");
            foreach (var entry in referrer.OrderByDescending(x => x.Value).Take(REFERRERCOUNT))
            {
                sb_referrer.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", entry.Value, entry.Key);
            }
            sb_referrer.Append("</table>");
            referrerStats = sb_referrer.ToString();

            // process trafficOverview
            var sb_traffic = new StringBuilder();
            var trafficTotal = directTraffic + referrerTraffic + googleTraffic;
            sb_traffic.Append("<table>");
            sb_traffic.AppendFormat("<tr><td>{0}%</td><td>Direct</td></tr>", directTraffic == 0? 0: Math.Round(directTraffic*100.0/trafficTotal,2));
            sb_traffic.AppendFormat("<tr><td>{0}%</td><td>Referring Sites</td></tr>", referrerTraffic == 0 ? 0: Math.Round(referrerTraffic * 100.0 / trafficTotal, 2));
            sb_traffic.AppendFormat("<tr><td>{0}%</td><td>Google</td></tr>", googleTraffic == 0 ? 0: Math.Round(googleTraffic * 100.0 / trafficTotal, 2));
            sb_traffic.Append("</table>");
            trafficOverviewStats = sb_traffic.ToString();

            var count = 0;
            // procces countries
            var sb_countries = new StringBuilder();
            var countriesTotal = countries.Sum(x => x.Value);
            sb_countries.Append("<table>");
            foreach (var entry in countries.OrderByDescending(x => x.Value))
            {
                sb_countries.AppendFormat("<tr><td>{0}%</td><td>{1}</td></tr>", Math.Round(entry.Value*100.0/countriesTotal,2), entry.Key);
                if (count++ == 5)
                {
                    break;
                }
            }
            sb_countries.Append("</table>");
            countriesStats = sb_countries.ToString();

            // process keyword
            var sb_keyword = new StringBuilder();
            sb_keyword.Append("<table>");
            count = 0;
            foreach (var entry in keywords.OrderByDescending(x => x.Value))
            {
                sb_keyword.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", entry.Value, entry.Key);
                if (count++ == 5)
                {
                    break;
                }
            }
            sb_keyword.Append("</table>");
            searchKeywordStats = sb_keyword.ToString();
        }
    }
}