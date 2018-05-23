using System;
using com.mosso.cloudfiles.domain.request.Interfaces;
using com.mosso.cloudfiles.utils;

namespace com.mosso.cloudfiles.domain.request
{
//    public class SetAclOnPublicContainer : IAddToWebRequest
//    {
//        private readonly string _containername;
//        private readonly string _storageurl;
//        private readonly string _agentacl;
//        private readonly string _refacl;
//
//        public SetAclOnPublicContainer(string containername,string cdnmanagementurl , string agentacl, string refacl)
//        {
//            _containername = containername;
//            _storageurl = cdnmanagementurl;
//            _agentacl = agentacl;
//            _refacl = refacl;
//        }
//
//        public Uri CreateUri()
//        {
//             return new Uri(_storageurl + "/" + _containername.Encode());
//        }
//
//        public void Apply(ICloudFilesRequest request)
//        {
//            request.Method = "POST";
//            request.Headers.Add("X-User-Agent-ACL", _agentacl);
//            request.Headers.Add("X-Referrer-ACL", _refacl);
//        }
//    }
}