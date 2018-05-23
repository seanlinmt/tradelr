using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace tradelr.Models.google
{
    public enum GoogleItemType
    {
        [Description("Housing")]
        HOUSING,
        [Description("Jobs")]
        JOBS,
        [Description("Local Products")]
        LOCALPRODUCTS,
        [Description("Products")]
        PRODUCTS,
        [Description("Travel Packages")]
        TRAVELPACKAGES,
        [Description("Vehicles")]
        VEHICLES
    }
}