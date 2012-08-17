namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System.Collections.Generic;
    using System.Net.Mail;

    public interface INotificationService
    {
        void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource);

        void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource, IDictionary<string, string> messageParameters);

        void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource, bool useBcc);
    }
}