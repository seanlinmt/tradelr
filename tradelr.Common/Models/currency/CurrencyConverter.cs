using System;
using System.Collections.Generic;
using clearpixels.Logging;

namespace tradelr.Common.Models.currency
{
    public class CurrencyConverter
    {
        public static readonly CurrencyConverter Instance = new CurrencyConverter();
        private readonly Dictionary<string, decimal> rates;
        private CurrencyConverter()
        {
            rates = new Dictionary<string, decimal>();
        }

        public decimal GetRate(string source, string target)
        {
            // try from cache first
            var key = source + target;
            decimal rate;
            if (rates.TryGetValue(key, out rate))
            {
                return rate;
            }
            var converter = new Zayko.Finance.CurrencyConverter();
            try
            {
                var data = converter.GetCurrencyData(source, target);
                rate = (decimal)data.Rate;
                rates.Add(key, rate);
                return rate;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex.Message);
                throw;
            }
        }

        public decimal Convert(string source, string target, decimal amount)
        {
            // try from cache first
            var key = source + target;
            decimal rate;
            if (rates.TryGetValue(key, out rate))
            {
                return rate*amount;
            }

            var converter = new Zayko.Finance.CurrencyConverter();
            try
            {
                var data = converter.GetCurrencyData(source, target);
                rate = (decimal) data.Rate;
                rates.Add(key, rate);
                return rate * amount;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex.Message);
                throw;
            }
        }

    }
}
