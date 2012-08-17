namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class RoleService : IRoleService
    {
        private readonly BlobShareDataStoreEntities context;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public RoleService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        public RoleService(BlobShareDataStoreEntities context)
        {
            this.context = context;
        }

        public Role GetRoleByName(string name)
        {
            return this.context.Roles.Where(r => r.RoleName.Equals(name, StringComparison.OrdinalIgnoreCase))
                                     .SingleOrDefault();
        }

        public Role GetRoleById(Guid id)
        {
            return this.context.Roles.Where(r => r.RoleId == id).SingleOrDefault();
        }

        public void CreateRole(Role role)
        {
            var currentRole = this.context.Roles.Where(r => r.RoleName.Equals(role.RoleName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (currentRole != null)
            {
                throw new InvalidOperationException("Role already exists.");
            }

            if (role.RoleId == Guid.Empty)
            {
                role.RoleId = Guid.NewGuid();
            }

            this.context.Roles.AddObject(role);
            this.context.SaveChanges();
        }

        public void UpdateRole(Role role)
        {
            var findRole = this.context.Roles.Where(r => r.RoleName.Equals(role.RoleName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (findRole != null)
            {
                throw new InvalidOperationException("Role already exists.");
            }

            var currentRole = this.context.Roles.Where(r => r.RoleId == role.RoleId).SingleOrDefault();
            if (currentRole != null)
            {
                currentRole.RoleName = role.RoleName;

                this.context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("Role does not exist.");
            }
        }

        public IQueryable<Role> GetRoles()
        {
            return this.context.Roles.OrderBy(r => r.RoleName);
        }

        public void RemoveUserFromRole(Role role, User user)
        {
            role.Users.Remove(user);
            this.context.SaveChanges();
        }

        public void AddUserToRole(Role role, User user)
        {
            role.Users.Add(user);
            this.context.SaveChanges();
        }
    }
}