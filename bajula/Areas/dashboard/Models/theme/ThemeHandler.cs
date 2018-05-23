using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HtmlAgilityPack;
using ICSharpCode.SharpZipLib.Zip;
using tradelr.Areas.dashboard.Models.store.navigation;
using tradelr.Areas.dashboard.Models.store.page;
using tradelr.DBML;
using tradelr.Libraries.file;
using tradelr.Library;
using tradelr.Library.Constants;
using clearpixels.Logging;
using tradelr.Models.products;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeHandler
    {
        private const string THEMES_PATH = "/Uploads/files";
        public const string CustomThemeName = "Custom";

        private bool IsMobile { get; set; }
        public bool IsCustom {get { return CustomThemeName == SelectedThemeName; }}

        private string DestRelativePath { get; set; }
        private string ThemeLocationPath { get; set; }
        private MASTERsubdomain sd { get; set; }
        private string SelectedThemeName { get; set; }

        public ThemeHandler(MASTERsubdomain sd, bool isMobile)
        {
            IsMobile = isMobile;
            this.sd = sd;

            SelectedThemeName = sd.theme.title;
            
            if (isMobile)
            {
                DestRelativePath = string.Format("{0}/{1}/mobile_theme", THEMES_PATH, sd.uniqueid);
            }
            else
            {
                DestRelativePath = string.Format("{0}/{1}/theme", THEMES_PATH, sd.uniqueid);
            }
            ThemeLocationPath = string.Format("{0}/{1}", GeneralConstants.APP_ROOT_DIR, DestRelativePath);
        }

        /// <summary>
        /// used to get key to cached liquid asset files
        /// </summary>
        /// <param name="subdomainid"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetCacheKey(string uniqueid, string filename, bool isMobile)
        {
            return string.Format("{0}|{1}|{2}|", uniqueid, filename, isMobile ? "mobile" : "main");
        }

        /// <summary>
        /// Deletes all theme files and return new empty theme directory
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo ClearUserThemeDirectory()
        {
            if (Directory.Exists(ThemeLocationPath))
            {
                UtilFile.DeleteDirectory(ThemeLocationPath);
            }
            return Directory.CreateDirectory(ThemeLocationPath);
        }

        public void CopyThemeToUserThemeDirectory(DirectoryInfo sourceThemeDir)
        {
            var destThemeDir = ClearUserThemeDirectory();

            CopyThemeDirectory(sourceThemeDir, destThemeDir);
        }

        private void CopyThemeDirectory(DirectoryInfo source, DirectoryInfo destination)
        {
            if (source.Attributes.HasFlag(FileAttributes.Hidden))
            {
                // don't process hidden directories
                return;
            }
            if (!destination.Exists)
            {
                destination.Create();
            }

            // Copy all files.
            FileInfo[] files = source.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    // don't process hidden files
                    continue;
                }
                var filename = file.Name;
                file.CopyTo(Path.Combine(destination.FullName, filename));
            }

            // Process subdirectories.
            DirectoryInfo[] dirs = source.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                // Get destination directory.
                string destinationDir = Path.Combine(destination.FullName, dir.Name);

                // Call CopyDirectory() recursively.
                CopyThemeDirectory(dir, new DirectoryInfo(destinationDir));
            }
        }

        public DirectoryInfo GetThemeDirectory()
        {
            return new DirectoryInfo(GeneralConstants.APP_ROOT_DIR + DestRelativePath);
        }

        public string GetThemeUrl()
        {
            return DestRelativePath;
        }

        private string GetThemeAssetPath()
        {
            return string.Format("{0}/assets", DestRelativePath);
        }

        private string GetThemeConfigPath()
        {
            return string.Format("{0}/config", DestRelativePath);
        }

        private string GetThemeTemplatePath()
        {
            return string.Format("{0}/templates", DestRelativePath);
        }

        public static void GenerateDefaultStructures(long subdomainid)
        {
            using (var repository = new TradelrRepository())
            {
                var mastersubdomain = repository.GetSubDomain(subdomainid);
                if (mastersubdomain == null)
                {
                    Syslog.Write("Can't generate liquid structures for domainid: " + subdomainid);
                    return;
                }

                if (mastersubdomain.theme != null)
                {
                    // already initialise, return
                    return;
                }

                // init default theme (SOLO)
                mastersubdomain.theme = new DBML.theme()
                {
                    created = DateTime.UtcNow,
                    title = "Solo",
                    preset = "",
                    url = "/Content/templates/store/themes/solo/thumb.jpg"
                };
                
                // do liquid stuff
                var page_about = new page()
                {
                    name = "About Us",
                    permalink = "about-us",
                    creator = mastersubdomain.organisation.users.First().id,
                    updated = DateTime.UtcNow,
                    settings = (int)PageSettings.VISIBLE
                };

                using (var reader = File.OpenText(GeneralConstants.APP_ROOT_DIR + "Content/templates/store/aboutus.txt"))
                {
                    page_about.content = reader.ReadToEnd();
                }
                mastersubdomain.pages.Add(page_about);

                var linklist_mainmenu = new linklist()
                {
                    permalink = "main-menu",
                    permanent = true,
                    title = "Main Menu"
                };
                mastersubdomain.linklists.Add(linklist_mainmenu);
                var link_home = new link
                {
                    title = "Home",
                    type = (int)LinkType.FRONTPAGE,
                    url = "/"
                };
                var link_catalog = new link()
                {
                    title = "Catalog",
                    type = (int)LinkType.WEB,
                    url = "/collections/all"
                };
                var link_about = new link()
                {
                    title = "About Us",
                    type = (int)LinkType.PAGE,
                    url = "/pages/about-us"
                };
                linklist_mainmenu.links.Add(link_home);
                linklist_mainmenu.links.Add(link_catalog);
                linklist_mainmenu.links.Add(link_about);

                var linklist_footer = new linklist()
                {
                    permalink = "footer",
                    permanent = true,
                    title = "Footer"
                };
                mastersubdomain.linklists.Add(linklist_footer);

                var link_search = new link
                {
                    title = "Search",
                    type = (int)LinkType.SEARCHPAGE,
                    url = "/search"
                };
                linklist_footer.links.Add(link_search);
                linklist_footer.links.Add(link_about);

                // create default collection
                var collection = new product_collection()
                                     {
                                         name = "Frontpage",
                                         permalink = "frontpage",
                                         settings = (int) (CollectionSettings.VISIBLE | CollectionSettings.PERMANENT)
                                     };
                mastersubdomain.product_collections.Add(collection);

                // finally save
                repository.Save();

                // copy theme files
                var handler = new ThemeHandler(mastersubdomain, false);
                new Thread(() =>
                {
                    var source =
                        new DirectoryInfo(GeneralConstants.APP_ROOT_DIR +
                                          "Content/templates/store/themes/solo");
                    handler.CopyThemeToUserThemeDirectory(source);
                }).Start();


                // copy mobile theme files
                var handler_mobile = new ThemeHandler(mastersubdomain, true);
                new Thread(() =>
                {
                    var source = handler.GetMobileThemeRepositorySourceDir();
                    handler_mobile.CopyThemeToUserThemeDirectory(source);
                }).Start();
                
            }
        }

        public DirectoryInfo GetMobileThemeRepositorySourceDir()
        {
            string path = string.Format("{0}/Content/templates/store/mobile", GeneralConstants.APP_ROOT_DIR);
            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path);
            }
            return null;
        }

        public DirectoryInfo GetThemeRepositorySourceDir()
        {
            string path = string.Format("{0}/Content/templates/store/themes/{1}", GeneralConstants.APP_ROOT_DIR, SelectedThemeName);
            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path);
            }
            return null;
        }

        public string GetSettingsHtml()
        {
            var config_file = string.Format("{0}/settings.html", GetThemeConfigPath());
            if (!File.Exists(GeneralConstants.APP_ROOT_DIR + config_file))
            {
                return "";
            }
            var doc = new HtmlDocument
                          {
                              OptionFixNestedTags = true
                          };
            doc.Load(GeneralConstants.APP_ROOT_DIR + config_file);

            #region populate SELECTs
            foreach (var entry in doc.DocumentNode.Descendants("select").Where(x => x.Attributes["class"] != null))
            {
                // create none option
                var none = doc.CreateElement("option");
                none.SetAttributeValue("value", "");
                none.InnerHtml = "None";

                switch (entry.Attributes["class"].Value)
                {
                    case "linklist":
                        entry.InsertAfter(none, entry.FirstChild);
                        foreach (var linklist in sd.linklists)
                        {
                            var option = doc.CreateElement("option");
                            option.SetAttributeValue("value", linklist.permalink);
                            option.InnerHtml = linklist.title;
                            entry.InsertAfter(option, entry.FirstChild);
                        }
                        break;
                    case "collection":
                        entry.InsertAfter(none, entry.FirstChild);
                        foreach (var collection in sd.product_collections)
                        {
                            var option = doc.CreateElement("option");
                            option.SetAttributeValue("value", collection.permalink);
                            option.InnerHtml = collection.name;
                            entry.InsertAfter(option, entry.FirstChild);
                        }
                        break;
                    case "blog":
                        entry.InsertAfter(none, entry.FirstChild);
                        foreach (var blog in sd.blogs)
                        {
                            var option = doc.CreateElement("option");
                            option.SetAttributeValue("value", blog.permalink);
                            option.InnerHtml = blog.title;
                            entry.InsertAfter(option, entry.FirstChild);
                        }
                        break;
                    case "page":
                        entry.InsertAfter(none, entry.FirstChild);
                        foreach (var page in sd.pages)
                        {
                            var option = doc.CreateElement("option");
                            option.SetAttributeValue("value", page.permalink);
                            option.InnerHtml = page.name;
                            entry.InsertAfter(option, entry.FirstChild);
                        }
                        break;
                    case "font":
                        // create optgroups for different font types
                        // monospace
                        var monospacegroup = doc.CreateElement("optgroup");
                        monospacegroup.SetAttributeValue("label", "Monospace");
                        entry.InsertAfter(monospacegroup, entry.FirstChild);

                        var monospace1 = doc.CreateElement("option");
                        monospace1.SetAttributeValue("value", "'Courier New', Courier, monospace");
                        monospace1.InnerHtml = "Courier New";
                        monospacegroup.InsertAfter(monospace1, monospacegroup.FirstChild);

                        var monospace2 = doc.CreateElement("option");
                        monospace2.SetAttributeValue("value", "Monaco, 'Lucida Console', 'DejaVu Sans Mono', monospace");
                        monospace2.InnerHtml = "Monaco";
                        monospacegroup.InsertAfter(monospace2, monospacegroup.FirstChild);

                        // serif
                        var serifgroup = doc.CreateElement("optgroup");
                        serifgroup.SetAttributeValue("label", "Serif");
                        entry.InsertAfter(serifgroup, entry.FirstChild);

                        var serif1 = doc.CreateElement("option");
                        serif1.SetAttributeValue("value", "Garamond, Baskerville, Caslon, serif");
                        serif1.InnerHtml = "Garamond";
                        serifgroup.InsertAfter(serif1, serifgroup.FirstChild);

                        var serif2 = doc.CreateElement("option");
                        serif2.SetAttributeValue("value", "Georgia, Utopia, 'Times New Roman', Times, serif");
                        serif2.InnerHtml = "Georgia";
                        serifgroup.InsertAfter(serif2, serifgroup.FirstChild);

                        var serif3 = doc.CreateElement("option");
                        serif3.SetAttributeValue("value", "Palatino, 'Palatino Linotype', 'Book Antiqua', serif");
                        serif3.InnerHtml = "Palatino";
                        serifgroup.InsertAfter(serif3, serifgroup.FirstChild);

                        var serif4 = doc.CreateElement("option");
                        serif4.SetAttributeValue("value", "'Times New Roman', Times, serif");
                        serif4.InnerHtml = "Times New Roman";
                        serifgroup.InsertAfter(serif4, serifgroup.FirstChild);

                        // san-serif
                        var sanserifgroup = doc.CreateElement("optgroup");
                        sanserifgroup.SetAttributeValue("label","Sans-serif");
                        entry.InsertAfter(sanserifgroup, entry.FirstChild);

                        var sans0 = doc.CreateElement("option");
                        sans0.SetAttributeValue("value", "Helvetica, Arial, sans-serif");
                        sans0.InnerHtml = "Helvetica/Arial";
                        sanserifgroup.InsertAfter(sans0, sanserifgroup.FirstChild);

                        var sans1 = doc.CreateElement("option");
                        sans1.SetAttributeValue("value", "Impact, Charcoal, Helvetica, Arial, sans-serif");
                        sans1.InnerHtml = "Impact";
                        sanserifgroup.InsertAfter(sans1, sanserifgroup.FirstChild);

                        var sans2 = doc.CreateElement("option");
                        sans2.SetAttributeValue("value", "'Lucida Grande', 'Lucida Sans Unicode', 'Lucida Sans', Lucida, Helvetica, Arial, sans-serif");
                        sans2.InnerHtml = "Lucida Grande";
                        sanserifgroup.InsertAfter(sans2, sanserifgroup.FirstChild);

                        var sans3 = doc.CreateElement("option");
                        sans3.SetAttributeValue("value", "Trebuchet MS, sans-serif");
                        sans3.InnerHtml = "Trebuchet MS";
                        sanserifgroup.InsertAfter(sans3, sanserifgroup.FirstChild);

                        var sans4 = doc.CreateElement("option");
                        sans4.SetAttributeValue("value", "Verdana, Helvetica, Arial, sans-serif");
                        sans4.InnerHtml = "Verdana";
                        sanserifgroup.InsertAfter(sans4, sanserifgroup.FirstChild);
                        
                        // find out where to insert existing fonts
                        var optionNodes = entry.ChildNodes.Where(x => x.Name == "option").ToArray();
                        for (int i = optionNodes.Length - 1; i >= 0; i--)
                        {
                            var fonts =
                                optionNodes[i].GetAttributeValue("value", "")
                                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(x => x.ToLower().Trim()).ToArray();
                            if (fonts.Intersect(FontType.SerifFonts).Count() != 0)
                            {
                                serifgroup.InsertAfter(optionNodes[i], serifgroup.FirstChild);
                                entry.ChildNodes.Remove(optionNodes[i]);
                            }
                            else if (fonts.Intersect(FontType.SansSerifFonts).Count() != 0)
                            {
                                sanserifgroup.InsertAfter(optionNodes[i], sanserifgroup.FirstChild);
                                entry.ChildNodes.Remove(optionNodes[i]);
                            }
                            else if (fonts.Intersect(FontType.MonospaceFonts).Count() != 0)
                            {
                                monospacegroup.InsertAfter(optionNodes[i], monospacegroup.FirstChild);
                                entry.ChildNodes.Remove(optionNodes[i]);
                            }
                        }
                        break;
                    default:
                        break;

                }
            } 
            #endregion

            // look for input=file 
            #region populate input=file
            
            foreach (var entry in doc.DocumentNode.Descendants("input").Where(x => x.Attributes["type"] != null && x.Attributes["type"].Value == "file").ToArray())
            {
                var uploaded_filename = entry.Attributes["name"].Value;

                // see if file exists
                if (!string.IsNullOrEmpty(uploaded_filename))
                {
                    var path = string.Format("{0}/{1}", GetThemeAssetPath(), uploaded_filename);
                    if (File.Exists(GeneralConstants.APP_ROOT_DIR + path))
                    {
                        var preview = doc.CreateElement("a");
                        preview.SetAttributeValue("href", path);
                        preview.SetAttributeValue("class", "block");
                        preview.SetAttributeValue("target", "_blank");
                        preview.InnerHtml = "<img src='/Content/img/icons/img_icon.png' alt='' /> " + uploaded_filename;

                        var td = entry.ParentNode;

                        td.InsertAfter(preview, td.FirstChild);
                    }
                }
            }

            #endregion

            var sb = new StringBuilder();
            var writer = new StringWriter(sb);
            doc.Save(writer);

            return sb.ToString();
        }

        public string GetSettingsText()
        {
            var config_settings = string.Format("{0}/settings_data.json", GetThemeConfigPath());
            if (!File.Exists(GeneralConstants.APP_ROOT_DIR + config_settings))
            {
                return "";
            }
            var settings_text = File.ReadAllText(GeneralConstants.APP_ROOT_DIR + config_settings);
            return settings_text;
        }

        public bool ThemeCopiedOK()
        {
            // use settings.html to determine if theme was copied properly
            var config_file = string.Format("{0}/settings.html", GetThemeConfigPath());
            if (!File.Exists(GeneralConstants.APP_ROOT_DIR + config_file))
            {
                return false;
            }
            return true;
        }

        public void SaveSettings(string settings)
        {
            var config_settings = string.Format("{0}/settings_data.json", GetThemeConfigPath());
            File.WriteAllText(GeneralConstants.APP_ROOT_DIR + config_settings, settings);
        }

        public void SaveImage(string filename, Stream inputStream)
        {
            var asset_path = string.Format("{0}/assets", DestRelativePath);
            if (!Directory.Exists(GeneralConstants.APP_ROOT_DIR + asset_path))
            {
                Directory.CreateDirectory(GeneralConstants.APP_ROOT_DIR + asset_path);
            }
            var file_path = string.Format("{0}/{1}", asset_path, filename);

            // test if this is actually an image
            try
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                Image.FromStream(inputStream);
            }
            catch (Exception ex)
            {
                Syslog.Write(string.Format("{0}:{1}:{2}", sd.id, filename, ex.Message));
                return;
            }

            using (var file = File.Create(GeneralConstants.APP_ROOT_DIR + file_path))
            {
                inputStream.Seek(0, SeekOrigin.Begin);
                Utility.CopyStream(inputStream, file);
            }
        }

        public IEnumerable<string> GetTemplateNamesStartingWith(string name)
        {
            var path = GeneralConstants.APP_ROOT_DIR + GetThemeTemplatePath();
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                return Enumerable.Empty<string>();
            }

            var files = dir.GetFiles(name + "*.liquid");

            return files.OrderBy(x => x.Name).Select(x => x.Name);

        }

        public void ZipFolder(string rootFolder, string currentFolder, ZipOutputStream zStream)
        {
            string[] subFolders = Directory.GetDirectories(currentFolder);

            //calls the method recursively for each subfolder
            foreach (string folder in subFolders)
            {
                ZipFolder(rootFolder, folder, zStream);
            }

            string relativePath = currentFolder.Substring(rootFolder.Length) + "/";

            //the path "/" is not added or a folder will be created
            //at the root of the file
            if (relativePath.Length > 1)
            {
                var dirEntry = new ZipEntry(relativePath);
                dirEntry.DateTime = DateTime.Now;
            }

            //adds all the files in the folder to the zip
            foreach (string file in Directory.GetFiles(currentFolder))
            {
                AddFileToZip(zStream, relativePath, file);
            }
        }

        private static void AddFileToZip(ZipOutputStream zStream, string relativePath, string file)
        {
            var buffer = new byte[4096];

            //the relative path is added to the file in order to place the file within
            //this directory in the zip
            string fileRelativePath = (relativePath.Length > 1 ? relativePath : " ")
                                      + Path.GetFileName(file);

            var entry = new ZipEntry(fileRelativePath.Substring(1));
            entry.DateTime = DateTime.Now;
            zStream.PutNextEntry(entry);

            using (FileStream fs = File.OpenRead(file))
            {
                int sourceBytes;
                do
                {
                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                    zStream.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);
            }
        }
    }
}