using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using tradelr.Libraries.compression;
using tradelr.Library.Constants;

namespace tradelr.Libraries.Loader
{
    public class CssLoader
    {
        private readonly string CSS_BASE_DIR = GeneralConstants.APP_ROOT_DIR + "/Content/css/";

        public static readonly CssLoader Instance = new CssLoader();

        public LoadedContent LoadFeatures(string xmlFile)
        {
            var csscontent = new LoadedContent();
            string xmlcontent = File.ReadAllText(string.Concat(CSS_BASE_DIR, xmlFile));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlcontent);
            XmlNodeList files = doc.GetElementsByTagName("files");
            StringBuilder sb = new StringBuilder();
            foreach (XmlElement file in files)
            {
                XmlNodeList links = file.GetElementsByTagName("link");
                foreach (XmlElement link in links)
                {
                    String source = link.Attributes["href"].Value;
                    string filename = string.Concat(CSS_BASE_DIR, source);
                    csscontent.filenames.Add(filename);
                    string content = File.ReadAllText(filename);
                    sb.Append(content);
                }
            }
#if DEBUG
            csscontent.content = sb.ToString();
#else
            csscontent.content = MyMin.parse(sb.ToString(), false, true);
#endif
            return csscontent;
        }
    }
}
