using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.store.policies
{
    public class PolicySettings
    {
        // payment
        public string paymentTerms { get; set; }
        public string refundPolicy { get; set; }

    }
}