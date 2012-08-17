namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class MockBlobSetService : IBlobSetService
    {
        private List<BlobSet> sets = new List<BlobSet>();

        public MockBlobSetService()
        {
            for (int k = 1; k <= 10; k++) 
            {
                this.sets.Add(new BlobSet() { BlobSetId = Guid.NewGuid(), Name = string.Format("Resource Set {0}", k) });
            }
        }

        public void AddBlobToSet(Guid setId, Guid blobId)
        {
            throw new NotImplementedException();
        }

        public void RemoveBlobFromSet(Guid setId, Guid blobId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BlobSet> GetBlobSets()
        {
            return this.sets;
        }

        public Data.Model.BlobSet GetBlobSetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public void CreateBlobSet(BlobSet set)
        {
            throw new NotImplementedException();
        }

        public void UpdateBlobSet(BlobSet set)
        {
            throw new NotImplementedException();
        }

        public void AddBlobToSet(Guid setId, string blobName)
        {
            throw new NotImplementedException();
        }

        public IQueryable<BlobSet> GetBlobSetsByPartialName(string name)
        {
            throw new NotImplementedException();
        }

        public BlobSet GetBlobSetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void AddBlobToSet(string setName, Guid blobId)
        {
            throw new NotImplementedException();
        }

        public void DeleteBlobSet(Guid setId)
        {
            throw new NotImplementedException();
        }
    }
}
