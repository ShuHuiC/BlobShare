namespace Microsoft.Samples.DPE.BlobShare.Data.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Script.Serialization;
    using System.Xml.Serialization;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class AzureBlobContainer<T> : IAzureBlobContainer<T>
    {
        private readonly CloudStorageAccount account;
        private readonly CloudBlobContainer container;

        public AzureBlobContainer(CloudStorageAccount account)
            : this(account, typeof(T).Name.ToLowerInvariant())
        {
        }

        public AzureBlobContainer(CloudStorageAccount account, string containerName)
        {
            this.account = account;

            var client = this.account.CreateCloudBlobClient();
            client.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(5));

            this.container = client.GetContainerReference(containerName);
        }

        public void EnsureExist()
        {
            this.container.CreateIfNotExist();
        }

        public void EnsureExist(bool publicContainer)
        {
            this.container.CreateIfNotExist();
            var permissions = new BlobContainerPermissions();

            if (publicContainer)
            {
                permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            }

            this.container.SetPermissions(permissions);
        }

        public void Save(string objId, T obj)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = "application/json";
            var serializer = new JavaScriptSerializer();
            blob.UploadText(serializer.Serialize(obj));
        }

        public void SaveAsXml(string objId, T obj)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = "text/xml";
            var serializer = new XmlSerializer(typeof(T));

            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                blob.UploadText(writer.ToString());
            }
        }

        public string SaveFile(string objId, string filePath, string contentType)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = contentType;
            blob.UploadFile(filePath);
            return blob.Uri.ToString();
        }

        public string SaveFile(string objId, string filePath, string contentType, TimeSpan timeOut)
        {
            TimeSpan currentTimeOut = this.container.ServiceClient.Timeout;
            this.container.ServiceClient.Timeout = timeOut;
            string result = this.SaveFile(objId, filePath, contentType);
            this.container.ServiceClient.Timeout = currentTimeOut;
            return result;
        }

        public string SaveByteArray(string objId, byte[] content, string contentType)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.Properties.ContentType = contentType;
            blob.UploadByteArray(content);
            return blob.Uri.ToString();
        }

        public string SaveByteArray(string objId, byte[] content, string contentType, TimeSpan timeOut)
        {
            TimeSpan currentTimeOut = this.container.ServiceClient.Timeout;
            this.container.ServiceClient.Timeout = timeOut;
            string result = this.SaveByteArray(objId, content, contentType);
            this.container.ServiceClient.Timeout = currentTimeOut;
            return result;
        }

        public string SaveStream(string objId, Stream stream, IDictionary<string, string> metadata)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);

            if (metadata != null)
            {
                foreach (string key in metadata.Keys)
                {
                    blob.Metadata[key] = metadata[key];
                }
            }

            blob.UploadFromStream(stream);
            return blob.Uri.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "If we dispose the stream the clien won't be able to use it")]
        public Stream GetFile(string objId)
        {
            Stream stream = new MemoryStream();
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.DownloadToStream(stream);
            stream.Seek(0, 0);
            return stream;
        }

        public CloudBlob GetBlob(string objId)
        {
            return this.container.GetBlobReference(objId);
        }

        public IEnumerable<CloudBlob> ListBlobs(string matchingId)
        {
            var list = new List<CloudBlob>();

            BlobRequestOptions options = new BlobRequestOptions();
            options.UseFlatBlobListing = true;

            foreach (var blobItem in this.container.ListBlobs(options))
            {
                var blob = (CloudBlob)blobItem;
                if (blob.Uri.Segments[blob.Uri.Segments.Length - 1].StartsWith(matchingId, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(blob);
                }
            }

            return list;
        }

        public T Get(string objId)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            try
            {
                var serializer = new JavaScriptSerializer();
                return serializer.Deserialize<T>(blob.DownloadText());
            }
            catch (StorageClientException)
            {
                return default(T);
            }
        }

        public T GetFromXml(string objId)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                T result;

                using (var reader = new StringReader(blob.DownloadText()))
                {
                    result = (T)serializer.Deserialize(reader);
                }

                return result;
            }
            catch (StorageClientException)
            {
                return default(T);
            }
        }

        public void Delete(string objId)
        {
            CloudBlob blob = this.container.GetBlobReference(objId);
            blob.DeleteIfExists();
        }
    }
}