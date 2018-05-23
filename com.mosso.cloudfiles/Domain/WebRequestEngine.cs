using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.domain.response.Interfaces;

namespace com.mosso.cloudfiles.domain.request
{
    public interface IWebRequestEngine
    {
        ICloudFilesResponse Submit(ICloudFilesRequest request);
       
    }
}