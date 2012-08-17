namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;

    [TestClass]
    public class BlobSetServiceTests
    {
        private const string TestContainerName = "testcontainer";

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void AddAnExistingResourceToAnExistingList()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSet set = CreateBlobSetForTest("Test List", context);
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);

                BlobSetService service = new BlobSetService(context);
                service.AddBlobToSet(set.Name, blob.BlobId);

                BlobSet newSet = service.GetBlobSetById(set.BlobSetId);

                Assert.IsNotNull(newSet);
                Assert.AreEqual(1, newSet.Blobs.Count);

                BlobService bservices = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
                Blob newResource = bservices.GetBlobById(blob.BlobId);

                Assert.IsNotNull(newResource);
                Assert.AreEqual(1, newResource.BlobSets.Count);

                Assert.AreEqual(newResource.BlobSets.First().BlobSetId, set.BlobSetId);
                Assert.AreEqual(newSet.Blobs.First().BlobId, blob.BlobId);
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void UpdateExistingList()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSet set = CreateBlobSetForTest("Test List", context);
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);

                BlobSetService service = new BlobSetService(context);
                service.AddBlobToSet(set.Name, blob.BlobId);

                set.Name = "updated test list";
                set.Description = "updated test description";
                service.UpdateBlobSet(set);

                BlobSet newSet = service.GetBlobSetById(set.BlobSetId);

                Assert.IsNotNull(newSet);
                Assert.AreEqual(1, newSet.Blobs.Count);
                Assert.AreEqual(set.Name, newSet.Name);
                Assert.AreEqual(set.Description, newSet.Description);
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void RemoveResourceFromList()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSet set = CreateBlobSetForTest("Test List", context);
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);

                BlobSetService service = new BlobSetService(context);
                service.AddBlobToSet(set.BlobSetId, blob.Name);
                service.RemoveBlobFromSet(set.BlobSetId, blob.BlobId);

                BlobSet newList = service.GetBlobSetById(set.BlobSetId);

                Assert.IsNotNull(newList);
                Assert.AreEqual(0, newList.Blobs.Count);

                BlobService bservices = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
                Blob newResource = bservices.GetBlobById(blob.BlobId);

                Assert.IsNotNull(newResource);
                Assert.AreEqual(0, newResource.BlobSets.Count);
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void RemoveSet()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSet set = CreateBlobSetForTest("Test List", context);
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);

                BlobSetService service = new BlobSetService(context);
                service.AddBlobToSet(set.BlobSetId, blob.BlobId);
                BlobSet newList = service.GetBlobSetById(set.BlobSetId);

                Assert.IsNotNull(newList);
                Assert.AreEqual(1, newList.Blobs.Count);

                service.DeleteBlobSet(set.BlobSetId);

                BlobSet deletedList = service.GetBlobSetById(set.BlobSetId);

                Assert.IsNull(deletedList);

                BlobService bservices = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
                Blob newResource = bservices.GetBlobById(blob.BlobId);

                Assert.IsNotNull(newResource);
                Assert.AreEqual(0, newResource.BlobSets.Count);
            }
        }

        private static BlobSet CreateBlobSetForTest(string name)
        {
            return CreateBlobSetForTest(name, new BlobShareDataStoreEntities());
        }

        private static BlobSet CreateBlobSetForTest(string name, BlobShareDataStoreEntities context)
        {
            var service = new BlobSetService(context);
            BlobSet entity = new BlobSet()
            {
                Name = name
            };

            service.CreateBlobSet(entity);

            return entity;
        }
    }
}