///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using com.mosso.cloudfiles.exceptions;

namespace com.mosso.cloudfiles.domain
{
    public interface IContainer
    {
        int ObjectCount { get; }
        long BytesUsed { get; }
        string Name { get; }
        IObject AddObject(string objectName);
        IObject AddObject(string objectName, Dictionary<string, string> metadata);
        IObject AddObject(Stream localObjectStream, string remoteObjectName);
        IObject AddObject(Stream localObjectStream, string remoteObjectName, Dictionary<string, string> metadata);
        void DeleteObject(string objectName);
        void MarkAsPublic();
        bool ObjectExists(string objectName);
        string[] GetObjectNames();
        string[] GetObjectNames(Dictionary<GetItemListParameters, string> parameters);
        Uri PublicUrl { get; set; }
        string JSON { get; }
        XmlDocument XML { get; }
    }

    public class CF_Container : IContainer
    {
        private readonly IConnection connection;
        protected List<IObject> objects;
        protected int objectCount;
        protected long bytesUsed;

        public CF_Container(IConnection connection, string containerName)
        {
            objects = new List<IObject>();
            Name = containerName;
            this.connection = connection;
        }

        public string Name { get; private set; }

        public int ObjectCount
        {
            get
            {
                CloudFilesHeadContainer();
                return objectCount;
            }
        }

        public long BytesUsed
        {
            get
            {
                CloudFilesHeadContainer();
                return bytesUsed;
            }
        }

        public string JSON
        {
            get
            {
                return CloudFileContainerInformationJson();
            }
        }

        public XmlDocument XML
        {
            get
            {
                return CloudFileContainerInformationXml();
            }
        }

        public string[] GetObjectNames()
        {
            return GetObjectNames(new Dictionary<GetItemListParameters, string>());
        }

        public string[] GetObjectNames(Dictionary<GetItemListParameters, string> parameters)
        {
            return CloudFilesGetContainer(parameters);
        }

        public Uri PublicUrl { get; set; }

        public IObject AddObject(string objectName)
        {
            return AddObject(objectName, new Dictionary<string, string>());
        }

        public IObject AddObject(string objectName, Dictionary<string, string> metadata)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            CloudFilesPutObject(objectName, metadata);
            IObject @object = PopulateObjectProperties(objectName, metadata);

            if (objects.Find(x => x.Name == objectName) == null)
                objects.Add(@object);

            return @object;
        }

        public IObject AddObject(Stream localObjectStream, string remoteObjectName)
        {
            return AddObject(localObjectStream, remoteObjectName, new Dictionary<string, string>());
        }

        public IObject AddObject(Stream localObjectStream, string remoteObjectName, Dictionary<string, string> metadata)
        {
            if (string.IsNullOrEmpty(remoteObjectName)
                || localObjectStream == null)
                throw new ArgumentNullException();

            CloudFilesPutObject(localObjectStream, remoteObjectName, metadata);
            IObject @object = PopulateObjectProperties(remoteObjectName, metadata);

            if (objects.Find(x => x.Name == remoteObjectName) == null)
                objects.Add(@object);

            return @object;
        }

        private CF_Object PopulateObjectProperties(string objectName, Dictionary<string, string> metadata)
        {
            CF_Object @object = new CF_Object(connection, objectName, metadata);            
            @object.ContainerName = Name;
            @object.PublicUrl = PublicUrl;
            return @object;
        }

        public void DeleteObject(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            CloudFilesDeleteObject(objectName);
            if(objects.Find(x => x.Name == objectName) == null)
                throw new StorageItemNotFoundException();
            objects.Remove(objects.Find(x => x.Name == objectName));
        }


        public void MarkAsPublic()
        {
            CloudFilesMarkContainerPublic();
        }

        public bool ObjectExists(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                throw new ArgumentNullException();

            return CloudFilesHeadObject(objectName)
                   && objects.Contains(objects.Find(x => x.Name == objectName));
        }


        protected virtual string CloudFileContainerInformationJson()
        {
            return connection.GetContainerInformationJson(Name);
        }

        protected virtual XmlDocument CloudFileContainerInformationXml()
        {
            return connection.GetContainerInformationXml(Name);
        }

        protected virtual string[] CloudFilesGetContainer(Dictionary<GetItemListParameters, string> parameters)
        {
            return connection.GetContainerItemList(Name, parameters).ToArray();
        }

        protected virtual void CloudFilesHeadContainer()
        {
            var containerInformation = connection.GetContainerInformation(Name);
            bytesUsed = containerInformation.ByteCount;
            objectCount = int.Parse(containerInformation.ObjectCount.ToString());
        }

        protected virtual void CloudFilesPutObject(string objectName, Dictionary<string, string> metadata)
        {
            connection.PutStorageItem(Name, objectName, metadata);
        }

        protected virtual void CloudFilesPutObject(Stream localObjectStream, string remoteObjectName, Dictionary<string, string> metadata)
        {
            connection.PutStorageItem(Name, localObjectStream, remoteObjectName, metadata);
        }

        protected virtual void CloudFilesDeleteObject(string objectName)
        {
            connection.DeleteStorageItem(Name, objectName);
        }

        protected virtual bool CloudFilesHeadObject(string objectName)
        {
            try
            {
                var @object = connection.GetStorageItemInformation(Name, objectName);
                return @object != null;
            }
            catch (ContainerNameException)
            {
                return false;
            }
			catch(StorageItemNameException)
			{
				return false;	
			}
			catch(StorageItemNotFoundException)
			{
				return false;
			}
			
        }

        protected virtual void CloudFilesMarkContainerPublic()
        {
            PublicUrl = connection.MarkContainerAsPublic(Name);
        }
    }
}