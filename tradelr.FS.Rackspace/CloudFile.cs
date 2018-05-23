using System;
using System.IO;
using com.mosso.cloudfiles;
using com.mosso.cloudfiles.domain;
using tradelr.Common;
using tradelr.Logging;


namespace tradelr.FS.Rackspace
{
    public class CloudFile
    {
        private readonly Connection connection;
        private readonly string containerName;

        public CloudFile(string container_name)
        {
            // get storage account
            var userCredentials = new UserCredentials(Constants.RACKSPACE_CLOUD_USERNAME, Constants.RACKSPACE_CLOUD_APIKEY);
            connection = new Connection(userCredentials);

            // create container
            containerName = container_name;
            bool containerFound = false;
            foreach (var container in connection.GetContainers())
            {
                if (container == container_name)
                {
                    containerFound = true;
                }
            }
            if (!containerFound)
            {
                connection.CreateContainer(container_name);
            }

            // mark public
            connection.MarkContainerAsPublic(container_name);
        }

        public string GetBlobItemUri(string path)
        {
            var item = connection.GetPublicContainerInformation(containerName);
            return string.Concat(item.CdnUri, "/", path);
        }

        public bool DoesBlobItemExists(string path)
        {
            try
            {
                connection.GetStorageItemInformation(containerName, path);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public Stream GetBlobContentStream(string path)
        {
            var content = connection.GetStorageItem(containerName, path);
            if (content == null)
            {
                return null;
            }
            return content.ObjectStream;
        }

        public void DeleteBlobItem(string path)
        {
            connection.DeleteStorageItem(containerName, path);
        }

        

        public bool AddBlobItemAsync(string sourcePath)
        {
            try
            {
                connection.PutStorageItemAsync(containerName, sourcePath);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return false;
            }
            return true;
        }

        public bool AddBlobItem(string dest, Stream ms)
        {
            try
            {
                connection.PutStorageItem(containerName, ms, dest);
            }
            catch (Exception ex)
            {
                Syslog.Write(ex);
                return false;
            }
            return true;
        }

        public void CopyContent(string src, string dest)
        {
            var srcblob = connection.GetStorageItem(containerName, src);
            connection.PutStorageItem(containerName, srcblob.ObjectStream, dest);
        }

    }
}