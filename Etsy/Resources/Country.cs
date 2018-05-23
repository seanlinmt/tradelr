using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Etsy.Resources
{
    public class Country
    {
        public int country_id { get; set; }
        public string iso_country_code { get; set; }
        public string world_bank_country_code { get; set; }
        public string name { get; set; }
        public decimal? lat { get; set; }
        public decimal? lon { get; set; }

    }
}
