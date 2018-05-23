using System;

namespace tradelr.Models.time
{
    public enum TimeLine
    {
        Last24Hours,
        LastWeek,
        LastMonth,
        LastQuarter,
        LastYear,
        Last5Years
    }

    public static class TimeLineHelper
    {
        public static DateTime ToPointInTime(this TimeLine timeline)
        {
            DateTime result = DateTime.UtcNow;
            switch (timeline)
            {
                case TimeLine.Last24Hours:
                    result = result.AddHours(-24);
                    break;
                case TimeLine.Last5Years:
                    result = result.AddYears(-5);
                    break;
                case TimeLine.LastMonth:
                    result = result.AddMonths(-1);
                    break;
                case TimeLine.LastQuarter:
                    result = result.AddMonths(-4);
                    break;
                case TimeLine.LastWeek:
                    result = result.AddDays(-7);
                    break;
                case TimeLine.LastYear:
                    result = result.AddYears(-1);
                    break;
            }
            return result;
        }
    }
}
