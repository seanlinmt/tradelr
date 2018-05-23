using System.Collections.Generic;

namespace tradelr.Models.admin
{
    public class Admin : BaseViewModel
    {
        public Admin(BaseViewModel viewmodel) : base(viewmodel)
        {
            registeredCountries = new List<string>();
        }

        public bool cacheTimer1Min { get; set; }
        public bool cacheTimer5Min { get; set; }
        public bool cacheTimer10Min { get; set; }
        public bool cacheTimer60Min { get; set; }
        public bool cacheTimer10Second { get; set; }
        public long mailQueueLength { get; set; }
        public long userCount { get; set; }
        public long creatorCount { get; set; }
        public long supportCount { get; set; }
        public long productCount { get; set; }
        public long orderCount { get; set; }
        public int sessionCount { get; set; }

        public List<string> registeredCountries { get; set; }
    }
}
