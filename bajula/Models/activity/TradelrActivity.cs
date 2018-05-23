using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tradelr.Libraries;

namespace tradelr.Models.activity
{
    public class TradelrActivity
    {
        public IEnumerable<FilterBoxListInfo> filterList { get; set; }
        public IEnumerable<Activity> activities { get; set; }
    }
}