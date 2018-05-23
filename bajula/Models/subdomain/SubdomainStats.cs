using System.Collections.Generic;
using tradelr.DBML;
using tradelr.Models.google.analytics;

namespace tradelr.Models.subdomain
{
    public class SubdomainStats
    {
        public long products_mine { get; set; }
        public long total_orders { get; set; }
        public long orders_sent { get; set; }
        public long orders_received { get; set; }
        public long total_invoices { get; set; }
        public long invoices_sent { get; set; }
        public long invoices_received { get; set; }
        public long total_contacts { get; set; }
        public long contacts_public { get; set; }
        public long contacts_private { get; set; }
        public long contacts_staff { get; set; }
        public long total_outofstock { get; set; }
    }

    public static class SubdomainStatsHelper
    {
        public static SubdomainStats ToSubdomainStats(this MASTERsubdomain value)
        {
            return new SubdomainStats()
                       {
                           products_mine = value.total_products_mine,
                           contacts_private = value.total_contacts_private,
                           contacts_public = value.total_contacts_public,
                           contacts_staff = value.total_contacts_staff,
                           orders_received = value.total_orders_received,
                           orders_sent = value.total_orders_sent,
                           invoices_received = value.total_invoices_received,
                           invoices_sent = value.total_invoices_sent,
                           total_outofstock = value.total_outofstock,
                           total_contacts = value.total_contacts_private + value.total_contacts_public + value.total_contacts_staff,
                           total_invoices = value.total_invoices_received + value.total_invoices_sent,
                           total_orders = value.total_orders_received + value.total_orders_sent
                       };
        }
    }
}
