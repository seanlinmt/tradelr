using System;
using System.Linq;
using tradelr.Models.account;

namespace tradelr.DBML
{
    public partial class theme
    {
        partial void OnCreated()
        {
            var time = DateTime.UtcNow.Ticks.ToString("x");
            theme_version = time;
            theme_mobile_version = time;
        }
    }
}
