using System.Collections.Generic;
using System.Linq;

namespace tradelr.Common.Models.currency
{
    public class Currency
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
        public byte decimalCount { get; set; }
    }

    public static class CurrencyHelper
    {
        public static IEnumerable<Currency> GetCurrencies()
        {
            return Currencies.Values.Values;    
        }

        public static Currency ToCurrency(this int id)
        {
            return Currencies.Values[id];
        }

        public static Currency ToCurrency(this int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }
            return Currencies.Values[id.Value];
        }

        public static Currency ToCurrency(this string code)
        {
            return Currencies.Values.Values.SingleOrDefault(x => x.code == code);
        }

        public static string ToCurrencySymbol(this int? id)
        {
            if (!id.HasValue)
            {
                return "";
            }
            return Currencies.Values[id.Value].symbol;
        }

        public static string ToCurrencyCode(this int? id)
        {
            if (!id.HasValue)
            {
                return "";
            }
            return Currencies.Values[id.Value].code;
        }

        public static string ToCurrencyName(this int? id)
        {
            if (!id.HasValue)
            {
                return "";
            }
            return Currencies.Values[id.Value].name;
        }

        public static bool IsCurrencySupportedByPaypal(this string code)
        {
            switch (code)
            {
                case "AUD":
                case "BRL":
                case "CAD":
                case "CZK":
                case "DKK":
                case "EUR":
                case "HKD":
                case "HUF":
                case "ILS":
                case "JPY":
                case "MYR":
                case "MXN":
                case "NOK":
                case "NZD":
                case "PHP":
                case "PLN":
                case "GBP":
                case "SGD":
                case "SEK":
                case "CHF":
                case "TWD":
                case "THB":
                case "USD":
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsCurrencySupportedByGoogleCheckout(this string code)
        {
            switch (code)
            {
                case "USD":
                case "GDP":
                    return true;

                default:
                    return false;
            }
        }
    }
}
