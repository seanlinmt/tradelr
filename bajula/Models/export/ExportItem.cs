using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Common.Models.currency;
using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Library.geo;
using tradelr.Models.google;
using tradelr.Models.products;
using tradelr.Models.subdomain;

namespace tradelr.Models.export
{
    public class ExportItem
    {
        protected long ProductId { get; set; }
        protected string Title { get; set; }
        protected string Description { get; set; }
        protected string Condition { get; set; }
        protected decimal? SellingPrice { get; set; }
        protected decimal? SpecialPrice { get; set; }
        protected string PaymentNotes { get; set; }
        public string[] PaymentMethods { get; set; }
        protected string hostName { get; set; }

        protected Currency Currency { get; set; }

        protected string LocationState { get; set; }
        protected Country Country { get; set; }

        // info for error handling
        public string ErrorMessage { get; set; }
        protected long? ownerid { get; set; }
        protected string producturl { get; set; }
        protected string productname { get; set; }
        protected long subdomainid { get; set; }

        public ExportItem(string hostname)
        {
            this.hostName = hostname;
        }

        protected void InitValues(product p, decimal exchange_rate, MASTERsubdomain sd = null)
        {
            productname = p.title;
            subdomainid = p.subdomainid;
            producturl = p.ToProductUrl();

            Title = p.title;
            Description = p.details;
            ProductId = p.id;
            Condition = "New";
            if (p.sellingPrice.HasValue)
            {
                SellingPrice = (p.specialPrice ?? p.sellingPrice).Value * exchange_rate;
            }

            if (p.sellingPrice.HasValue)
            {
                SellingPrice = p.tax.HasValue
                                           ? (p.sellingPrice.Value * (p.tax.Value / 100 + 1))
                                           : p.sellingPrice.Value;

                if (p.specialPrice.HasValue)
                {
                    // if has special price then original (strike-through) + special price
                    SpecialPrice = p.tax.HasValue
                                           ? (p.specialPrice.Value * (p.tax.Value / 100 + 1))
                                           : p.specialPrice.Value;
                }
            }

            if (sd == null)
            {
                sd = p.MASTERsubdomain;
            }

            Currency = sd.currency.ToCurrency();
            PaymentNotes = sd.paymentTerms;
            hostName = sd.ToHostName();
            LocationState = sd.organisation.state;
            Country = sd.organisation.country.ToCountry();
        }
    }
}