namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;

    public interface IChangesNotificationService
    {
        void NotifyBlobsetUsers(Guid blobsetId, string invitationLink, string title, string personalMessage, bool signEmail);
    }
}
