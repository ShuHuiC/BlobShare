namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    [TestClass]
    public class PermissionServiceTests
    {
        private const string TestContainerName = "testcontainer";

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void AddUserToBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);
                User user = CreateUserForTest("testuser1", context);
                PermissionService service = new PermissionService(context);
                Permission permission = service.GrantPermissionToUserBlob(Privilege.Read, user, blob, DateTime.UtcNow.AddDays(1));

                Assert.IsNotNull(permission);
                Assert.AreEqual(user, permission.Users.First());
                Assert.AreEqual(blob, permission.Blob);
                Assert.AreEqual((int)Privilege.Read, permission.Privilege);

                IEnumerable<Blob> resources = service.GetBlobsByUser(user);

                Assert.IsNotNull(resources);
                Assert.AreEqual(1, resources.Count());
                Assert.AreEqual(blob.BlobId, resources.First().BlobId);

                Assert.IsTrue(service.CheckPermissionToBlob(user.NameIdentifier, user.IdentityProvider, blob.BlobId));

                BlobService blobService = new BlobService(context, CloudStorageAccount.DevelopmentStorageAccount, TestContainerName);
                CloudBlob cloudBlob = blobService.GetBlob(blob);
                cloudBlob.Delete();
            }
        }

        [TestMethod]
        public void AddUserToBlobSet()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSetService setService = new BlobSetService(context);
                BlobSet set = new BlobSet()
                {
                    Name = "Resource Set Test",
                    Description = "Resource Set Test"
                };
                setService.CreateBlobSet(set);

                User user = CreateUserForTest("testuser1", context);
                PermissionService service = new PermissionService(context);
                Permission permission = service.GrantPermissionToUserBlobSet(Privilege.Read, user, set, DateTime.UtcNow.AddDays(1));

                Assert.IsNotNull(permission);
                Assert.AreEqual(user, permission.Users.First());
                Assert.AreEqual(set, permission.BlobSet);
                Assert.AreEqual((int)Privilege.Read, permission.Privilege);

                IEnumerable<BlobSet> sets = service.GetBlobSetsByUser(user);

                Assert.IsNotNull(sets);
                Assert.AreEqual(1, sets.Count());
                Assert.AreEqual(set.BlobSetId, sets.First().BlobSetId);

                Assert.IsTrue(service.CheckPermissionToBlobSet(user.NameIdentifier, user.IdentityProvider, set.BlobSetId));
            }
        }

        [TestMethod]
        [DeploymentItem("Files\\logo-dpe.png")]
        public void AddRoleToBlob()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                Blob blob = BlobServicesTests.CreateBlobForTest("Test Resource", context);

                Role role = CreateRoleForTest("testrole1", context);
                PermissionService service = new PermissionService(context);
                Permission permission = service.GrantPermissionToRoleBlob(Privilege.Read, role, blob, DateTime.UtcNow.AddDays(1));

                Assert.IsNotNull(permission);
                Assert.AreEqual(role, permission.Roles.First());
                Assert.AreEqual(blob, permission.Blob);
                Assert.AreEqual((int)Privilege.Read, permission.Privilege);

                IEnumerable<Blob> resources = service.GetBlobsByRole(role);

                Assert.IsNotNull(resources);
                Assert.AreEqual(1, resources.Count());
                Assert.AreEqual(blob.BlobId, resources.First().BlobId);
            }
        }

        [TestMethod]
        public void AddRoleToBlobSet()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                BlobSetService setService = new BlobSetService(context);
                BlobSet set = new BlobSet()
                {
                    Name = "Resource Set Test",
                    Description = "Resource Set Test"
                };
                setService.CreateBlobSet(set);

                Role role = CreateRoleForTest("testrole1", context);
                PermissionService service = new PermissionService(context);
                Permission permission = service.GrantPermissionToRoleBlobSet(Privilege.Read, role, set, DateTime.UtcNow.AddDays(1));

                Assert.IsNotNull(permission);
                Assert.AreEqual(role, permission.Roles.First());
                Assert.AreEqual(set, permission.BlobSet);
                Assert.AreEqual((int)Privilege.Read, permission.Privilege);

                IEnumerable<BlobSet> sets = service.GetBlobSetsByRole(role);

                Assert.IsNotNull(sets);
                Assert.AreEqual(1, sets.Count());
                Assert.AreEqual(set.BlobSetId, sets.First().BlobSetId);
            }
        }

        internal static User CreateUserForTest(string name, BlobShareDataStoreEntities context)
        {
            User user = new User()
            {
                UserId = Guid.NewGuid(),
                Name = name,
                Email = name,
                NameIdentifier = name,
                IdentityProvider = name,
            };

            context.Users.AddObject(user);
            context.SaveChanges();

            return user;
        }

        internal static Role CreateRoleForTest(string name, BlobShareDataStoreEntities context)
        {
            Role role = new Role()
            {
                RoleName = name,
                Description = name
            };

            context.Roles.AddObject(role);
            context.SaveChanges();

            return role;
        }
    }
}
