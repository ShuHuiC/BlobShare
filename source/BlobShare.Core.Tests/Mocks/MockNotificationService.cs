namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Mocks
{
    using System.Collections.Generic;
    using System.Net.Mail;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;

    public class MockNotificationService : INotificationService
    {
        public string[] Addresses { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string MessageTemplate { get; set; }

        public bool SendNotificationInvoked { get; set; }

        public IDictionary<string, string> Parameters { get; set; }

        public bool UseBcc { get; set; }

        public LinkedResource LinkedResource { get; set; }

        public void SendNotification(string[] addresses, string title, string message, LinkedResource linkedResource)
        {
            this.Addresses = addresses;
            this.Title = title;
            this.Message = message;
            this.SendNotificationInvoked = true;
            this.LinkedResource = LinkedResource;
        }

        public void SendNotification(string[] addresses, string title, string messageTemplate, LinkedResource linkedResource, IDictionary<string, string> messageParameters)
        {
            this.Addresses = addresses;
            this.Title = title;
            this.MessageTemplate = messageTemplate;
            this.Parameters = messageParameters;
            this.SendNotificationInvoked = true;
            this.LinkedResource = LinkedResource;
        }

        public void SendNotification(string[] addresses, string title, string message, LinkedResource linkedResource, bool useBcc)
        {
            this.Addresses = addresses;
            this.Title = title;
            this.Message = message;
            this.SendNotificationInvoked = true;
            this.LinkedResource = LinkedResource;
            this.UseBcc = useBcc;
        }
    }
}