using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.yahoo
{
    public class Contact
    {
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public string uri { get; set; }
        public bool isConnection { get; set; }
        public int id { get; set; }
        public List<Field> fields { get; set; }
        public List<Category> categories { get; set; }
    }
}