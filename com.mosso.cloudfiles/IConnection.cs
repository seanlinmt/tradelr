using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using com.mosso.cloudfiles.domain;
using com.mosso.cloudfiles.domain.request;

namespace com.mosso.cloudfiles
{
    public interface IConnection
    {
        AccountInformation GetAccountInformation();
        string GetAccountInformationJson();
        XmlDocument GetAccountInformationXml();
        void CreateContainer(string containerName);
        void DeleteContainer(string continerName);
        List<string> GetContainers();
        List<string> GetContainerItemList(string containerName);
        List<string> GetContainerItemList(string containerName, Dictionary<GetItemListParameters, string> parameters);
        Container GetContainerInformation(string containerName);
        string GetContainerInformationJson(string containerName);
        XmlDocument GetContainerInformationXml(string containerName);
        void PutStorageItemAsync(string containerName, Stream storageStream, string remoteStorageItemName);
        void PutStorageItemAsync(string containerName, string localStorageItemName);
        void GetStorageItemAsync(string containerName, string storageItemName, string localItemName);
        void PutStorageItem(string containerName, string localFilePath, Dictionary<string, string> metadata);
        void PutStorageItem(string containerName, string localFilePath);
        void PutStorageItem(string containerName, Stream storageStream, string remoteStorageItemName);
        void PutStorageItem(string containerName, Stream storageStream, string remoteStorageItemName, Dictionary<string, string> metadata);
        void DeleteStorageItem(string containerName, string storageItemname);
        StorageItem GetStorageItem(string containerName, string storageItemName);
        void GetStorageItem(string containerName, string storageItemName, string localFileName);
        StorageItem GetStorageItem(string containerName, string storageItemName, Dictionary<RequestHeaderFields, string> requestHeaderFields);
        void GetStorageItem(string containerName, string storageItemName, string localFileName, Dictionary<RequestHeaderFields, string> requestHeaderFields);
        StorageItemInformation GetStorageItemInformation(string containerName, string storageItemName);
        void SetStorageItemMetaInformation(string containerName, string storageItemName, Dictionary<string, string> metadata);
        List<string> GetPublicContainers();
        Uri MarkContainerAsPublic(string containerName);
        Uri MarkContainerAsPublic(string containerName, int timeToLiveInSeconds);
        void MarkContainerAsPrivate(string containerName);
       
        Container GetPublicContainerInformation(string containerName);
        void MakePath(string containerName, string path);
        IAccount Account { get; }

        /// <summary>
        /// The storage url used to interact with cloud files
        /// </summary>
        string StorageUrl { get;  set; }

        /// <summary>
        /// the session based token used to ensure the user was authenticated
        /// </summary>
        string AuthToken { get;  set; }


        void SetDetailsOnPublicContainer(string publiccontainer, bool loggingenabled, int ttl, string referreracl, string useragentacl );
        XmlDocument GetPublicAccountInformationXML();
        string GetPublicAccountInformationJSON();
    }
}