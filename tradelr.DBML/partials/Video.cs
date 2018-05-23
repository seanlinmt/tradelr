using System.Linq;

namespace tradelr.DBML
{
    public partial class TradelrRepository
    {
        public void AddVideo(string youtubeid, long thumbnailid, long subdomainid)
        {
            // check entry does not already exist
            var video = db.videos.Where(x => x.youtubeid == youtubeid &&
                x.subdomainid == subdomainid &&
                x.thumbnailid == thumbnailid).SingleOrDefault();
            if (video == null)
            {
                video = new video
                            {
                                thumbnailid = thumbnailid, 
                                subdomainid = subdomainid, 
                                youtubeid = youtubeid
                            };
                db.videos.InsertOnSubmit(video);
                db.SubmitChanges();
            }
        }
    }
}