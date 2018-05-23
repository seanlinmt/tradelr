using System.Collections.Generic;
using System.Web.Mvc;

namespace tradelr.Areas.dashboard.Models.store.page
{
    public class Page
    {
        public string id { get; set; }
        public string title { get; set; }
        public string pageUrl {get; set; }
        public string permalink { get; set; }
        public string content { get; set; }
        public string updated { get; set; }
        public bool visible { get; set; }
        public IEnumerable<SelectListItem> templateList { get; set; }
        public bool editMode { get; set; }
    }
}