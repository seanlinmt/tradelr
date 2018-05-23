///
/// See COPYING file for licensing information
///

using System;
using System.Collections.Generic;
using System.Xml;
using com.mosso.cloudfiles.exceptions;

namespace com.mosso.cloudfiles.domain
{
    public interface IAccount
    {
        int ContainerCount { get; }
        long BytesUsed { get; }
        IContainer CreateContainer(string containerName);
        void DeleteContainer(string containerName);
        IContainer GetContainer(string containerName);
        bool ContainerExists(string containerName);
        string JSON { get; }
        XmlDocument XML { get; }
    }

    public class CF_Account : IAccount
    {
        private readonly IConnection connection;
        protected List<IContainer> containers;
        protected int containerCount;
        protected long bytesUsed;

        public CF_Account(IConnection connection)
        {
            this.connection = connection;
            containers = new List<IContainer>();
        }

        public Uri StorageUrl
        {
            get { return new Uri(connection.StorageUrl); }
        }

        public string AuthToken
        {
            get { return connection.AuthToken; }
        }

        public int ContainerCount
        {
            get
            {
                CloudFilesHeadAccount();
                return containerCount;
            }
        }

        public long BytesUsed
        {
            get
            {
                CloudFilesHeadAccount();
                return bytesUsed;
            }
        }

        public Uri CDNManagementUrl { get; set; }
        public UserCredentials UserCreds { get; set; }

        public string JSON
        {
            get
            {
                return CloudFileAccountInformationJson();
            }
        }

        public XmlDocument XML
        {
            get
            {
                return CloudFileAccountInformationXml();
            }
        }

        public IContainer CreateContainer(string containerName)
        {
            CloudFileCreateContainer(containerName);

            IContainer container = new CF_Container(connection, containerName);
            containers.Add(container);

            return container;
        }

        public bool ContainerExists(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
                throw new ArgumentNullException();

            return CloudFilesHeadContainer(containerName)
                   && containers.Contains(containers.Find(x => x.Name == containerName));
        }

        public void DeleteContainer(string containerName)
        {
            CloudFilesDeleteContainer(containerName);
            if (containers.Find(x => x.Name == containerName) == null)
                throw new ContainerNotFoundException();
            containers.Remove(containers.Find(x => x.Name == containerName));
        }

        public IContainer GetContainer(string containerName)
        {
            CloudFilesGetContainer(containerName);
            return containers.Find(x => x.Name == containerName);
        }

        protected virtual string CloudFileAccountInformationJson()
        {
            return connection.GetAccountInformationJson();
        }

        protected virtual XmlDocument CloudFileAccountInformationXml()
        {
            return connection.GetAccountInformationXml();
        }

        protected virtual void CloudFileCreateContainer(string containerName)
        {
            connection.CreateContainer(containerName);
        }

        protected virtual void CloudFilesHeadAccount()
        {
            var accountInformation = connection.GetAccountInformation();
            
            containerCount = accountInformation.ContainerCount;
            bytesUsed = accountInformation.BytesUsed;
        }

        protected virtual bool CloudFilesHeadContainer(string containerName)
        {
            try
            {
                return connection.GetContainerInformation(containerName) != null;
                
            }
            catch(ContainerNotFoundException)
            {
                return false;
            }
        }

        protected virtual void CloudFilesDeleteContainer(string containerName)
        {
            connection.DeleteContainer(containerName);
        }

        protected virtual void CloudFilesGetContainer(string containerName)
        {
            throw new NotImplementedException();
        }
    }
}