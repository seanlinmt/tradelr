namespace tradelr.Areas.dashboard.Models.orchard.media.viewmodels {
    public class MediaItemAddViewModel {
        public MediaItemAddViewModel() {
            ExtractZip = true;
        }

        public string FolderName { get; set; }
        public string MediaPath { get; set; }
        public bool ExtractZip { get; set; }
    }
}
