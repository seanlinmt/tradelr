using System;
using System.IO;
using System.Net;
using com.mosso.cloudfiles.domain.response.Interfaces;

namespace com.mosso.cloudfiles.domain.request.Interfaces
{
    public interface ICloudFilesRequest
    {
        ICloudFilesResponse GetResponse();
        Uri RequestUri { get; }
        string Method { get; set; }
        WebHeaderCollection Headers { get; }

        long ContentLength { get;  }
        int RangeTo { get; set; }
        int RangeFrom { get; set; }
        string ContentType { get; set; }
        DateTime IfModifiedSince { get; set; }
        string ETag { get;  }
        bool AllowWriteStreamBuffering { get; set; }
        bool SendChunked { get; set; }
        Stream ContentStream { get; }
        Stream GetRequestStream();
        void SetContent(Stream stream, Connection.ProgressCallback progress);
    }
}