namespace clearpixels.Facebook.Resources
{
    public class Photo
    {
        public string id { get; set; }
        public IdName from { get; set; }
        public string source { get; set; }
        public string name { get; set; }
        public PhotoProperty[] images { get; set; }
    }

    public class PhotoProperty
    {
        public string height { get; set; }
        public string width { get; set; }
        public string source { get; set; }
    }
}
