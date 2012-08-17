namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IRoleService
    {
        void CreateRole(Role role);
        
        void UpdateRole(Role role);

        Role GetRoleById(Guid id);

        IQueryable<Role> GetRoles();

        Role GetRoleByName(string name);

        void AddUserToRole(Role role, User user);

        void RemoveUserFromRole(Role role, User user);
    }
}