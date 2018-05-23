using System.Collections.Generic;

namespace clearpixels.Facebook.Resources
{
    public class User
    {
        public string id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string link { get; set; }
        public string picture { get; set; }
        public string about { get; set; }
        public string birthday { get; set; }
        public List<Work> work { get; set; }
        public List<Education> education { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public List<IdName> hometown { get; set; }
        public List<IdName> location { get; set; }
        public string bio { get; set; }
        public string gender { get; set; }
        public string quotes { get; set; }
        public List<string> interested_in { get; set; }
        public List<string> meeting_for { get; set; }
        public string relationship_status { get; set; }
        public string religion { get; set; }
        public string political { get; set; }
        public bool verified { get; set; }
        public IdName significant_other { get; set; }
        public double timezone { get; set; }
        public string third_party_id { get; set; }
        public string updated_time { get; set; }
        public string locale { get; set; }
    }
}
