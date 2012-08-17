namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    public class SmtpNotificationService : INotificationService
    {
        private readonly string smtpHost;
        private readonly int smtpPort;
        private readonly string smtpUser;
        private readonly string smtpPassword;
        private readonly string senderName;
        private readonly string senderAddress;

        public SmtpNotificationService(string smtpHost, int smtpPort, string smtpUser, string smtpPassword, string senderName, string senderAddress)
        {
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
            this.smtpUser = smtpUser;
            this.smtpPassword = smtpPassword;
            this.senderName = senderName;
            this.senderAddress = senderAddress;
        }

        public void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource, bool useBcc)
        {
            using (AlternateView htmlView = AlternateView.CreateAlternateViewFromString(htmlMessage, Encoding.UTF8, "text/html"))
            {
                if (linkedResource != null)
                {
                    htmlView.LinkedResources.Add(linkedResource);
                }

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.Subject = title;
                    mailMessage.From = new MailAddress(this.senderAddress);

                    if (useBcc)
                    {
                        mailMessage.To.Add(this.senderAddress);
                        foreach (var email in addresses)
                        {
                            mailMessage.Bcc.Add(email);
                        }
                    }
                    else
                    {
                        foreach (var email in addresses)
                        {
                            mailMessage.To.Add(email);
                        }
                    }

                    mailMessage.AlternateViews.Add(htmlView);

                    using (var smtpClient = new SmtpClient(this.smtpHost, this.smtpPort))
                    {
                        smtpClient.EnableSsl = true;
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtpClient.Credentials = new NetworkCredential(this.smtpUser, this.smtpPassword);

                        smtpClient.Send(mailMessage);
                    }
                }
            }
        }

        public void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource)
        {
            this.SendNotification(addresses, title, htmlMessage, linkedResource, false);
        }

        public void SendNotification(string[] addresses, string title, string htmlMessage, LinkedResource linkedResource, IDictionary<string, string> messageParameters)
        {
            foreach (var messageParameter in messageParameters)
            {
                htmlMessage = htmlMessage.Replace(messageParameter.Key, messageParameter.Value);
            }

            this.SendNotification(addresses, title, htmlMessage, linkedResource, false);
        }
    }
}