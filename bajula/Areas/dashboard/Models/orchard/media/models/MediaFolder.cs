using System;

namespace tradelr.Areas.dashboard.Models.orchard.media.models { 
    public class MediaFolder {
        public string Name { get; set; }
        public string MediaPath { get; set; }
        public long Size { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
