using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Web;
using clearpixels.Logging;
using tradelr.DBML;
using tradelr.DBML.Helper;
using tradelr.Library.Constants;

#if AZURE
using Microsoft.WindowsAzure.StorageClient;
using tradelr.Common;
using tradelr.Libraries.CloudStorage.Azure;
#endif
#if RACKSPACE
#endif

namespace tradelr.Common.Library.Imaging
{
    public static class Img
    {
#if AZURE
        private static readonly IStorage photoContainer = new AzureBlob(Constants.AZURE_CONTAINER_IMAGES, BlobContainerPublicAccessType.Container);
#endif
        /// <summary>
        /// returns path to thumbnail, if thumbnail does not exist it creates it then returns the new path
        /// </summary>
        /// <param name="filePath">the path to the thumbnail, usually stored in db</param>
        /// <param name="imgsize"></param>
        /// <param name="returnLocalFileOnly"></param>
        /// <returns></returns>
        public static string by_size(string filePath, Imgsize type) 
        {
            var dim = getImageDimensionsFromSize(type);
            int width = dim.Width;
            int height = dim.Height;
            // appends dimension to path
            string suffix = type.ToString().ToLower();
            string ofile;
            string thumb = ofile = filePath;
            string ext = thumb.Substring(thumb.LastIndexOf(".") + 1);
            string part1 = thumb.Substring(0, thumb.LastIndexOf("."));
            if (part1.IndexOf(".") == -1)
            {
                thumb = part1 + "_" + suffix + "." + ext;
            }
            else
            {
                string part2 = part1.Substring(0, part1.LastIndexOf("."));
                thumb = part2 + "_" + suffix + "." + ext;
            }
            bool fileExist = false;
#if AZURE
            fileExist = photoContainer.DoesBlobItemExists(thumb);
#else
#if RACKSPACE
            // check if thumbnail exists on rackspace first
            if (!returnLocalFileOnly)
            {
                CloudFile cf = new CloudFile(Constants.RACKSPACE_CONTAINER_IMAGES);
                fileExist = cf.DoesBlobItemExists(Path.GetFileName(thumb));
                if (fileExist)
                {
                    thumb = cf.GetBlobItemUri(Path.GetFileName(thumb));
                }
            }
#endif
            if (!fileExist)
            {
                fileExist = File.Exists(GeneralConstants.APP_ROOT_DIR + thumb);
            }
#endif
            if (!fileExist)
            {
                thumb = thumbnail(ofile, suffix, width, height, type);
            }
#if AZURE
            if (!string.IsNullOrEmpty(thumb))
            {
                return photoContainer.GetBlobItemUri(thumb);
            }
            else
            {
                return "";
            }
#else
            return thumb;
#endif
        }

        private static Image createResizedImage(Image originalImage, Size newSize, Imgsize type)
        {
            //Detach image from its source
            var oImageOriginal = (Image)originalImage.Clone();

            //Resize new image
            var oResizedImage = new Bitmap(newSize.Width, newSize.Height);
            Graphics oGraphic = Graphics.FromImage(oResizedImage);

            oGraphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            oGraphic.CompositingQuality = CompositingQuality.HighSpeed;
            oGraphic.SmoothingMode = SmoothingMode.HighSpeed;
            oGraphic.InterpolationMode = InterpolationMode.Low;
            var oRectangle = new Rectangle(0, 0, newSize.Width, newSize.Height);

            oGraphic.DrawImage(oImageOriginal, oRectangle);

            // cleanup
            oGraphic.Dispose();
            
            if (type != Imgsize.BANNER && 
                newSize.Width != newSize.Height)
            {
                float size;
                float ratioX;
                float ratioY;
                if(newSize.Width > newSize.Height)
                {
                    size = newSize.Width;
                    ratioX = size;
                    ratioY = (float)newSize.Height / (float)newSize.Width;
                    ratioY *= size;
                }
                else
                {
                    size = newSize.Height;
                    ratioY = size;
                    ratioX = (float)newSize.Width / (float)newSize.Height;
                    ratioX *= size;
                }

                int sizeX = (int)ratioX;
                int sizeY = (int)ratioY;

                var backgroundImage = new Bitmap((int)size, (int)size, PixelFormat.Format32bppPArgb);
                Graphics g = Graphics.FromImage(backgroundImage);
                g.CompositingQuality = CompositingQuality.AssumeLinear;
                g.SmoothingMode = SmoothingMode.Default;
                g.InterpolationMode = InterpolationMode.High;
                g.FillRectangle(Brushes.White, 0, 0, size, size);
                g.DrawImageUnscaled(oResizedImage, (int)((size - sizeX) / 2), (int)((size - sizeY) / 2));
                g.Dispose();
                return backgroundImage;
            }

            oImageOriginal.Dispose();
            return oResizedImage;
        }

