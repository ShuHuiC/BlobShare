namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class PermissionService : IPermissionService
    {
        private BlobShareDataStoreEntities context;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public PermissionService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        public PermissionService(BlobShareDataStoreEntities context)
        {
            this.context = context;
        }

        public Permission GrantPermissionToUserBlob(Privilege privilege, User user, Blob blob, DateTime expiration)
        {
            var permission = new Permission()
            {
                PermissionId = Guid.NewGuid(),
                Privilege = (int)privilege,
                Blob = this.context.Blobs.Single(br => br.BlobId == blob.BlobId),
                CreationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expiration
            };

            permission.Users.Add(this.context.Users.Single(u => u.UserId == user.UserId));
            this.context.Permissions.AddObject(permission);
            this.context.SaveChanges();

            return permission;
        }

        public Permission GrantPermissionToUserBlobSet(Privilege privilege, User user, BlobSet set, DateTime expiration)
        {
            var permission = new Permission()
            {
                PermissionId = Guid.NewGuid(),
                Privilege = (int)privilege,
                BlobSet = this.context.BlobSets.Single(rs => rs.BlobSetId == set.BlobSetId),
                CreationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expiration
            };

            permission.Users.Add(this.context.Users.Single(u => u.UserId == user.UserId));
            this.context.Permissions.AddObject(permission);
            this.context.SaveChanges();

            return permission;
        }

        public Permission GrantPermissionToRoleBlob(Privilege privilege, Role role, Blob blob, DateTime expiration)
        {
            var permission = new Permission()
            {
                PermissionId = Guid.NewGuid(),
                Privilege = (int)privilege,
                Blob = blob,
                CreationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expiration
            };

            permission.Roles.Add(this.context.Roles.Single(r => r.RoleId == role.RoleId));
            this.context.Permissions.AddObject(permission);
            this.context.SaveChanges();

            return permission;
        }

        public Permission GrantPermissionToRoleBlobSet(Privilege privilege, Role role, BlobSet set, DateTime expiration)
        {
            var permission = new Permission()
            {
                PermissionId = Guid.NewGuid(),
                Privilege = (int)privilege,
                BlobSet = set,
                CreationDateTime = DateTime.UtcNow,
                ExpirationDateTime = expiration
            };

            permission.Roles.Add(this.context.Roles.Single(r => r.RoleId == role.RoleId));
            this.context.Permissions.AddObject(permission);
            this.context.SaveChanges();

            return permission;
        }

        public IEnumerable<BlobSet> GetBlobSetsByUser(string nameIdentifier, string identityProvider)
        {
            return this.GetBlobSetsByUser(this.RetrieveUserByNameIdentifier(nameIdentifier, identityProvider));
        }

        public IEnumerable<BlobSet> GetBlobSetsByUser(User user)
        {
            IEnumerable<BlobSet> sets = this.context.Permissions.Where(p => p.Users.Any(u => u.UserId == user.UserId) && p.ExpirationDateTime > DateTime.UtcNow && p.BlobSet != null).Select(p => p.BlobSet).Distinct();

            var rolesets = user.Roles.SelectMany(r => r.Permissions).Where(p => p.BlobSet != null && p.ExpirationDateTime > DateTime.UtcNow).Select(p => p.BlobSet).Distinct();

            return sets.Union(rolesets).Distinct().OrderBy(rs => rs.Name);
        }

        public IEnumerable<BlobSet> GetBlobSetsByRole(Role role)
        {
            return role.Permissions.Where(p => p.BlobSet != null && p.ExpirationDateTime > DateTime.UtcNow).Select(p => p.BlobSet).Distinct().OrderBy(rs => rs.Name);
        }

        public IEnumerable<Blob> GetBlobsByUser(string nameIdentifier, string identityProvider)
        {
            return this.GetBlobsByUser(this.RetrieveUserByNameIdentifier(nameIdentifier, identityProvider));
        }

        public IEnumerable<Blob> GetBlobsByUser(User user)
        {
            IEnumerable<Blob> blobs = this.context.Permissions.Where(p => p.Users.Any(u => u.UserId == user.UserId) && p.ExpirationDateTime > DateTime.UtcNow && p.Blob != null).Select(p => p.Blob).Distinct();

            var roleblobs = user.Roles.SelectMany(r => r.Permissions).Where(p => p.Blob != null && p.ExpirationDateTime > DateTime.UtcNow).Select(p => p.Blob).Distinct();

            return blobs.Union(roleblobs).Distinct().OrderBy(br => br.Name);
        }

        public IEnumerable<Blob> GetBlobsByRole(Role role)
        {
            return role.Permissions.Where(p => p.Blob != null && p.ExpirationDateTime > DateTime.UtcNow).Select(p => p.Blob).Distinct().OrderBy(br => br.Name);
        }

        public IEnumerable<Permission> GetNewPermissionsByUser(User user, int count)
        {
            return this.context.Permissions.Where(p => p.ExpirationDateTime > DateTime.UtcNow && (p.Users.Any(u => u.UserId == user.UserId) || p.Roles.Any(r => r.Users.Any(u => u.UserId == user.UserId)))).OrderByDescending(p => p.CreationDateTime).Take(count);
        }

        public bool CheckPermissionToBlob(string nameIdentifier, string identityProvider, Guid blobId)
        {
            var blob = this.context.Blobs.SingleOrDefault(br => br.BlobId == blobId);
            var validAccess = false;

            if (blob != null)
            {
                var blobSets = blob.BlobSets.Select(rs => rs.BlobSetId);

                validAccess = this.context.Users.Any(
                    u =>
                        u.NameIdentifier.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase) &&
                        u.IdentityProvider.Equals(identityProvider, StringComparison.OrdinalIgnoreCase) &&
                        (u.Permissions.Any(
                            up =>
                                up.ExpirationDateTime > DateTime.UtcNow && 
                                (blobSets.Any(rs => rs == up.BlobSet.BlobSetId) || up.Blob.BlobId == blobId)) ||
                         u.Roles.Any(
                            r => r.Permissions.Any(
                                rp =>
                                    rp.ExpirationDateTime > DateTime.UtcNow && 
                                    (blobSets.Any(rs => rs == rp.BlobSet.BlobSetId) || rp.Blob.BlobId == blobId)))));
            }

            return validAccess;
        }

        public bool CheckPermissionToBlobSet(string nameIdentifier, string identityProvider, Guid blobSetId)
        {
            return this.context.Users.Any(
                u =>
                    u.NameIdentifier.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase) &&
                    u.IdentityProvider.Equals(identityProvider, StringComparison.OrdinalIgnoreCase) &&
                    (u.Permissions.Any(
                        up => 
                            up.ExpirationDateTime > DateTime.UtcNow &&
                            up.BlobSet.BlobSetId == blobSetId) ||
                     u.Roles.Any(
                        r => r.Permissions.Any(
                                rp =>
                                    rp.ExpirationDateTime > DateTime.UtcNow && 
                                    rp.BlobSet.BlobSetId == blobSetId))));
        }

        public void RevokePermission(Guid id)
        {
            Permission permission = this.context.Permissions.FirstOrDefault(p => p.PermissionId == id);

            if (permission != null)
            {
                var user = permission.Users.FirstOrDefault();
                var role = permission.Roles.FirstOrDefault();

                if (user != null)
                {
                    permission.Users.Remove(user);
                }

                if (role != null)
                {
                    permission.Roles.Remove(role);
                }

                this.context.Permissions.DeleteObject(permission);
                this.context.SaveChanges();
            }
        }

        private User RetrieveUserByNameIdentifier(string nameIdentifier, string identityProvider)
        {
            return this.context.Users.Where(u => u.NameIdentifier.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase) &&
                    u.IdentityProvider.Equals(identityProvider, StringComparison.OrdinalIgnoreCase))
                                     .SingleOrDefault();
        }
    }
}