using System;

namespace tradelr.Time
{
    public static class TimeUtil
    {
        public static DateTime GetDateTime(string date, string format)
        {
            DateTime endDate = DateTime.ParseExact(date,
                                                   format, System.Globalization.CultureInfo.InvariantCulture);
            return endDate;
        }
    }
}
