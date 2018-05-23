using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Models.currency;
using tradelr.DataAccess;
using tradelr.DBML;
using tradelr.Models.google.analytics;
using tradelr.Models.transactions;

namespace tradelr.Models.subdomain
{
    public class Statistics : VisitorStatistics
    {
        // sales
        public string salesThisMonth { get; private set; }
        public int numberOfSalesThisMonth { get; private set; }
        public string salesUnpaidThisMonth { get; private set; }
        public int numberofSalesUnpaidThisMonth { get; private set; }
        public string salesThisYear { get; private set; }
        public int numberOfSalesThisYear { get; private set; }
        public string salesUnpaidThisYear { get; private set; }
        public int numberofSalesUnpaidThisYear { get; private set; }

        // products
        public long productTotal { get; set; }
        public long outOfStockTotal { get; set; }

        public Statistics(IEnumerable<VisitorStat> stats, DateTime startdate, DateTime enddate)
            :base(stats, startdate, enddate)
        {

        }

        public void InitSalesAndProductsStatistics(ITradelrRepository repository, long subdomainid, long viewerid, 
            MASTERsubdomain sd)
        {
            var date = DateTime.UtcNow;
            var currency = sd.currency.ToCurrency();
            // sales stats
            var invoices = repository.GetOrders(subdomainid, TransactionType.INVOICE, viewerid, null, null, null, false);
            var invoices_month = invoices.Where(x => x.created.Month == date.Month);
            var invoices_year = invoices.Where(x => x.created.Year == date.Year);
            numberOfSalesThisMonth = invoices_month.Count();
            numberOfSalesThisYear = invoices_year.Count();
            numberofSalesUnpaidThisMonth =
                invoices_month.Where(x => x.totalPaid != x.total + (x.shippingCost ?? 0)).Count();
            numberofSalesUnpaidThisYear = invoices_year.Where(x => x.totalPaid != x.total + (x.shippingCost ?? 0)).Count();
            salesThisMonth = string.Concat(currency.symbol,
                                           invoices_month.ToArray().Sum(x => x.total).ToString("n" + currency.decimalCount));
            salesThisYear = string.Concat(currency.symbol,
                                           invoices_year.ToArray().Sum(x => x.total).ToString("n" + currency.decimalCount));
            salesUnpaidThisMonth = string.Concat(currency.symbol,
                                                 invoices_month.Where(
                                                     x => x.totalPaid != x.total + (x.shippingCost ?? 0)).ToArray().Sum(
                                                         x => x.total + (x.shippingCost ?? 0) - x.totalPaid).ToString(
                                                             "n" + currency.decimalCount));
            salesUnpaidThisYear = string.Concat(currency.symbol,
                                                 invoices_year.Where(
                                                     x => x.totalPaid != x.total + (x.shippingCost ?? 0)).ToArray().Sum(
                                                         x => x.total + (x.shippingCost ?? 0) - x.totalPaid).ToString(
                                                             "n" + currency.decimalCount));

            // product stats
            productTotal = sd.total_products_mine;
            outOfStockTotal = sd.total_outofstock;
        }
    }
}