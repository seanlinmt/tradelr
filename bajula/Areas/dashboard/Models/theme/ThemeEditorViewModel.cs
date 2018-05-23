using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeEditorViewModel
    {
        public ThemeDirectory asset_folder { get; set; }
        public ThemeDirectory templates_folder { get; set; }
        public ThemeDirectory snippets_folder { get; set; }
        public ThemeDirectory layout_folder { get; set; }
        public ThemeDirectory config_folder { get; set; }
        public ThemeType themeType { get; set; }   // mobile or main

        public ThemeEditorViewModel()
        {
            asset_folder = new ThemeDirectory(){ foldername = "assets"};
            templates_folder = new ThemeDirectory() { foldername = "templates"};
            snippets_folder = new ThemeDirectory() { foldername = "snippets"};
            layout_folder = new ThemeDirectory() { foldername = "layout"};
            config_folder = new ThemeDirectory() { foldername = "config"};
        }
    }
}