namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ReportServiceTests
    {
        [TestMethod]
        public void GetBlobEvents()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blob = this.GetTestBlob(context);

                eventService.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);

                var reportService = new ReportService(context);
                int totalPages;

                var events = reportService.GetBlobEvents(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    new List<int>() { (int)EventType.View },
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(1, events.Count());
                var e = events.First();
                Assert.AreEqual(blob.BlobId, e.Blob.BlobId);
                Assert.AreEqual(blob.Name, e.Blob.Name);
                Assert.AreEqual((int)EventType.View, e.EventType);
                Assert.AreEqual(user.UserId, e.User.UserId);
            }
        }

        [TestMethod]
        public void GetBlobEventsByBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blob = this.GetTestBlob(context);

                eventService.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);

                var reportService = new ReportService(context);
                int totalPages;

                var events = reportService.GetBlobEventsByBlob(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    new List<int>() { (int)EventType.View },
                    blob.BlobId,
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(1, events.Count());
                var e = events.First();
                Assert.AreEqual(blob.BlobId, e.Blob.BlobId);
                Assert.AreEqual(blob.Name, e.Blob.Name);
                Assert.AreEqual((int)EventType.View, e.EventType);
                Assert.AreEqual(user.UserId, e.User.UserId);
            }
        }

        [TestMethod]
        public void GetBlobSetEventsByBlobSet()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blobSet = this.GetTestBlobSet(context);

                eventService.CreateEventUserViewBlobSet(user.NameIdentifier, user.IdentityProvider, blobSet);

                var reportService = new ReportService(context);
                int totalPages;

                var events = reportService.GetBlobSetEventsByBlobSet(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    EventType.View,
                    blobSet.BlobSetId,
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(1, events.Count());
                var e = events.First();
                Assert.AreEqual(blobSet.BlobSetId, e.BlobSet.BlobSetId);
                Assert.AreEqual(blobSet.Name, e.BlobSet.Name);
                Assert.AreEqual((int)EventType.View, e.EventType);
                Assert.AreEqual(user.UserId, e.User.UserId);
            }
        }

        [TestMethod]
        public void GetBlobSetEvents()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blobSet = this.GetTestBlobSet(context);

                eventService.CreateEventUserViewBlobSet(user.NameIdentifier, user.IdentityProvider, blobSet);

                var reportService = new ReportService(context);
                int totalPages;

                var events = reportService.GetBlobSetEvents(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    EventType.View,
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(1, events.Count());
                var e = events.First();
                Assert.AreEqual(blobSet.BlobSetId, e.BlobSet.BlobSetId);
                Assert.AreEqual(blobSet.Name, e.BlobSet.Name);
                Assert.AreEqual((int)EventType.View, e.EventType);
                Assert.AreEqual(user.UserId, e.User.UserId);
            }
        }

        [TestMethod]
        public void GetTopItemsByUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blob = this.GetTestBlob(context);

                eventService.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);

                var reportService = new ReportService(context);
                var events = reportService.GetTopItemsByUser(user, 3);

                Assert.AreEqual(1, events.Count());
                var e = events.First();
                Assert.AreEqual(1, e.Total);
                Assert.IsFalse(e.IsBlobSet);
                Assert.AreEqual(blob.Name, e.Name);
            }
        }

        [TestMethod]
        public void GetUserEvents()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blob = this.GetTestBlob(context);

                eventService.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);

                var reportService = new ReportService(context);
                int totalPages;
                var events = reportService.GetUserEvents(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    new List<int> { (int)EventType.View }, // userEventTypes
                    new List<int> { (int)EventType.View }, // setEventTypes
                    new List<int> { (int)EventType.View }, // resourceEventTypes
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(2, events.Count());
                var ev = events.First(e => !e.IsUserEvent);
                Assert.AreEqual(blob.BlobId, ev.Blob.BlobId);
                Assert.AreEqual(blob.Name, ev.Blob.Name);
                Assert.AreEqual((int)EventType.View, ev.EventType);
                Assert.AreEqual(user.UserId, ev.User.UserId);
            }
        }

        [TestMethod]
        public void GetUserEventsByUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var context = BlobShareDataStoreEntities.CreateInstance();
                var eventService = new EventService(context);
                var user = this.GetTestUser(context);
                var blob = this.GetTestBlob(context);

                eventService.CreateEventUserViewBlob(user.NameIdentifier, user.IdentityProvider, blob);

                var reportService = new ReportService(context);
                int totalPages;
                var events = reportService.GetUserEventsByUser(
                    DateTime.UtcNow.AddSeconds(-5),
                    DateTime.UtcNow.AddSeconds(1),
                    new List<int> { (int)EventType.View }, // userEventTypes
                    new List<int> { (int)EventType.View }, // setEventTypes
                    new List<int> { (int)EventType.View }, // resourceEventTypes
                    user.UserId,
                    100, 
                    1, 
                    out totalPages);

                Assert.AreEqual(1, totalPages);
                Assert.AreEqual(2, events.Count());
                var ev = events.First(e => !e.IsUserEvent);
                Assert.AreEqual(blob.BlobId, ev.Blob.BlobId);
                Assert.AreEqual(blob.Name, ev.Blob.Name);
                Assert.AreEqual((int)EventType.View, ev.EventType);
                Assert.AreEqual(user.UserId, ev.User.UserId);
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
    }
}
