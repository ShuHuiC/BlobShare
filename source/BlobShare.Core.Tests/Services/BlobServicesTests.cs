namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [TestClass]
    public class BlobServicesTests
    {
        private const string TestContainerName = "testcontainer";

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void UploadBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var service = new BlobService(CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);

                var blob = new Blob()
                {
                    Name = "Test Resource",
                    OriginalFileName = "logo-dpe.png",
                    Description = "Test Resource",
                    UploadDateTime = DateTime.Now
                };

                Stream stream = new FileStream("logo-dpe.png", FileMode.Open, FileAccess.Read);

                service.UploadBlob(blob, stream);

                Assert.IsNotNull(blob.BlobId);
                Assert.AreNotEqual(Guid.Empty, blob.BlobId);

                Blob newBlob = service.GetBlobById(blob.BlobId);

                Assert.IsNotNull(newBlob);
                Assert.AreEqual(blob.BlobId, newBlob.BlobId);
                Assert.AreEqual(blob.Description, newBlob.Description);
                Assert.AreEqual(blob.OriginalFileName, newBlob.OriginalFileName);
                Assert.AreEqual(blob.UploadDateTime.ToString(), newBlob.UploadDateTime.ToString());

                var resources = service.GetBlobs();

                Assert.IsNotNull(resources);
                Assert.IsTrue(resources.Count() >= 1);
                Assert.IsNotNull(resources.Where(r => r.BlobId == newBlob.BlobId).FirstOrDefault());

                CloudBlob cloudBlob = service.GetBlob(newBlob);
                cloudBlob.Delete();
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void UpdateDescription()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                Blob blob = CreateBlobForTest("Resource For Test", context);
                BlobService service = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);

                service.UpdateDescription(blob.BlobId, "New Name", "New Description");

                Blob newResource = service.GetBlobById(blob.BlobId);

                Assert.IsNotNull(newResource);
                Assert.AreEqual(0, newResource.BlobSets.Count);
                Assert.AreEqual(blob.BlobId, newResource.BlobId);
                Assert.AreEqual("New Name", newResource.Name);
                Assert.AreEqual("New Description", newResource.Description);
                Assert.AreEqual(blob.OriginalFileName, newResource.OriginalFileName);
                Assert.AreEqual(blob.UploadDateTime.ToString(), newResource.UploadDateTime.ToString());

                CloudBlob cloudBlob = service.GetBlob(newResource);
                cloudBlob.Delete();
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void DeleteBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                Blob blob = CreateBlobForTest("Test Resource", context);
                BlobService service = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);

                service.DeleteBlobById(blob.BlobId);

                Assert.IsNull(service.GetBlobById(blob.BlobId));
            }
        }

        [TestMethod]
        public void GetContentTypes()
        {
            BlobService service = new BlobService(CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
            Blob blob = new Blob();
            blob.OriginalFileName = "image.jpg";
            Assert.AreEqual("image/jpeg", service.GetContentType(blob));
            blob.OriginalFileName = "image.gif";
            Assert.AreEqual("image/gif", service.GetContentType(blob));
            blob.OriginalFileName = "video.wmv";
            Assert.AreEqual("video/x-ms-wmv", service.GetContentType(blob));
            blob.OriginalFileName = "video.avi";
            Assert.AreEqual("video/avi", service.GetContentType(blob));
            blob.OriginalFileName = "sound.wav";
            Assert.AreEqual("audio/wav", service.GetContentType(blob));

            ////These two Asserts will fail if they are executed on a PC without office
            ////blob.OriginalFileName = "doc.docx";
            ////Assert.AreEqual("application/vnd.openxmlformats-officedocument.wordprocessingml.document", service.GetContentType(blob));
            ////blob.OriginalFileName = "plan.xlsx";
            ////Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", service.GetContentType(blob));
        }

        internal static Blob CreateBlobForTest(string name)
        {
            return CreateBlobForTest(name, new BlobShareDataStoreEntities());
        }

        internal static Blob CreateBlobForTest(string name, BlobShareDataStoreEntities context)
        {
            BlobService service = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
            Blob blob = new Blob()
            {
                Name = "Test Resource",
                OriginalFileName = "logo-dpe.png",
                Description = "Test Resource",
                UploadDateTime = DateTime.Now
            };

            Stream stream = new FileStream("logo-dpe.png", FileMode.Open, FileAccess.Read);

            service.UploadBlob(blob, stream);

            stream.Close();

            return blob;
        }
    }
}