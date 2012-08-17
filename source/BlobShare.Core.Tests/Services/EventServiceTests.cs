namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventServiceTests
    {
        [TestMethod]
        public void CreateEventUserActivation()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                service.CreateEventUserActivation(user);
                Assert.AreEqual(2, user.UserEvents.Count);
                Assert.AreEqual((int)UserEventType.Activation, user.UserEvents.Skip(1).First().EventType);
            }
        }

        [TestMethod]
        public void CreateEventUserDeactivation()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                service.CreateEventUserDeactivation(user);
                Assert.AreEqual(2, user.UserEvents.Count);
                Assert.AreEqual((int)UserEventType.Deactivation, user.UserEvents.Skip(1).First().EventType);
            }
        }

        [TestMethod]
        public void CreateEventUserLogin()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                service.CreateEventUserLogin(user);
                Assert.AreEqual(2, user.UserEvents.Count);
                Assert.AreEqual((int)UserEventType.Login, user.UserEvents.Skip(1).First().EventType);
            }
        }

        [TestMethod]
        public void CreateEventUserViewBlobSet()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                BlobSet set = this.GetTestBlobSet(context);
                service.CreateEventUserViewBlobSet(user.NameIdentifier, user.IdentityProvider, set);
                Assert.AreEqual(1, user.BlobSetEvents.Count);
                Assert.AreEqual((int)EventType.View, user.BlobSetEvents.First().EventType);
            }
        }

        [TestMethod]
        public void CreateEventUserViewBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                Blob blob = this.GetTestBlob(context);
                service.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);
                Assert.AreEqual(1, user.BlobEvents.Count);
                Assert.AreEqual((int)EventType.View, user.BlobEvents.First().EventType);
            }
        }

        [TestMethod]
        public void CreateEventUserDownloadBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                EventService service = new EventService(context);
                User user = this.GetTestUser(context);
                Blob blob = this.GetTestBlob(context);
                service.CreateEventUserDownloadBlob(user.NameIdentifier, user.IdentityProvider, blob);
                Assert.AreEqual(1, user.BlobEvents.Count);
                Assert.AreEqual((int)EventType.Download, user.BlobEvents.First().EventType);
            }
        }

        private User GetTestUser(BlobShareDataStoreEntities context)
        {
            UserService userService = new UserService(context);
            User user = new User()
            {
                Name = "Test User",
                Email = "test@live.com",
                IdentityProvider = "identityProvider",
                NameIdentifier = "nameIdentifier"
            };
            userService.CreateUser(user);
            return user;
        }

        private BlobSet GetTestBlobSet(BlobShareDataStoreEntities context)
        {
            BlobSet set = new BlobSet()
            {
                BlobSetId = Guid.NewGuid(),
                Name = "Test Resource Set"
            };

            context.BlobSets.AddObject(set);

            return set;
        }

        private Blob GetTestBlob(BlobShareDataStoreEntities context)
        {
            Blob blob = new Blob()
            {
                BlobId = Guid.NewGuid(),
                Name = "Test Blob Resource",
                OriginalFileName = "originalfile.txt",
                UploadDateTime = DateTime.UtcNow
            };

            context.Blobs.AddObject(blob);

            return blob;
        }
    }
}
