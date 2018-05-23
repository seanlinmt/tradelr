using System;
using System.IO;
using clearpixels.Logging;
using tradelr.Library.Constants;

namespace tradelr.Library.files
{
    public class FileHandler
    {
        private string filename { get; set; }
        private string url { get; set; }
        private long size { get; set; }
        private UploadFileType filetype { get; set; }

        public FileHandler(string filename, UploadFileType type, string uniqueid)
        {
            this.filename = filename;
            string folder = "";
            filetype = type;
            switch (type)
            {
                case UploadFileType.DIGITAL:
                    folder = string.Format("/Uploads/files/{0}/digital", uniqueid);
                    break;
                case UploadFileType.IMAGE:
                    folder = string.Format("/Uploads/files/{0}/images", uniqueid);
                    break;
                case UploadFileType.MOBILE_THEME:
                    folder = string.Format("/Uploads/files/{0}/mobile_theme", uniqueid);
                    break;
                case UploadFileType.THEME:
                    folder = string.Format("/Uploads/files/{0}/theme", uniqueid);
                    break;
            }

            // create folder if it doesn't exist
            if (!Directory.Exists(GeneralConstants.APP_ROOT_DIR + folder))
            {
                Directory.CreateDirectory(GeneralConstants.APP_ROOT_DIR + folder);
            }
            url = string.Format("{0}/{1}", folder, filename);
        }

        public string Save(Stream filestream)
        {
            try
            {
#if AZURE
                IStorage container = new AzureBlob(Constants.AZURE_CONTAINER_IMAGES, BlobContainerPublicAccessType.Container);
                bool ok = container.AddBlobItem(blobname, imageUpload.InputStream);
                var url = filename;
#else
                SaveFile(filestream, url);
#if RACKSPACE
                new Thread(()=> Img.UploadImageSizes(url)).Start();
#endif
#endif
                // get filesize
                var fileinfo = new FileInfo(GeneralConstants.APP_ROOT_DIR + url);
                size = fileinfo.Length;
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return "";
            }
            finally
            {
                filestream.Flush();
                filestream.Close();
            }
            return url;
        }

        public static void Delete(string path)
        {
            var mainloc = GeneralConstants.APP_ROOT_DIR + path;
            if (File.Exists(mainloc))
            {
                File.Delete(mainloc);
            }
        }

        private bool SaveFile(Stream fileStream, string destName)
        {
            try
            {
                var destination = GeneralConstants.APP_ROOT_DIR + destName;
                // delete if exists
                if (File.Exists(destination))
                {
                    Syslog.Write("Existing file overwritten: " + destination);
                    File.Delete(destination);
                }

                using (var file = File.Create(destination))
                {
                    byte[] buffer = new byte[8 * 1024];
                    int len;
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, len);
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write("Unable to save file: " + destName + " " + ex.Message);
                return false;
            }
            return true;
        }



    }
}