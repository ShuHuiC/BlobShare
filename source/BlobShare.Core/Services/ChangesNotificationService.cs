namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Web;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class ChangesNotificationService : IChangesNotificationService 
    {
        private BlobShareDataStoreEntities context;
        private SmtpNotificationService smtpNotificationService;
        private IBlobSetService blobSetService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Disepose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public ChangesNotificationService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Disepose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public ChangesNotificationService(BlobShareDataStoreEntities context)
            : this(
                context,
                new BlobSetService(context),
                new SmtpNotificationService(
                    ConfigReader.GetConfigValue("SmtpHost"),
                    int.Parse(ConfigReader.GetConfigValue("SmtpPort"), CultureInfo.InvariantCulture),
                    ConfigReader.GetConfigValue("SmtpUser"),
                    ConfigReader.GetConfigValue("SmtpPassword"),
                    ConfigReader.GetConfigValue("SmtpSenderName"),
                    ConfigReader.GetConfigValue("SmtpSenderAddress")))
        {
        }

        public ChangesNotificationService(BlobShareDataStoreEntities context, BlobSetService blobSetService, SmtpNotificationService smtpNotificationService)
        {
            this.context = context;
            this.blobSetService = blobSetService;
            this.smtpNotificationService = smtpNotificationService;
        }
                
        public void NotifyBlobsetUsers(Guid blobsetId, string blobsetLink, string title, string personalMessage, bool signEmail)
        {
            string[] emails = this.GetRecipients(blobsetId);

            if (personalMessage == null)
            {
                personalMessage = string.Empty;
            }

            personalMessage = personalMessage.Replace("\n", "\n<br/>");

            var notificationSubject = title;

            string signature = string.Empty;

            if (signEmail)
            {
                signature = string.Format("Thanks, {0}", GetCurrentUserIdentityName());
            }

            var notificationTemplate = new StringBuilder()
            .Append("<html>")
            .Append("<body link=\"#ff6600\" vlink=\"#ff6600\">")
            .Append("<div>")
            .Append("<table border=\"0\" cellspacing=\"0\" cellpadding=\"0\" style=\"border-collapse:collapse;border:none\">")
            .Append("<tr>")
            .Append("<td valign=\"top\" style=\"height:18pt; width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("<td colspan=\"2\" valign=\"top\" style=\"width:424.0pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("<td valign=\"top\" style=\"width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("</tr>")
            .Append("<tr>")
            .Append("<td valign=\"top\" style=\"width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("<td valign=\"top\" style=\"width:70.85pt;padding:5.4pt 0cm 0cm 0cm\"><img src=\"cid:logo\"/></td>")
            .Append("<td valign=\"top\" style=\"width:353.15pt;padding:0cm 5.4pt 0cm 5.4pt\">")
            .Append("<p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'><br>Hi! This is a notification message from Blob Share. </span></p>")
            .Append("<p></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>Blob Share is a Windows Azure based application that allows you to upload, view, download and share your blobs on the cloud.</span></p><p></p>")
            .Append(string.Format("<p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>In order to access the changed resources, please visit the following URL:&nbsp;<a style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#ff6600' href=\"{0}\">{0}</a></span></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>{1}</span></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>{2}</span></p><br>", blobsetLink, personalMessage, signature))
            .Append("<td valign=\"top\" style=\"width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></p></td>")
            .Append("</tr>")
            .Append("<tr>")
            .Append("<td valign=\"top\" style=\"height:18pt; width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("<td colspan=\"2\" valign=\"top\" style=\"width:424.0pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("<td valign=\"top\" style=\"width:13.8pt;background:#e6e6e6;padding:0cm 5.4pt 0cm 5.4pt\"></td>")
            .Append("</tr>")
            .Append("</table>")
            .Append("</div>")
            .Append("</body>")
            .Append("</html>");

            using (LinkedResource logo = new LinkedResource(HttpContext.Current.Server.MapPath("~/Content/Images/home_bg_mail.jpg"), MediaTypeNames.Image.Jpeg))
            {
                logo.ContentId = "logo";
                logo.TransferEncoding = TransferEncoding.Base64;

                this.smtpNotificationService.SendNotification(emails, notificationSubject, notificationTemplate.ToString(), logo, true);
            }
        }

        private static string GetCurrentUserIdentityName()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }

        private string[] GetRecipients(Guid id)
        {
            var blobset = this.blobSetService.GetBlobSetById(id);

            var emailList = new ArrayList();
            foreach (var permission in blobset.Permissions.Where(p => p.ExpirationDateTime > DateTime.UtcNow && p.Roles.Any()))
            {
                foreach (var role in permission.Roles)
                {
                    foreach (var user in role.Users)
                    {
                        if (!emailList.Contains(user.Email))
                        {
                            emailList.Add(user.Email);
                        }
                    }
                }
            }

            foreach (var permission in blobset.Permissions.Where(p => p.ExpirationDateTime > DateTime.UtcNow && p.Users.Any()))
            {
                foreach (var user in permission.Users)
                {
                    if (!emailList.Contains(user.Email))
                    {
                        emailList.Add(user.Email);
                    }
                }
            }

            return emailList.ToArray(typeof(string)) as string[];
        }
    }
}
