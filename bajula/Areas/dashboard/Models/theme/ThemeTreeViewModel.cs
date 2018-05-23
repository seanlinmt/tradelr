using System.IO;
using tradelr.Models;

namespace tradelr.Areas.dashboard.Models.theme
{
    public class ThemeTreeViewModel : BaseViewModel
    {
        public DirectoryInfo dir { get; set; }
        public string rootPath { get; set; }

        public ThemeTreeViewModel()
        {
            
        }

        public ThemeTreeViewModel(BaseViewModel baseviewmodel) : base(baseviewmodel)
        {
            
        }
    }
}