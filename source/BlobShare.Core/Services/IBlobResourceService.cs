namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.WindowsAzure.StorageClient;

    public interface IBlobService
    {
        void DeleteBlobById(Guid id);

        CloudBlob GetBlob(Blob blob);

        string GetContentType(Blob blob);

        string GetBlobId(Blob blob);

        string GetBlobName(Blob blob);

        Blob GetBlobById(Guid id);

        Blob GetBlobByName(string name);

        IQueryable<Blob> GetBlobs();

        IQueryable<Blob> GetBlobsByPartialName(string name);

        void UpdateDescription(Guid id, string name, string description);

        Guid UploadBlob(Blob blob, Stream stream);

        Guid UploadBlob(Blob blob, string filePath);

        Guid CreateBlob(Blob blob);
    }
}