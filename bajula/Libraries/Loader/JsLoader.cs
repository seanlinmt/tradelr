using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using tradelr.Libraries.compression;
using tradelr.Library.Constants;

namespace tradelr.Libraries.Loader
{
    public class JsLoader
    {
        private readonly string JAVASCRIPT_BASE_DIR = GeneralConstants.APP_ROOT_DIR + "/Scripts/";
        
        public static readonly JsLoader Instance = new JsLoader();

        public LoadedContent LoadFeatures(string featurePath, string xmlFile)
        {
            var jscontent = new LoadedContent();
            string xmlcontent = File.ReadAllText(string.Concat(JAVASCRIPT_BASE_DIR, featurePath, "/", xmlFile));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlcontent);
            XmlNodeList files = doc.GetElementsByTagName("files");
            StringBuilder sb = new StringBuilder();
            foreach (XmlElement file in files)
            {
                XmlNodeList libraries = file.GetElementsByTagName("script");
                foreach (XmlElement script in libraries)
                {
                    String source = script.Attributes["src"].Value;
                    string filename = string.Concat(JAVASCRIPT_BASE_DIR, featurePath, "/", source);
                    jscontent.filenames.Add(filename);
                    string content = File.ReadAllText(filename);
                    sb.Append(content);
                }
            }
#if DEBUG
            jscontent.content = sb.ToString();
#else
            // can't use closure at the moment because it messes up jqgrid
            jscontent.content = MyMin.parse(sb.ToString(), false, false);
#endif
            return jscontent;
        }

        public string LoadViewJavascript(string path)
        {
            var jslocation = string.Concat(JAVASCRIPT_BASE_DIR, path, ".js");
            string jscontent = File.ReadAllText(jslocation);
#if DEBUG
            return jscontent;
#else
            //string minified = compress.getJSMachine(jscontent);
            string minified = MyMin.parse(jscontent, false, false);
            return minified;
#endif
        }
    }
}