using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CustomBuildTasks
{
    /// <summary>
    /// http://erikzaadi.com/blog/2010/03/05/CompressJavascriptWithGoogleClosureCompilerInVisualStudiowithJQuerySupport.xhtml
    /// </summary>
    public class Closure : Task
    {
        [Required]
        public string Modules { get; set; }

        private const string PhysicalPath = @"C:\code\tradelr\bajula\bajula\";

        [Required]
        public string OutputName { get; set; }

        public override bool Execute()
        {
            string[] modules = Modules.Split(new[]{","}, StringSplitOptions.RemoveEmptyEntries);
            var sb = new StringBuilder();
            var loader = new JsLoader(PhysicalPath);
            foreach (var module in modules)
            {
                sb.Append(loader.LoadFeatures(module));
            }
            var path = PhysicalPath + "/Scripts/" + OutputName;
            using (var output = File.CreateText(path))
            {
                output.Write(sb.ToString());
            }
//#if DEBUG
            // just create a .min.js version that's not compressed
//            path = path.Replace(".js", ".min.js");
//            using (var output = File.CreateText(path))
//            {
//                output.Write(sb.ToString());
//            }
//#else
            Compress(path);
//#endif
            return true;
        }

        private void Compress(string path)
        {
            try
            {
                string[] warnings;
                string oldFile = path;
                string newFile = oldFile.Replace(".js", ".min.js");
                using (var process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = @"C:\Program Files\Java\jdk1.6.0_26\bin\java.exe",
                        Arguments = String.Format(@"-jar ""{0}"" --js {1} --js_output_file {2} --compilation_level {3} --summary_detail_level {4}",
                        @"C:\code\tradelr\bajula\CustomBuildTasks\jar\compiler.jar",
                        oldFile,
                        newFile,
                        "SIMPLE_OPTIMIZATIONS",
                        "3"),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };
                    process.Start();
                    warnings = process.StandardError.ReadToEnd()
                        .Replace("\r", String.Empty)
                        .Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
                    process.WaitForExit(5000);
                }

                foreach (string warning in warnings)
                    Log.LogWarning(null, null, null, oldFile, 1, 1, 1, 1, FormatWarning(warning), null);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
            }
        }

        private string FormatWarning(string warning)
        {
            return warning
                .Trim()
                .Replace("[WARNING] ", String.Empty);
        }
    }
}
