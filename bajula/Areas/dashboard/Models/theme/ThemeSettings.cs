using System.Collections.Generic;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeSettings
    {
        public Dictionary<string, object> presets { get; set; }
        public dynamic current { get; set; }

        public ThemeSettings()
        {
            presets = new Dictionary<string, object>();
        }
    }
}