        private static Size getImageDimensionsFromSize(Imgsize size)
        {
            switch (size)
            {
                case Imgsize.PICO:
                    return new Size(16, 16);
                case Imgsize.ICON:
                    return new Size(32, 32);
                case Imgsize.THUMB:
                    return new Size(50, 50);
                case Imgsize.SMALL:
                    return new Size(100,100);
                case Imgsize.COMPACT:
                    return new Size(160,160);
                case Imgsize.MEDIUM:
                    return new Size(240,240);
                case Imgsize.LARGE:
                    return new Size(480, 480);
                case Imgsize.GRANDE:
                    return new Size(600, 600);
                case Imgsize.ORIGINAL:
                    return new Size(1024, 1024);
                case Imgsize.BANNER:
                    return new Size(880, 150);
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// generates a thumbnail given the path of the original images
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="suffix"></param>
        /// <param name="desiredWidth"></param>
        /// <param name="desiredHeight"></param>
        /// <returns></returns>
        private static string thumbnail(string filePath, string suffix, float desiredWidth, float desiredHeight, Imgsize type) 
        {
            string thumb = filePath;
            string file = filePath;
            string ext = thumb.Substring(thumb.LastIndexOf(".") + 1);
            thumb = thumb.Substring(0, thumb.LastIndexOf(".")) + "_" + suffix + "." + ext;
#if AZURE 
            file = file.Substring(file.LastIndexOf("/") + 1);
            thumb = thumb.Substring(thumb.LastIndexOf("/") + 1);
            bool exists = photoContainer.DoesBlobItemExists(file);
#else
            bool exists = File.Exists(GeneralConstants.APP_ROOT_DIR + file);
#endif
            if (!exists) 
            {
                // delete from db
                new Thread(() =>
                               {
                                   Syslog.Write(String.Concat("Cannot find file: ", GeneralConstants.APP_ROOT_DIR + file));
                                   using (var db = new tradelrDataContext())
                                   {
                                       var repository = new TradelrRepository(db);
                                       repository.SetIsolationToNoLock();
                                       repository.DeleteImage(file);
                                   }

                               }).Start();
                
                return "";
            }
            // These are the ratio calculations
#if AZURE 
            Image img = Image.FromStream(photoContainer.GetBlobContentStream(file));
#else
            Image img = Image.FromFile(GeneralConstants.APP_ROOT_DIR + file);
#endif
            int width = img.Width;
            int height = img.Height;

            img.Dispose();

            float factor = 0;
            if (width > 0 && height > 0) 
            {
                float wfactor = desiredWidth / width;
                float hfactor = desiredHeight / height;
                factor = wfactor < hfactor ? wfactor : hfactor;
            }
            if (factor > 0) 
            {
                int twidth = Convert.ToInt32(Math.Floor(factor * width));
                int theight = Convert.ToInt32(Math.Floor(factor * height));
                convert(file, thumb, twidth, theight, type);
            } 
            else
            {
#if AZURE 
                photoContainer.DeleteBlobItem(thumb);
                photoContainer.CopyContent(file, thumb);
#else
                if (File.Exists(GeneralConstants.APP_ROOT_DIR + thumb))
                {
                    File.Delete(GeneralConstants.APP_ROOT_DIR + thumb);
                }
                File.Copy(GeneralConstants.APP_ROOT_DIR + file, GeneralConstants.APP_ROOT_DIR + thumb);
#endif
            }
            return thumb;
        }

        private static void convert(string source, string destination, int desiredWidth, int desiredHeight, Imgsize type) 
        {
            createImage(source, destination, desiredWidth, desiredHeight, type);
        }

        private static bool createImage(string srcName, string destName, int desiredWidth, int desiredHeight, Imgsize type) 
        {
            var source = GeneralConstants.APP_ROOT_DIR + srcName;
            var destination = GeneralConstants.APP_ROOT_DIR + destName;
            // Capture the original size of the uploaded image
            Image src = null;
            try
            {
#if AZURE 
                src = Image.FromStream(photoContainer.GetBlobContentStream(srcName));
#else
                src = Image.FromFile(source);
#endif
            }
            catch (Exception ex)
            {
                if (src != null)
                {
                    src.Dispose();
                }
                Syslog.Write(ex);
                throw;
            }
            
            //Resize new image
            //Image tmp = src.GetThumbnailImage(desiredWidth, desiredHeight, null, IntPtr.Zero);
            Size imgSize = new Size(desiredWidth,desiredHeight);
            Image tmp = createResizedImage(src, imgSize, type);

            try
            {
                File.Delete(destination);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
            }

            try
            {
#if AZURE 
                var ms = new MemoryStream();
                tmp.Save(ms, destName.ToImageFormat());
                ms.Seek(0, SeekOrigin.Begin);
                photoContainer.AddBlobItem(destName, ms);
#else
                tmp.Save(destination, destination.ToImageFormat());
#endif
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                Syslog.Write(destination);
                return false;
            }
            finally
            {
                src.Dispose();
                tmp.Dispose();
            }
           
            return true;
        }

        public static bool SaveImage(HttpPostedFileBase file, string destName)
        {
            try
            {
#if AZURE 
                photoContainer.DeleteBlobItem(destName);
                var destination = destName;
#else
                var destination = GeneralConstants.APP_ROOT_DIR + destName;
                // delete if exists
                if (File.Exists(destination))
                {
                    Syslog.Write("Existing file overwritten: " + destination);
                    File.Delete(destination);
                }
#endif
#if AZURE 
                string blobname = Path.GetFileName(destination);
                file.InputStream.Seek(0, SeekOrigin.Begin);
                photoContainer.AddBlobItem(blobname, file.InputStream);
#else
                file.SaveAs(destination);
#endif
            }
            catch (Exception ex)
            {
                Syslog.Write("Unable to save image: " + destName + " " + ex.Message);
                return false;
            }
            return true;
        }

#if RACKSPACE
        /// <summary>
        /// using the given image url, create all image sizes and upload images to cloud
        /// usually done on a separate thread
        /// </summary>
        /// <param name="imagePath">relative file path of image</param>
        public static void UploadImageSizes(string imagePath)
        {
            // go through all image sizes
            var cloudfile = new CloudFile(Constants.RACKSPACE_CONTAINER_IMAGES);
            foreach (Imgsize values in Enum.GetValues(typeof(Imgsize)))
            {
                var thumbnailPath = Img.by_size(imagePath, values, true);

                if (string.IsNullOrEmpty(thumbnailPath))
                {
                    Syslog.Write("Image not found:" + imagePath);
                    continue;
                }

                // save various image sizes to cloud
                cloudfile.AddBlobItemAsync(Constants.PHOTO_SOURCE_DIRECTORY + thumbnailPath);
            }
            // save original image to cloud
            cloudfile.AddBlobItemAsync(Constants.PHOTO_SOURCE_DIRECTORY + imagePath);
        }
#endif

        public static string ToDataUri(this string url)
        {
            var path = GeneralConstants.APP_ROOT_DIR + by_size(url, Imgsize.MEDIUM);
            if (!File.Exists(path))
            {
                return "";
            }
            byte[] buff = null;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(path).Length;
            buff = br.ReadBytes((int)numBytes);
            var datauri = Convert.ToBase64String(buff);
            return String.Concat(url.ToDataUriType(), datauri);
        }

        private static string ToDataUriType(this string filename)
        {
            var extIndex = filename.LastIndexOf('.');
            var ext = filename.Substring(extIndex);
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".JPG":
                case "JPEG":
                    return "data:image/jpeg;base64,";
                case ".png":
                case ".PNG":
                    return "data:image/png;base64,";
                case ".gif":
                case ".GIF":
                    return "data:image/gif;base64,";
                default:
                    throw new Exception(String.Concat("Unrecognised image extension for datauri: ", filename));
            }
        }

        public static string ConvertToBase64String(string path)
        {
            string data;
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var photoContents = new byte[stream.Length];
                stream.Read(photoContents, 0, photoContents.Length);
                data = Convert.ToBase64String(photoContents);
            }

            return data;
        }
    }
}