namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.WindowsAzure.StorageClient;

    public class MockBlobService : IBlobService
    {
        private List<Blob> resources = new List<Blob>();

        public MockBlobService()
        {
            for (int k = 1; k <= 10; k++)
            {
                this.resources.Add(new Blob
                {
                    Name = string.Format("Blob Resource {0}", k)
                });
            }
        }

        public void DeleteBlobById(Guid id)
        {
            throw new NotImplementedException();
        }

        public CloudBlob GetBlob(Blob blob)
        {
            return new CloudBlob("http://localhost:10002/blob.jgp");
        }

        public string GetContentType(Blob blob)
        {
            throw new NotImplementedException();
        }

        public string GetBlobId(Blob blob)
        {
            throw new NotImplementedException();
        }

        public string GetBlobName(Blob blob)
        {
            throw new NotImplementedException();
        }

        public Blob GetBlobById(Guid id)
        {
            return this.resources.Where(r => r.BlobId == id).FirstOrDefault();
        }

        public IQueryable<Blob> GetBlobs()
        {
            return this.resources.AsQueryable();
        }

        public void UpdateDescription(Guid id, string name, string description)
        {
            throw new NotImplementedException();
        }

        public Guid UploadBlob(Blob blob, System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }

        public Guid UploadBlob(Blob blob, string filePath)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Blob> GetBlobsByPartialName(string name)
        {
            throw new NotImplementedException();
        }

        public Blob GetBlobByName(string name)
        {
            throw new NotImplementedException();
        }

        public Guid CreateBlob(Blob blob)
        {
            throw new NotImplementedException();
        }
    }
}
