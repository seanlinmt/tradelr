using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Models.contacts
{
    public class ContactBasic
    {
        public long? id { get; set; } // need to be nullable, otherwise add/edit contact form will be messed up
        public string email { get; set; }
        public string address { get; set; }
        public string companyName { get; set; }
        public string countryName { get; set; }
        public string state { get; set; }

        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string profileThumbnail { get; set; }
        public string phone { get; set; }
    }
}