namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IBlobSetService
    {
        void AddBlobToSet(Guid setId, Guid blobId);

        void AddBlobToSet(Guid setId, string blobName);

        void AddBlobToSet(string setName, Guid blobId);

        void RemoveBlobFromSet(Guid setId, Guid blobId);

        IEnumerable<BlobSet> GetBlobSets();

        BlobSet GetBlobSetById(Guid id);

        void CreateBlobSet(BlobSet set);

        void UpdateBlobSet(BlobSet set);

        IQueryable<BlobSet> GetBlobSetsByPartialName(string name);

        BlobSet GetBlobSetByName(string name);

        void DeleteBlobSet(Guid setId);
    }
}