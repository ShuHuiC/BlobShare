namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.DPE.BlobShare.Data.AzureStorage;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Win32;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public sealed class BlobService : IBlobService
    {
        private readonly BlobShareDataStoreEntities context;
        private readonly CloudStorageAccount storageAccount;
        private readonly string containerName;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public BlobService(CloudStorageAccount storageAccount, string containerName)
            : this(BlobShareDataStoreEntities.CreateInstance(), storageAccount, containerName)
        {
        }

        public BlobService(BlobShareDataStoreEntities context, CloudStorageAccount storageAccount, string containerName)
        {
            this.context = context;
            this.storageAccount = storageAccount;
            this.containerName = containerName;
        }

        public Guid UploadBlob(Blob blob, Stream stream)
        {
            if (blob.BlobId == Guid.Empty)
            {
                blob.BlobId = Guid.NewGuid();
            }

            IAzureBlobContainer<byte[]> container = GetBlobContainer(this.storageAccount, this.containerName);
            container.SaveStream(this.GetBlobId(blob), stream, null);          
            this.context.Blobs.AddObject(blob);
            this.context.SaveChanges();
            
            return blob.BlobId;
        }

        public Guid UploadBlob(Blob blob, string filePath)
        {
            if (blob.BlobId == Guid.Empty)
            {
                blob.BlobId = Guid.NewGuid();
            }

            IAzureBlobContainer<byte[]> container = GetBlobContainer(this.storageAccount, this.containerName);
            var contentType = this.GetContentType(blob);
            container.SaveFile(this.GetBlobId(blob), filePath, contentType);
            this.context.Blobs.AddObject(blob);
            this.context.SaveChanges();

            return blob.BlobId;
        }

        public Guid CreateBlob(Blob blob)
        {
            if (blob.BlobId == Guid.Empty)
            {
                blob.BlobId = Guid.NewGuid();
            }

            IAzureBlobContainer<byte[]> container = GetBlobContainer(this.storageAccount, this.containerName);
            this.context.Blobs.AddObject(blob);
            this.context.SaveChanges();

            return blob.BlobId;
        }

        public Blob GetBlobById(Guid id)
        {
            return this.context.Blobs.SingleOrDefault(br => br.BlobId == id);
        }

        public void DeleteBlobById(Guid id)
        {
            var resource = this.context.Blobs.SingleOrDefault(br => br.BlobId == id);
            
            this.context.Blobs.DeleteObject(resource);
            this.context.SaveChanges();

            CloudBlob blob = this.GetBlob(resource);
            blob.Delete();
        }

        public string GetBlobName(Blob blob)
        {
            return blob.BlobId.ToString() + Path.GetExtension(blob.OriginalFileName);
        }

        public string GetBlobId(Blob blob)
        {
            return this.GetBlobName(blob);
        }

        public CloudBlob GetBlob(Blob blob)
        {
            IAzureBlobContainer<byte[]> container = GetBlobContainer(this.storageAccount, this.containerName);
            return container.GetBlob(this.GetBlobId(blob));
        }

        public IQueryable<Blob> GetBlobs()
        {
            return this.context.Blobs.OrderBy(br => br.Name).AsQueryable();
        }

        public IQueryable<Blob> GetBlobsByPartialName(string name)
        {
            return this.context.Blobs.Where(br => br.Name.Contains(name)).OrderBy(br => br.Name);
        }

        public void UpdateDescription(Guid id, string name, string description)
        {
            var blob = this.context.Blobs.Where(br => br.BlobId == id).SingleOrDefault();
            var blobName = string.IsNullOrEmpty(name) ? Path.GetFileNameWithoutExtension(blob.OriginalFileName) : name;

            blob.Name = blobName;
            blob.Description = description;
            
            this.context.SaveChanges();
        }

        public string GetContentType(Blob blob)
        {
            string fileExt = System.IO.Path.GetExtension(blob.OriginalFileName).ToLowerInvariant();
            string contentType = null;

            RegistryKey fileExtKey = Registry.ClassesRoot.OpenSubKey(fileExt);

            if (fileExtKey != null && fileExtKey.GetValue("Content Type") != null)
            {
                contentType = fileExtKey.GetValue("Content Type").ToString();
            }

            return contentType;
        }

        public Blob GetBlobByName(string name)
        {
            return this.context.Blobs.Where(r => r.Name == name).FirstOrDefault();
        }

        private static IAzureBlobContainer<byte[]> GetBlobContainer(CloudStorageAccount account, string containerName)
        {
            var container = new AzureBlobContainer<byte[]>(account, containerName);
            container.EnsureExist();

            return container;
        }
    }
}