using System.Collections.Generic;
using tradelr.Areas.dashboard.Models.orchard.media.models;

namespace tradelr.Areas.dashboard.Models.orchard.media.viewmodels {
    public class MediaFolderEditViewModel {
        public string FolderName { get; set; }
        public string MediaPath { get; set; }
        public IEnumerable<MediaFolder> MediaFolders { get; set; }
        public IEnumerable<MediaFile> MediaFiles { get; set; }
    }
}
