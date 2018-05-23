using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using clearpixels.Logging;

namespace tradelr.Libraries.file
{
    public static class UtilFile
    {
        public static void CopyDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                file.CopyTo(Path.Combine(destination.FullName,
                    file.Name));
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);

                try
                {
                    File.Delete(file);
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                    GC.Collect();
                    File.Delete(file);
                }
                
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        /// <summary>
        /// Returns directory including subdirectories totalsize in MBs
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static double GetDirectorySize(string folderPath)
        {
            long totalsize = 0;
            var dir = new DirectoryInfo(folderPath);
            if (dir.Exists)
            {
                totalsize = dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length);
            }

            return Math.Round(totalsize / (double)1000000, 2);
        }

    }
}