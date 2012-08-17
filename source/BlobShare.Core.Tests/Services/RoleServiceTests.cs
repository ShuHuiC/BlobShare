namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System;
    using System.Linq;
    using System.Transactions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleServiceTests
    {
        [TestMethod]
        public void CreateRole()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();
                Role role = new Role() { RoleName = "Test Role" };
                service.CreateRole(role);
                Assert.AreNotEqual(Guid.Empty, role.RoleId);
            }
        }

        [TestMethod]
        public void CreateAndRetrieveRole()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();
                Role role = new Role() { RoleName = "Test Role" };
                service.CreateRole(role);
                Role newRole = service.GetRoleById(role.RoleId);
                Assert.IsNotNull(newRole);
                Assert.AreEqual(role.RoleName, newRole.RoleName);
                Assert.AreEqual(role.RoleId, newRole.RoleId);
            }
        }

        [TestMethod]
        public void GetRoleByName()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();
                Role role = new Role() { RoleName = "Test Role" };
                service.CreateRole(role);
                Role newRole = service.GetRoleByName(role.RoleName);
                Assert.IsNotNull(newRole);
                Assert.AreEqual(role.RoleName, newRole.RoleName);
                Assert.AreEqual(role.RoleId, newRole.RoleId);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Role already exists")]
        public void RaiseIfDuplicateRole()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();
                Role role = new Role() { RoleName = "Test Role" };
                service.CreateRole(role);
                Role newRole = new Role() { RoleName = "Test Role" };
                service.CreateRole(newRole);
            }
        }

        [TestMethod]
        public void UpdateRole()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();
                Role role = new Role() { RoleName = "Test Role" };
                service.CreateRole(role);
                Role updatedRole = new Role() { RoleId = role.RoleId, RoleName = "New Test Role" };
                service.UpdateRole(updatedRole);
                Role newRole = service.GetRoleById(role.RoleId);
                Assert.IsNotNull(newRole);
                Assert.AreEqual("New Test Role", newRole.RoleName);
            }
        }

        [TestMethod]
        public void GetRoles()
        {
            using (TransactionScope ts = new TransactionScope())
            {
                RoleService service = new RoleService();

                for (int k = 1; k <= 10; k++)
                {
                    Role role = new Role() { RoleName = "Test Role " + k };
                    service.CreateRole(role);
                }

                IQueryable<Role> roles = service.GetRoles();

                Assert.IsNotNull(roles);
                Assert.IsTrue(roles.Count() >= 10);
            }
        }
    }
}
