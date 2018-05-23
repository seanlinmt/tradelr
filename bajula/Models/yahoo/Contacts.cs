using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.yahoo
{
    /// <summary>
    /// http://developer.yahoo.com/social/rest_api_guide/contacts-resource.html
    /// </summary>
    public class Contacts
    {
        public int start { get; set; }
        public int count { get; set; }
        public int total { get; set; }
        public string uri { get; set; }
        public List<Contact> contact { get; set; }
    }
}