using System;
using System.Net;

namespace com.mosso.cloudfiles.domain.request.Interfaces
{
    public interface IAddToWebRequest
    {
         Uri CreateUri();
         void Apply(ICloudFilesRequest request);
    }
}