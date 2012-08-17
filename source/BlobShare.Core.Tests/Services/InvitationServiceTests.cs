namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InvitationServiceTests
    {
        [TestMethod, Ignore]
        public void InviteUser()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
                InvitationService service = new InvitationService(context);
                User user = this.GetTestUser(context);
                var link = "http://test/";

                var invitation = service.InviteUser(user, link, DateTime.UtcNow.AddDays(5), "Test invitation", false);

                Assert.IsNotNull(invitation);
                Assert.AreEqual(user.Email, invitation.Email);
                Assert.AreEqual(user, invitation.User);
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
    }
}