using System;

namespace tradelr.Areas.dashboard.Models.orchard.media.models {
    public class MediaFile {
        public string Name { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string FolderName { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
