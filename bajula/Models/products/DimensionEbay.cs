using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using eBay.Service.Core.Soap;

namespace tradelr.Models.products
{
    public class DimensionEbay : Dimension
    {
        public MeasurementSystemCodeType measurementType { get; set; }
    }

    public static class DimensionEbayHelper
    {
        public static DimensionEbay ToDimensionEbay(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new DimensionEbay();
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<DimensionEbay>(value);
        }
    }
}