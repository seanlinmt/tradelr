using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace tradelr.Areas.dashboard.Models.orchard.media.viewmodels {
    public class MediaFolderCreateViewModel {
        [Required, DisplayName("Folder Name:")]
        public string Name { get; set; }
        public string MediaPath { get; set; }
    }
}
