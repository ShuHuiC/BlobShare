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
    public class UserServiceTests
    {
        [TestMethod]
        public void CreateUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                User user = new User() { Name = "Test User", Email = "test@live.com" };
                service.CreateUser(user);
                Assert.AreNotEqual(Guid.Empty, user.UserId);
            }
        }

        [TestMethod]
        public void CreateAndRetrieveUserById()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                User user = new User() { Name = "Test User", Email = "test@live.com" };
                service.CreateUser(user);
                User newUser = service.RetrieveUserById(user.UserId);

                Assert.IsNotNull(newUser);
                Assert.AreEqual(user.UserId, newUser.UserId);
                Assert.AreEqual(user.Name, newUser.Name);
                Assert.AreEqual(user.Email, newUser.Email);
            }
        }

        [TestMethod]
        public void CreateAndRetrieveUserByNameIdentifierAndIdentityProvider()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                User user = new User() { NameIdentifier = "nameIdentifier", IdentityProvider = "identityProvider", Name = "Test Name", Email = "test@email.com" };
                service.CreateUser(user);
                User newUser = service.RetrieveUserByNameIdentifier(user.NameIdentifier, user.IdentityProvider);

                Assert.IsNotNull(newUser);
                Assert.AreEqual(user.UserId, newUser.UserId);
                Assert.AreEqual(user.NameIdentifier, newUser.NameIdentifier);
                Assert.AreEqual(user.IdentityProvider, newUser.IdentityProvider);
            }
        }

        [TestMethod]
        public void SearchUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                service.CreateUser(new User() { Name = "Test User", Email = "test@live.com" });
                service.CreateUser(new User() { Name = "YesTest User", Email = "test2@live.com" });
                service.CreateUser(new User() { Name = "Non User", Email = "test3@live.com" });

                var users = service.SearchUsers("Test");

                Assert.IsNotNull(users);
                Assert.AreEqual(2, users.Count());
            }
        }

        [TestMethod]
        public void UpdateUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                User user = new User() { Name = "Test User", Email = "test@live.com" };

                service.CreateUser(user);
                
                User updatedUser = new User() { UserId = user.UserId, Name = "New Test User", Email = "newtest@live.com" };
                service.UpdateUser(updatedUser);

                User newUser = service.RetrieveUserById(user.UserId);

                Assert.IsNotNull(newUser);
                Assert.AreEqual("New Test User", newUser.Name);
                Assert.AreEqual("newtest@live.com", newUser.Email);
            }
        }

        [TestMethod]
        public void GetUsers()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();

                for (int k = 1; k <= 10; k++)
                {
                    User user = new User()
                    { 
                        Name = string.Format("Test User {0}", k),
                        Email = string.Format("test{0}@live.com", k) 
                    };
                    service.CreateUser(user);
                }

                var users = service.GetUsers();

                Assert.IsNotNull(users);
                Assert.IsTrue(users.Count() >= 10);
            }
        }

        [TestMethod]
        public void DeactivateAndActivateUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                UserService service = new UserService();
                User user = new User() { Name = "Test User", Email = "test@live.com" };
                service.CreateUser(user);
                service.DeactivateUser(user);
                Assert.IsTrue(user.Inactive);
                service.ActivateUser(user);
                Assert.IsFalse(user.Inactive);
                Assert.IsTrue(user.UserEvents.Count == 3);
            }
        }
    }
}
