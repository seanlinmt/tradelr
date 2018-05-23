using System;
using System.IO;
using System.Text;
using System.Xml;

namespace CustomBuildTasks
{
    public class JsLoader
    {
        private readonly string javascriptBaseDir;

        public JsLoader(string projectdir)
        {
            javascriptBaseDir = projectdir + "/Scripts/";
        }

        public string LoadFeatures(string featurePath)
        {
            string xmlcontent = File.ReadAllText(string.Concat(javascriptBaseDir, featurePath, "/files.xml"));
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
                    string filename = string.Concat(javascriptBaseDir, featurePath, "/", source);
                    string content = File.ReadAllText(filename);
                    sb.Append(content);
                }
            }
            return sb.ToString();
        }
    }
}