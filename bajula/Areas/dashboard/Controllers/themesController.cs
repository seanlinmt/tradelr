using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.Controllers;
using tradelr.DBML;
using tradelr.Libraries.ActionFilters;
using tradelr.Library;
using tradelr.Library.Caching;
using tradelr.Library.Constants;
using tradelr.Library.JSON;
using clearpixels.Logging;
using tradelr.Models.mobile;
using tradelr.Models.store.themes;
using tradelr.Models.users;
using ArgumentException = DotLiquid.Exceptions.ArgumentException;
using ThemeFile = tradelr.Areas.dashboard.Models.theme.ThemeFile;

namespace tradelr.Areas.dashboard.Controllers
{
    //[ElmahHandleError]
    [PermissionFilter(permission = UserPermission.NETWORK_STORE)]
    [RoleFilter(role = UserRole.USER)]
    [TradelrHttps]
    public class themesController : baseController
    {
        private const string Themepath = "/Content/templates/store/themes";
        private static string[] CriticalThemeFiles = new[]
                                                        {
                                                            "/layout/theme.liquid",
                                                            "/templates/404.liquid",
                                                            "/templates/article.liquid",
                                                            "/templates/blog.liquid",
                                                            "/templates/cart.liquid",
                                                            "/templates/collection.liquid",
                                                            "/templates/index.liquid",
                                                            "/templates/page.liquid",
                                                            "/templates/product.liquid",
                                                            "/templates/seatch.liquid"
                                                        };
        
        [HttpPost]
        public ActionResult AddAsset(string qqfile, bool ismobile)
        {
            Stream inputStream;
            if (Request.Files.Count != 0)
            {
                inputStream = Request.Files[0].InputStream;
            }
            else
            {
                inputStream = Request.InputStream;
            }
            inputStream.Position = 0;

            var handler = new ThemeHandler(MASTERdomain, ismobile);
            var dirs = handler.GetThemeDirectory().GetDirectories("assets");
            if (dirs.Length == 0)
            {
                return Json("Theme directory corrupted. Please select theme from gallery again.".ToJsonFail());
            }

            // try to get existing template
            // if none exist then create blank file
            var filename = qqfile;
            var dest = Path.Combine(dirs[0].FullName, filename);

            // if destination file exist, then delete destination file
            if (System.IO.File.Exists(dest))
            {
                System.IO.File.Delete(dest);
            }

            using (Stream file = System.IO.File.OpenWrite(dest))
            {
                Utility.CopyStream(inputStream, file);
            }

            return Json("File uploaded successfully".ToJsonOKMessage());
        }

        [HttpPost]
        public ActionResult AddSnippet(string name, bool ismobile)
        {
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            var dirs = handler.GetThemeDirectory().GetDirectories("snippets");
            if (dirs.Length == 0)
            {
                return new EmptyResult();
            }
            // try to get existing template
            // if none exist then create blank file
            var filename = string.Format("{0}.liquid", name);
            var dest = Path.Combine(dirs[0].FullName, filename);

            // if destination file exist, then delete destination file
            if (System.IO.File.Exists(dest))
            {
                System.IO.File.Delete(dest);
            }

            System.IO.File.Create(dest).Dispose();

            var viewmodel = new LiquidFileContent(handler.GetThemeUrl(), string.Format("/snippets/{0}", filename));
            return View("filecontent", viewmodel);
        }

        [HttpPost]
        public ActionResult AddTemplate(string name, string type, bool ismobile)
        {
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            var dirs = handler.GetThemeDirectory().GetDirectories("templates");
            if (dirs.Length == 0)
            {
                return new EmptyResult();
            }
            // try to get existing template
            // if none exist then create blank file
            FileInfo[] files = dirs[0].GetFiles(string.Format("{0}.liquid", type));
            var filename = string.Format("{0}.{1}.liquid", type, name);
            var dest = Path.Combine(dirs[0].FullName, filename);

            // if destination file exist, then delete destination file
            if (System.IO.File.Exists(dest))
            {
                System.IO.File.Delete(dest);
            }

            if (files.Length != 0)
            {
                files[0].CopyTo(dest);
            }
            else
            {
                System.IO.File.Create(dest).Dispose();
            }

            var viewmodel = new LiquidFileContent(handler.GetThemeUrl(), string.Format("/templates/{0}", filename));
            return View("fileContent", viewmodel);
        }

