using System.Collections.Generic;
using tradelr.Common.Library.Imaging;
using tradelr.DBML;
using tradelr.DBML.Helper;

namespace tradelr.Models.offline.tables
{
    public class PhotosColumn : IColumn
    {
        public CFlag cflag { get; set; }
        public long? id { get; set; }
        public long? serverid { get; set; }

        public string url { get; set; }
        public long contextid { get; set; }
        public string type { get; set; }
    }

    public static class PhotosColhelper
    {
        public static PhotosColumn ToSyncModel(this image v, CFlag flag, long? offlineid = null)
        {
            return new PhotosColumn
            {
                cflag = flag,
                serverid = v.id,
                url = v.url.ToDataUri(),
                contextid = v.contextID,
                type = v.imageType,
                id = offlineid
            };
        }

        public static List<PhotosColumn> ToSyncModel(this IEnumerable<image> values, CFlag flag, long? offlineid = null)
        {
            var result = new List<PhotosColumn>();
            foreach (var v in values)
            {
                result.Add(v.ToSyncModel(flag));
            }
            return result;
        }
    }
}