namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class BlobSetService : IBlobSetService
    {
        private BlobShareDataStoreEntities context;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public BlobSetService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        public BlobSetService(BlobShareDataStoreEntities context)
        {
            this.context = context;
        }

        public void AddBlobToSet(Guid setId, Guid blobId)
        {
            BlobSet set = this.context.BlobSets.Where(ls => ls.BlobSetId == setId).FirstOrDefault();
            Blob blob = this.context.Blobs.Where(r => r.BlobId == blobId).FirstOrDefault();
            set.Blobs.Add(blob);
            this.context.SaveChanges();
        }

        public void AddBlobToSet(Guid setId, string blobName)
        {
            BlobSet set = this.context.BlobSets.Where(ls => ls.BlobSetId == setId).FirstOrDefault();
            Blob blob = this.context.Blobs.Where(r => r.Name == blobName).FirstOrDefault();

            if (set != null && blob != null && !set.Blobs.Any(br => br.BlobId == blob.BlobId))
            {
                set.Blobs.Add(blob);
                this.context.SaveChanges();
            }
        }

        public void AddBlobToSet(string setName, Guid blobId)
        {
            Blob blob = this.context.Blobs.Where(br => br.BlobId == blobId).FirstOrDefault();
            BlobSet set = this.context.BlobSets.Where(s => s.Name == setName).FirstOrDefault();

            if (set != null && blob != null && !set.Blobs.Any(br => br.BlobId == blob.BlobId))
            {
                set.Blobs.Add(blob);
                this.context.SaveChanges();
            }
        }

        public void RemoveBlobFromSet(Guid setId, Guid blobId)
        {
            BlobSet set = this.context.BlobSets.Where(ls => ls.BlobSetId == setId).FirstOrDefault();
            Blob blob = set.Blobs.Where(br => br.BlobId == blobId).FirstOrDefault();
            set.Blobs.Remove(blob);
            this.context.SaveChanges();
        }

        public IEnumerable<BlobSet> GetBlobSets()
        {
            return this.context.BlobSets.OrderBy(rs => rs.Name);
        }

        public void CreateBlobSet(BlobSet blobSet)
        {
            if (blobSet.BlobSetId == Guid.Empty)
            {
                blobSet.BlobSetId = Guid.NewGuid();
            }

            this.context.BlobSets.AddObject(blobSet);
            this.context.SaveChanges();
        }

        public BlobSet GetBlobSetById(Guid id)
        {
            return this.context.BlobSets.Where(rl => rl.BlobSetId == id).FirstOrDefault();
        }

        public void UpdateBlobSet(BlobSet set)
        {
            var currentBlobSet = this.context.BlobSets.Where(rs => rs.BlobSetId == set.BlobSetId).SingleOrDefault();

            if (currentBlobSet != null)
            {
                currentBlobSet.Name = set.Name;
                currentBlobSet.Description = set.Description;

                this.context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Blob Set does not exist.");
            }
        }

        public IQueryable<BlobSet> GetBlobSetsByPartialName(string name)
        {
            return this.context.BlobSets.Where(rs => rs.Name.Contains(name)).OrderBy(rs => rs.Name);
        }

        public BlobSet GetBlobSetByName(string name)
        {
            return this.context.BlobSets.Where(r => r.Name == name).FirstOrDefault();
        }

        public void DeleteBlobSet(Guid setId)
        {
            var currentBlobSet = this.context.BlobSets.Where(rs => rs.BlobSetId == setId).SingleOrDefault();

            if (currentBlobSet != null)
            {
                var blobs = currentBlobSet.Blobs.ToList();

                foreach (var blob in blobs)
                {
                    currentBlobSet.Blobs.Remove(blob);
                }

                this.context.BlobSets.DeleteObject(currentBlobSet);

                this.context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Blob Set does not exist.");
            }
        }
    }
}