        [HttpPost]
        public ActionResult Current(string name)
        {
            const string rootdir = GeneralConstants.APP_ROOT_DIR + Themepath;
            var dir = new DirectoryInfo(rootdir);
            DirectoryInfo selected = null;
            foreach (var themedir in dir.GetDirectories())
            {
                if (themedir.Name == name)
                {
                    selected = themedir;
                    break;
                }
            }

            if (selected == null)
            {
                return RedirectToAction("Index", "error", new { Area = "" });
            }

            try
            {
                var handler = new ThemeHandler(MASTERdomain, false);
                handler.CopyThemeToUserThemeDirectory(selected);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return RedirectToAction("Index", "error", new {Area = ""});
            }

            // save theme
            var theme = MASTERdomain.theme;
            if (theme == null)
            {
                theme = new theme();
                MASTERdomain.theme = theme;
            }

            theme.title = name;
            theme.url = string.Format("{0}/{1}/thumb.jpg", Themepath, selected.Name);
            theme.created = DateTime.UtcNow;

            repository.Save();

            // need to invalidate any cached liquid assets
            CacheHelper.Instance.invalidate_dependency(DependencyType.liquid_assets, MASTERdomain.uniqueid);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteFile(string path, bool ismobile)
        {
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            var file = string.Format("{0}{1}{2}", GeneralConstants.APP_ROOT_DIR, handler.GetThemeUrl(), path);

            // prevent critical files from being deleted
            if (CriticalThemeFiles.Contains(path))
            {
                return Json("Unable to delete critical theme files".ToJsonFail());
            }

            try
            {
                System.IO.File.Delete(file);
            }
            catch (Exception ex)
            {
                return SendJsonErrorResponse(ex);
            }
            return Json("File deleted".ToJsonOKMessage());
        }

        public ActionResult Editor(bool ismobile)
        {
            // check that there are theme files, otherwise copy
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            if (!handler.IsCustom && !handler.ThemeCopiedOK())
            {
                DirectoryInfo sourceThemeDir = ismobile
                                                   ? handler.GetMobileThemeRepositorySourceDir()
                                                   : handler.GetThemeRepositorySourceDir();
                if (sourceThemeDir == null)
                {
                    throw new NotImplementedException();
                }
                handler.CopyThemeToUserThemeDirectory(sourceThemeDir);
            }

            var viewmodel = new ThemeEditorViewModel() { themeType = ismobile ? ThemeType.MOBILE : ThemeType.MAIN };

            var themedir = handler.GetThemeDirectory();
            foreach (var dir in themedir.GetDirectories().OrderByDescending(x => x.Name))
            {
                var folder = new ThemeDirectory()
                            {
                                foldername = dir.Name
                            };
                switch (dir.Name)
                {
                    case "templates":
                        viewmodel.templates_folder = folder;
                        break;
                    case "assets":
                        viewmodel.asset_folder = folder;
                        break;
                    case "snippets":
                        viewmodel.snippets_folder = folder;
                        break;
                    case "layout":
                        viewmodel.layout_folder = folder;
                        break;
                    case "config":
                        viewmodel.config_folder = folder;
                        break;
                    default:
                        throw new ArgumentException("Unknown directory " + dir.Name);
                }
                foreach (var entry in dir.GetFiles())
                {
                    var ext = entry.Extension.ToLower();
                    var file = new ThemeFile();
                    file.classname = string.Format("ext_{0}", ext.Substring(1));
                    file.url = string.Format("/{0}/{1}", dir.Name, entry.Name);
                    file.filename = entry.Name;

                    folder.files.Add(file);
                }
            }

            return View(viewmodel);
        }

        [HttpGet]
        public ActionResult Export()
        {
            var handler = new ThemeHandler(MASTERdomain, false);

            var themedir = GeneralConstants.APP_ROOT_DIR + handler.GetThemeUrl();

            var ms = new MemoryStream();
            using (var s = new ZipOutputStream(ms))
            {
                s.SetLevel(9); // 0 - store only to 9 - means best compression
                handler.ZipFolder(themedir, themedir, s);
            }

            return File(ms.ToArray(), "application/zip", string.Format("TradelrTheme_{0}_{1}.zip", MASTERdomain.theme.title, DateTime.UtcNow.ToShortDateString()));

        }

        [HttpPost]
        public ActionResult FileContent(string path, bool ismobile)
        {
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            var root = handler.GetThemeUrl();
            var viewmodel = new LiquidFileContent(root, path);

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Import(bool ismobile)
        {
            Stream inputStream;
            if (Request.Files.Count != 0)
            {
                inputStream = Request.Files[0].InputStream;
            }
            else
            {
                inputStream = Request.InputStream;
            }
            inputStream.Position = 0;
            var handler = new ThemeHandler(MASTERdomain, ismobile);
            try
            {
                var themedir = handler.ClearUserThemeDirectory();

                using (var s = new ZipInputStream(inputStream))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {

                        Console.WriteLine(theEntry.Name);

                        string directoryName = Path.GetDirectoryName(string.Format("{0}/{1}",themedir.FullName, theEntry.Name));
                        string fileName = Path.GetFileName(string.Format("{0}/{1}", themedir.FullName, theEntry.Name));

                        // create directory
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = System.IO.File.Create(string.Format("{0}/{1}", themedir.FullName, theEntry.Name)))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return Json("Failed to extract theme files".ToJsonFail());
            }

            // update theme
            var theme = MASTERdomain.theme;
            if (theme == null)
            {
                theme = new theme();
                MASTERdomain.theme = theme;
            }

            theme.title = ThemeHandler.CustomThemeName;
            theme.url = "/Content/img/store/custom_theme.png";
            theme.created = DateTime.UtcNow;

            // remove custom templates from existing pages
            var pages = MASTERdomain.pages.Where(x => !string.IsNullOrEmpty(x.templatename));
            foreach (var page in pages)
            {
                page.templatename = "";
            }
            repository.Save();

            // need to invalidate any cached liquid assets
            CacheHelper.Instance.invalidate_dependency(DependencyType.liquid_assets, MASTERdomain.uniqueid);
            
            return Json("Theme imported successfully".ToJsonOKMessage());
        }

        [HttpGet]
        [NoCache]
        public ActionResult Index()
        {
            // ensure that theme is not null
            if (MASTERdomain.theme == null)
            {
                MASTERdomain.theme = new theme()
                {
                    created = DateTime.UtcNow,
                    title = "Solo",
                    preset = "",
                    url = "/Content/templates/store/themes/solo/thumb.jpg"
                };
                repository.Save();
            }

            return View(baseviewmodel);
        }

        [HttpGet]
        public ActionResult List()
        {
            var viewmodel = new ThemeViewModel();

            var rootdir = GeneralConstants.APP_ROOT_DIR + Themepath;
            var dir = new DirectoryInfo(rootdir);
            foreach (var themedir in dir.GetDirectories())
            {
                if (themedir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    continue;
                }

                var thumbnail = string.Format("{0}/thumb.jpg", themedir.FullName);
                if (!System.IO.File.Exists(thumbnail))
                {
#if !DEBUG
                    Syslog.Write(string.Format("Theme thumbnail missing: {0}", thumbnail));
#endif
                    continue;
                }
                var theme = new Theme
                                {
                                    title = themedir.Name,
                                    thumbnail = string.Format("{0}/{1}/thumb.jpg", Themepath, themedir.Name)
                                };
                viewmodel.gallery.Add(theme);
            }

            // get current theme
            viewmodel.current = MASTERdomain.theme.ToModel();

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult MobileReset()
        {
            var handler = new ThemeHandler(MASTERdomain, true);
            var source = handler.GetMobileThemeRepositorySourceDir();

            handler.CopyThemeToUserThemeDirectory(source);

            return Json("Mobile theme has been reset".ToJsonOKMessage());
        }

        [HttpPost]
        [JsonFilter(Param = "changes", RootType = typeof(ThemeChangesJSON))]
        public ActionResult Save(ThemeChangesJSON changes)
        {
            var handler = new ThemeHandler(MASTERdomain, changes.ismobile);
            var root = handler.GetThemeUrl();

            for (var i = 0; i < changes.names.Length; i++)
            {
                var path = changes.names[i];
                var content = HttpUtility.UrlDecode(changes.contents[i]);
                try
                {
                    var filepath = string.Format("{0}/{1}", root, path);
                    var file = new FileInfo(GeneralConstants.APP_ROOT_DIR + filepath);
                    using (var writer = file.CreateText())
                    {
                        writer.Write(content);
                    }
                    // invalidate cache for liquid files (usually .css and .js)
                    if (filepath.EndsWith(".liquid"))
                    {
                        var filename = Path.GetFileName(filepath);
                        var cachekey = ThemeHandler.GetCacheKey(MASTERdomain.uniqueid, filename, changes.ismobile);
                        CacheHelper.Instance.Remove(CacheItemType.liquid_assets, cachekey);
                    }
                }
                catch (Exception ex)
                {
                    Syslog.Write(ex);
                }
            }

            // update theme versions
            var version = DateTime.UtcNow.Ticks.ToString("x");
            if (changes.ismobile)
            {
                MASTERdomain.theme.theme_mobile_version = version;
            }
            else
            {
                MASTERdomain.theme.theme_version = version;
            }

            repository.Save();

            return Json("Changes saved successfully".ToJsonOKMessage());
        }

        [HttpGet]
        public ActionResult Settings(bool ismobile)
        {
            // we need to parse settings file
            var theme = new ThemeHandler(MASTERdomain, ismobile);
            var settingHtml = theme.GetSettingsHtml();
            var settingsText = theme.GetSettingsText();
            var serializer = new JavaScriptSerializer();
            var settings = serializer.Deserialize<ThemeSettings>(settingsText);

            // set current theme settings if set
            if (!string.IsNullOrEmpty(MASTERdomain.theme.preset))
            {
                settings.current = MASTERdomain.theme.preset;
                settingsText = serializer.Serialize(settings);
            }

            var viewmodel = new ThemeSettingsViewModel
                                {
                                    SettingsHtml = settingHtml,
                                    SettingsJson = string.IsNullOrEmpty(settingsText) ? "{}": settingsText,
                                    ThemeTitle = MASTERdomain.theme.title,
                                    themeType = ismobile ? ThemeType.MOBILE : ThemeType.MAIN
                                };

            if (settings != null &&
                settings.presets != null && 
                settings.presets.Count != 0)
            {
                foreach (var key in settings.presets.Keys)
                {
                    viewmodel.presetList.Add(new SelectListItem()
                                                 {
                                                     Text = key,
                                                     Value = key,
                                                     Selected = key == MASTERdomain.theme.preset
                                                 });
                }
            }

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Settings(bool? save_preset, string save_preset_existing, string save_preset_custom, string preset, ThemeType themeType)
        {
            if (!save_preset.HasValue)
            {
                save_preset = false;
            }

            if (string.IsNullOrEmpty(preset))
            {
                preset = "Default";
            }

            var theme = new ThemeHandler(MASTERdomain, themeType == ThemeType.MOBILE);

            // handle files
            if (Request.Files.Count != 0)
            {
                foreach (var filename in Request.Files.AllKeys)
                {
                    var file = Request.Files[filename];
                    if (file == null || 
                        file.ContentLength == 0)
                    {
                        continue;
                    }
                    theme.SaveImage(filename, file.InputStream);
                }
            }

            // duplicate collection as original is read-only
            var values = new Dictionary<string, object>();
            foreach (string entry in Request.Form.Keys)
            {
                if (entry == "save_preset" ||
                    entry == "save_preset_existing" ||
                    entry == "save_preset_custom" ||
                    entry == "preset")
                {
                    continue;
                }

                var val = Request.Form.GetValues(entry).FirstOrDefault();
                bool boolval;
                if (bool.TryParse(val, out boolval))
                {
                    values[entry] = boolval;
                }
                else
                {
                    values[entry] = val;
                }
            }

            var selectedPreset = "";
            if (save_preset.Value)
            {
                if (save_preset_existing == "new" && !string.IsNullOrEmpty(save_preset_custom))
                {
                    // if new preset name is specified then use the defined name
                    selectedPreset = save_preset_custom;
                }
                else
                {
                    // otherwise we just use current preset
                    selectedPreset = preset;
                }
            }
            else
            {
                selectedPreset = preset;
            }


            // convert to json string
            // read current settings from file
            var old_settings = JsonConvert.DeserializeObject<ThemeSettings>(theme.GetSettingsText());

            // just set preset 
            old_settings.presets[selectedPreset] = values;
            old_settings.current = selectedPreset;

            // serialise and save to file
            var settings_string = JsonConvert.SerializeObject(old_settings, Formatting.Indented);
            theme.SaveSettings(settings_string);

            // save preset to db
            MASTERdomain.theme.preset = selectedPreset;

            // update versions
            switch (themeType)
            {
                case ThemeType.MAIN:
                    MASTERdomain.theme.theme_version = DateTime.UtcNow.Ticks.ToString("x");
                    break;
                case ThemeType.MOBILE:
                    MASTERdomain.theme.theme_mobile_version = DateTime.UtcNow.Ticks.ToString("x");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("themeType");
            }

            repository.Save();

            // need to invalidate any cached liquid assets
            CacheHelper.Instance.invalidate_dependency(DependencyType.liquid_assets, MASTERdomain.uniqueid);

            return RedirectToAction("Index");
        }
    }
}
