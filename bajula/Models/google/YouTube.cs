using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using clearpixels.Logging;

namespace tradelr.Models.google
{
    public class YouTube
    {
        private Uri url { get; set; }
        public string id { get; set; }

        public YouTube(string videourl)
        {
            try
            {
                url = new Uri(videourl);
            }
            catch (Exception ex)
            {
                Syslog.Write(string.Format("Failed to parse youtube url {0}", videourl));
            }
            
            if (url.Host == "www.youtube.com")
            {
                var parsed = HttpUtility.ParseQueryString(url.Query);
                id = parsed["v"];    
            }
        }

        public string GetExternalThumbnailUrl()
        {
            if (string.IsNullOrEmpty(id))
            {
                return "";
            }
            return string.Format("http://img.youtube.com/vi/{0}/hqdefault.jpg", id);
        }

    }
}