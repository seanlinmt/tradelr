#if AZURE
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using tradelr.Libraries.reporting;
using tradelr.Libraries.reporting;

namespace tradelr.Libraries.CloudStorage.Azure
{
    public class AzureBlob
    {

        // no singleton because there's also the lucene index
        // public static readonly AzureBlob Instance = new AzureBlob(Constants.AZURE_CONTAINER_IMAGES);

        private readonly CloudBlobClient blobStorage;
        private readonly CloudBlobContainer container;

        public AzureBlob(string containerName) : this(containerName, BlobContainerPublicAccessType.Off)
        {
            
        }

        public AzureBlob(string containerName, BlobContainerPublicAccessType access)
        {
            Trace.TraceInformation("AzureBlob Enter ...");

            // get storage account
            var storageAccount = CloudStorageAccount.FromConfigurationSetting("DataConnectionString");
            
            // get the blobStorage
            blobStorage = storageAccount.CreateCloudBlobClient();

            // create container
            container = blobStorage.GetContainerReference(containerName);
            container.CreateIfNotExist();
            
            // set container permissions
            var permissions = container.GetPermissions();
            permissions.PublicAccess = access;
            container.SetPermissions(permissions);
            Trace.TraceInformation("AzureBlob Exit ...");
        }

        public string GetBlobItemUri(string path)
        {
            var item = container.GetBlobReference(path);
            return item.Uri.ToString();
        }

        public bool DoesBlobItemExists(string path)
        {
            var blob = container.GetBlobReference(path);
            // try to get blob
            try
            {
                blob.FetchAttributes();
            }
            catch (StorageClientException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                throw;
            }
            return true;
        }

        public Stream GetBlobContentStream(string path)
        {
            var ms = new MemoryStream();
            var content = container.GetBlockBlobReference(path);
            if (content == null)
            {
                Syslog.Write(ErrorLevel.CRITICAL, "Blob content not found: " + path);
                return null;
            }
            content.DownloadToStream(ms);
            return ms;
        }

        public void DeleteBlobItem(string path)
        {
            var content = container.GetDirectoryReference(path);
            if (content != null)
            {
                var blobs = content.ListBlobs();
                foreach (var blob in blobs)
                {
                    blob.Container.Delete();
                }
            }
        }

        public CloudBlobContainer GetBlobContainer()
        {
            return container;
        }

        public bool AddBlobItem(string dest, Stream ms)
        {
            try
            {
                var content = container.GetBlockBlobReference(dest);
                content.UploadFromStream(ms);  
            }
            catch (Exception ex)
            {
                Syslog.Write(ErrorLevel.CRITICAL, ex.Message);
                return false;
            }
            return true;
        }

        public void CopyContent(string src, string dest)
        {
            var srcblob = container.GetBlockBlobReference(src);
            var destblob = container.GetBlockBlobReference(dest);

            destblob.CopyFromBlob(srcblob);
        }

    }
}

#endif