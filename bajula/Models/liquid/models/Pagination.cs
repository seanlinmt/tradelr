using System.Collections.Generic;
using DotLiquid;

namespace tradelr.Models.liquid.models
{
    public class TitleUrl : Drop
    {
        public bool is_link { get; set; }
        public string title { get; set; }
        public string url { get; set; }
    }

    public class Pagination : Drop
    {
        public int page_size { get; set; }
        public int current_page { get; set; }
        public int current_offset { get; set; }
        public int pages { get; set; }
        public int items { get; set; }
        public TitleUrl previous { get; set; }
        public TitleUrl next { get; set; }
        public List<TitleUrl> parts { get; set; }

        public Pagination()
        {
            parts = new List<TitleUrl>();
        }
    }
}