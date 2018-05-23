using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tradelr.DataAccess;
using tradelr.DBML;

namespace tradelr.Models.google
{
    public class GoogleMapData
    {
        public decimal latitude { get; set; }
        public decimal longtitude { get; set; }
        public int mapZoom { get; set; }
        public int? country { get; set; }
        public string countryData { get; set; }
        public SelectList countryList { get; set; }

        public long orgid { get; set; }
    }

    public static class GoogleMapHelper
    {
        public static GoogleMapData ToGoogleMap(this organisation row)
        {
            return new GoogleMapData
                       {
                           latitude = row.latitude.HasValue ? row.latitude.Value : 0,
                           longtitude = row.longtitude.HasValue ? row.longtitude.Value : 0,
                           mapZoom = row.zoom.HasValue ? row.zoom.Value : 0,
                           country = row.country,
                           orgid = row.id
                       };
        }
    }
}
