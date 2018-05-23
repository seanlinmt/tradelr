using System;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.exceptions;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
    public class SetLoggingToContainerRequest : IAddToWebRequest
    {
        private readonly string _publiccontainer;
        private readonly string _cdnManagmentUrl;
        private readonly bool _loggingenabled;

        public SetLoggingToContainerRequest(string publiccontainer, string cdnManagmentUrl, bool loggingenabled)
        {
            _publiccontainer = publiccontainer;
            _cdnManagmentUrl = cdnManagmentUrl;
            _loggingenabled = loggingenabled;
            if (String.IsNullOrEmpty(publiccontainer))
                throw new ArgumentNullException();

            if (!ContainerNameValidator.Validate(publiccontainer)) throw new ContainerNameException();
           
        }

        public Uri CreateUri()
        {
            return  new Uri(_cdnManagmentUrl + "/" + _publiccontainer.Encode());
        }

        public void Apply(ICloudFilesRequest request)
        {
            request.Method = "POST";
            string enabled = "False";
            if (_loggingenabled)
                enabled = "True";
            request.Headers.Add("X-Log-Retention", enabled);
           
        }
    }
}
