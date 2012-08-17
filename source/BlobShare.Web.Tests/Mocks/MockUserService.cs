namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class MockUserService : IUserService
    {
        public MockUserService()
        {
            this.Users = new List<User>();

            for (int k = 1; k <= 10; k++)
            {
                this.Users.Add(
                    new User
                    {
                        UserId = Guid.NewGuid(),
                        Name = string.Format("User {0}", k),
                        Roles = new EntityCollection<Role>()
                    });
            }
        }

        public IList<User> Users { get; set; }

        public User NewUser { get; set; }

        public User UpdatedUser { get; set; }

        public User RetrieveUserByNameIdentifier(string nameIdentifier, string identityProvider)
        {
            return this.Users.FirstOrDefault(u => u.NameIdentifier == nameIdentifier && u.IdentityProvider == identityProvider);
        }

        public User RetrieveUserById(Guid id)
        {
            return this.Users.FirstOrDefault(u => u.UserId == id);
        }

        public User RetrieveUserByEMail(string email)
        {
            return this.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<User> GetUsers()
        {
            return this.Users.AsQueryable<User>();
        }

        public void CreateUser(User user)
        {
            this.Users.Add(user);
            this.NewUser = user;
        }

        public void UpdateUser(User user)
        {
            this.UpdatedUser = user;
        }

        public void ActivateUser(User user)
        {
            user.Inactive = false;
        }

        public void DeactivateUser(User user)
        {
            user.Inactive = true;
        }

        public IQueryable<User> SearchUsers(string partialName)
        {
            return this.Users.Where(u => u.Name.Contains(partialName)).AsQueryable();
        }

        public void DeleteUser(User user)
        {
            this.Users.Remove(user);
        }

        public bool IsMe(User user)
        {
            return false;
        }

        public void Dispose()
        {
        }

        public User RetrieveUserByInvitationId(Guid invitationId)
        {
            throw new NotImplementedException();
        }
    }
}
