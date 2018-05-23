using System.Collections.Generic;
using tradelr.Areas.dashboard.Models.orchard.media.models;

namespace tradelr.Areas.dashboard.Models.orchard.media.viewmodels {
    public class MediaFolderIndexViewModel {
        public IEnumerable<MediaFolder> MediaFolders { get; set; }
    }
}
