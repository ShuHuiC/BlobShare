namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class MockRoleService : IRoleService
    {
        public MockRoleService()
        {
            this.Roles = new List<Role>();

            for (int k = 1; k <= 10; k++)
            {
                this.Roles.Add(new Role() { RoleId = Guid.NewGuid(), RoleName = string.Format("Role {0}", k) });
            }
        }

        public IList<Role> Roles { get; set; }

        public Role NewRole { get; set; }

        public Role UpdatedRole { get; set; }

        public void CreateRole(Role role)
        {
            role.RoleId = Guid.NewGuid();
            this.Roles.Add(role);
            this.NewRole = role;
        }

        public void UpdateRole(Role role)
        {
            this.UpdatedRole = role;
        }

        public Role GetRoleById(Guid id)
        {
            return this.Roles.Where(r => r.RoleId == id).FirstOrDefault();
        }

        public IQueryable<Role> GetRoles()
        {
            return this.Roles.AsQueryable();
        }

        public Role GetRoleByName(string name)
        {
            throw new NotImplementedException();
        }

        public void AddUserToRole(Role role, User user)
        {
            user.Roles.Add(role);
        }

        public void RemoveUserFromRole(Role role, User user)
        {
            user.Roles.Remove(role);
        }

        public void Dispose()
        {
        }
    }
}
