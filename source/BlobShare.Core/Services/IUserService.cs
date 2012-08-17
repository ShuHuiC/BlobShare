namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IUserService
    {
        User RetrieveUserByNameIdentifier(string nameIdentifier, string identityProvider);

        User RetrieveUserByInvitationId(Guid invitationId);

        User RetrieveUserById(Guid id);

        User RetrieveUserByEMail(string email);

        IQueryable<User> GetUsers();

        IQueryable<User> SearchUsers(string partialName);

        void CreateUser(User user);
        
        void UpdateUser(User user);

        void ActivateUser(User user);

        void DeactivateUser(User user);

        void DeleteUser(User user);
        
        bool IsMe(User user);
    }
}