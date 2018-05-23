using System;
using System.Collections.Generic;
using System.IO;
using tradelr.Areas.dashboard.Models.orchard.media.models;

namespace tradelr.Areas.dashboard.Models.orchard.media.helpers {
    public static class MediaHelpers {
        public static IEnumerable<FolderNavigation> GetFolderNavigationHierarchy(string mediaPath) {
            var navigations = new List<FolderNavigation>();
            if (String.IsNullOrEmpty(mediaPath)) {
                return navigations;
            }
            if ( !mediaPath.Contains(Path.DirectorySeparatorChar.ToString()) && !mediaPath.Contains(Path.AltDirectorySeparatorChar.ToString()) ) {
                navigations.Add(new FolderNavigation { FolderName = mediaPath, FolderPath = mediaPath });
                return navigations;
            }

            string[] navigationParts = mediaPath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = String.Empty;
            foreach (string navigationPart in navigationParts) {
                currentPath = (string.IsNullOrEmpty(currentPath) ? navigationPart : currentPath + "\\" + navigationPart);
                navigations.Add(new FolderNavigation { FolderName = navigationPart, FolderPath = currentPath });
            }

            return navigations;
        }
    }
}