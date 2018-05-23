using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using DotLiquid;
using tradelr.Areas.dashboard.Models.theme;
using tradelr.DBML;
using tradelr.Library.Constants;
using tradelr.Models.liquid.filters;
using tradelr.Models.mobile;

namespace tradelr.Models.liquid
{
    public class LiquidTemplateBase
    {
        protected Template content_template { get; set; }
        protected Hash parameters { get; set; }
        protected Hash values { get; set; }
        protected string themepath { get; set; }
        public ThemeHandler handler { get; set; }

        public LiquidTemplateBase(MASTERsubdomain sd, bool isMobile)
        {
            parameters = new Hash();
            values = new Hash();

            handler = new ThemeHandler(sd, isMobile);
            themepath = handler.GetThemeUrl();

            AddFilterValues("asset_url", themepath);
            AddFilterValues("theme_version", isMobile? sd.theme.theme_mobile_version:sd.theme.theme_version);
        }

        public void AddFilterValues(string name, object obj)
        {
            values.Add(name, obj);
        }

        public void AddParameters(string name, object obj)
        {
            parameters.Add(name, obj);
        }

        public string ReadTemplateFile(string physicalpPath)
        {
            using (var reader = File.OpenText(physicalpPath))
            {
                return reader.ReadToEnd();
            }
        }

        public Dictionary<string, object> ReadThemeSettings(string physicalPath)
        {
            Dictionary<string, object> current = null;

            if (File.Exists(physicalPath))
            {
                var settingsText = File.ReadAllText(physicalPath);
                var serializer = new JavaScriptSerializer();
                var settings = serializer.Deserialize<ThemeSettings>(settingsText);
                if (settings != null)
                {
                    if (settings.presets != null &&
                        settings.presets.Count != 0)
                    {
                        current = settings.presets[settings.current];
                    }
                    else
                    {
                        current = settings.current;
                    }
                }
            }
            
            return current;
        }

        public void InitContentTemplate(string templateString)
        {
            content_template = CreateTemplate(templateString);
        }

        protected Template CreateTemplate(string templateString)
        {
            return Template.Parse(templateString);
        }

        /// <summary>
        /// only handles css.liquid files at the moment
        /// </summary>
        /// <returns></returns>
        public string RenderBasicNoHeader()
        {
            var p = new RenderParameters()
            {
                LocalVariables = parameters,
                Values = values,
                Filters = new[]
                                          {
                                              typeof (TextFilter), 
                                              typeof(InputFilter), 
                                              typeof(UrlFilter),
                                              typeof(MoneyFilter)
                                          },
            };

            return content_template.Render(p);
        }

        public Stream RenderBasicToStreamNoHeader()
        {
            var p = new RenderParameters()
            {
                LocalVariables = parameters,
                Values = values,
                Filters = new[]
                                          {
                                              typeof (TextFilter), 
                                              typeof(InputFilter), 
                                              typeof(UrlFilter),
                                              typeof(MoneyFilter)
                                          },
            };
            var ms = new MemoryStream();
            content_template.Render(ms, p);
            ms.Position = 0;
            return ms;
        }
    }
}