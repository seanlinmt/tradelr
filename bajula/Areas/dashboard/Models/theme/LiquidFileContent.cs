using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using tradelr.Library.Constants;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class LiquidFileContent
    {
        public string filename { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        private const string TextContentWrapper = "<textarea id=\"file_content\">{0}</textarea>";

        private static readonly IEnumerable<string> ImageExtensions = new[] {".jpg", ".jpeg", ".gif", ".png"};

        public LiquidFileContent(string root, string path)
        {
            var file = new FileInfo(string.Format("{0}{1}{2}", GeneralConstants.APP_ROOT_DIR, root, path));
            filename = file.Name;
            url = path;

            var ext = Path.GetExtension(path);
            if (ext == null)
            {
                content = "<p class='ml20'>File cannot be viewed.</p>";
                return;
            }

            // determine if file is an image
            if (ImageExtensions.Contains(ext.ToLower()))
            {
                content = string.Format("<p class='ml20'><img src='{0}{1}'/></p>", root, path);
                return;
            }

            // open file for reading
            using (var reader = file.OpenText())
            {
                content = string.Format(TextContentWrapper, HttpUtility.HtmlEncode(reader.ReadToEnd()));
            }
        }
    }
}