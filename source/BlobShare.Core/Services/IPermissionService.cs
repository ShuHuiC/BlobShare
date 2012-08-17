namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IPermissionService
    {
        Permission GrantPermissionToUserBlob(Privilege privilege, User user, Blob blob, DateTime expiration);

        Permission GrantPermissionToUserBlobSet(Privilege privilege, User user, BlobSet set, DateTime expiration);

        Permission GrantPermissionToRoleBlob(Privilege privilege, Role role, Blob blob, DateTime expiration);

        Permission GrantPermissionToRoleBlobSet(Privilege privilege, Role role, BlobSet set, DateTime expiration);

        IEnumerable<BlobSet> GetBlobSetsByUser(User user);

        IEnumerable<BlobSet> GetBlobSetsByUser(string nameIdentifier, string identityProvider);

        IEnumerable<BlobSet> GetBlobSetsByRole(Role role);

        IEnumerable<Blob> GetBlobsByUser(User user);

        IEnumerable<Blob> GetBlobsByUser(string nameIdentifier, string identityProvider);

        IEnumerable<Blob> GetBlobsByRole(Role role);

        bool CheckPermissionToBlob(string nameIdentifier, string identityProvider, Guid blobId);

        bool CheckPermissionToBlobSet(string nameIdentifier, string identityProvider, Guid blobSetId);

        void RevokePermission(Guid id);

        IEnumerable<Permission> GetNewPermissionsByUser(User user, int count);
    }
}