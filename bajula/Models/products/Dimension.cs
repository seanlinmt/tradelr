using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using tradelr.Common;
using tradelr.Library;

namespace tradelr.Models.products
{
    public class Dimension
    {
        public decimal weight { get; set; }
        public decimal height { get; set; }
        public decimal length { get; set; }
        public decimal width { get; set; }

        public Dimension()
        {
            
        }

        /// <summary>
        /// everything stored as metric in database
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="height"></param>
        /// <param name="length"></param>
        /// <param name="width"></param>
        /// <param name="isMetric"></param>
        public Dimension(decimal? weight, decimal? height, decimal? length, decimal? width, bool isMetric)
        {
            if (isMetric)
            {
                this.weight = weight ?? 0;
                this.height = height ?? 0;
                this.length = length ?? 0;
                this.width = width ?? 0;
            }
            else
            {
                this.weight = weight.HasValue ? weight.Value.ConvertWeight(true) : 0;
                this.height = height.HasValue ? height.Value.ConvertDistance(true) : 0;
                this.length = length.HasValue ? length.Value.ConvertDistance(true) : 0;
                this.width = width.HasValue ? width.Value.ConvertDistance(true) : 0;
            }
        }

        private void RoundValues()
        {
            weight = Decimal.Round(weight, 2);
            height = Decimal.Round(height, 2);
            length = Decimal.Round(length, 2);
            width = Decimal.Round(width, 2);
        }

        public void ToImperial()
        {
            weight = weight.ConvertWeight(false);
            height = height.ConvertDistance(false);
            length = length.ConvertDistance(false);
            width = width.ConvertDistance(false);

            RoundValues();
        }

        public void ToMetric()
        {
            weight = weight.ConvertWeight(true);
            height = height.ConvertDistance(true);
            length = length.ConvertDistance(true);
            width = width.ConvertDistance(true);

            RoundValues();
        }

        public string Serialize()
        {
            // round values first
            RoundValues();

            var serializer = new JavaScriptSerializer();
            return serializer.Serialize(this);
        }

        public static string GetDistanceUnit(bool isMetric)
        {
            if (isMetric)
            {
                return "cm";
            }
            return "in";
        }

        public static string GetWeightUnit(bool isMetric)
        {
            if (isMetric)
            {
                return "kg";
            }
            return "lb";
        }
    }

    public static class DimensionHelper
    {
        public static Dimension ToDimension(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return new Dimension();
            }
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<Dimension>(value);
        }

        /// <summary>
        /// converts between cm and in
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toMetric"></param>
        /// <returns></returns>
        public static decimal ConvertDistance(this decimal value, bool toMetric)
        {
            if (toMetric)
            {
                return 2.54m * value;
            }
            return 0.3937m * value;
        }

        /// <summary>
        /// converts between kg and lb
        /// </summary>
        /// <param name="value"></param>
        /// <param name="toMetric"></param>
        /// <returns></returns>
        public static decimal ConvertWeight(this decimal value, bool toMetric)
        {
            if (toMetric)
            {
                return 0.4536m * value;
            }
            return 2.2046m * value;
        }
    }
}