using System;

namespace tradelr.Areas.dashboard.Models.orchard.filesystem.media {
    public interface IStorageFolder {
        string GetPath();
        string GetName();
        long GetSize();
        DateTime GetLastUpdated();
        IStorageFolder GetParent();
    }
}