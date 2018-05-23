namespace tradelr.Models.liquid.filters
{
    public static class MoneyFilter
    {
        public static string money_with_currency(decimal? input, Common.Models.currency.Currency currency)
        {
            if (!input.HasValue)
            {
                return "&nbsp;";
            }
            return string.Format("{0}{1} {2}", currency.symbol, input.Value.ToString("n" + currency.decimalCount), currency.code);
        }

        public static string money_without_currency(decimal? input, Common.Models.currency.Currency currency)
        {
            if (!input.HasValue)
            {
                return "&nbsp;";
            }
            return string.Format("{0}", input.Value.ToString("n" + currency.decimalCount));
        }

        public static string money(decimal? input, Common.Models.currency.Currency currency)
        {
            if (!input.HasValue)
            {
                return "&nbsp;";
            }
            return string.Format("{0}{1}", currency.symbol, input.Value.ToString("n" + currency.decimalCount));
        }

    }
}