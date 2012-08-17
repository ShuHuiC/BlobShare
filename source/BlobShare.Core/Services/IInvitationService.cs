namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IInvitationService
    {
        Invitation InviteUser(User user, string invitationLink, DateTime expiration, string personalMessage, bool signEmail);

        void ActivateUserInvitation(Invitation invitation, User user);

        Invitation RetrieveInvitation(Guid invitationId);

        IEnumerable<Invitation> GetInvitations();

        IEnumerable<Invitation> GetPendingInvitations();

        int GetPendingInvitationsCount();
    }
}