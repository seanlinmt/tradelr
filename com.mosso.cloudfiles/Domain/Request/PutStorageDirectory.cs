using System;
using System.IO;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    public class PutStorageDirectory:IAddToWebRequest
    {
        private readonly string _storageurl;
        private readonly string _containername;
        private readonly string _objname;

        public PutStorageDirectory(string storageurl, string containername, string objname)
        {
            _storageurl = storageurl;
            _containername = containername;
            _objname = objname;
        }

        public Uri CreateUri()
        {
             return new Uri(_storageurl + "/" + _containername.Encode() + "/" + _objname.StripSlashPrefix().Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "PUT";
            request.ContentType = "application/directory";
            request.SetContent(new MemoryStream(new byte[0]), delegate { }); 
        }
    }
}