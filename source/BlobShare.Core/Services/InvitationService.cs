namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net.Mail;
    using System.Net.Mime;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class InvitationService : IInvitationService
    {
        private readonly BlobShareDataStoreEntities context;
        private readonly IPermissionService permissionService;
        private readonly INotificationService notificationService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Disepose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public InvitationService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Disepose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public InvitationService(BlobShareDataStoreEntities context)
            : this(
                context,
                new PermissionService(context),
                new SmtpNotificationService(
                    ConfigReader.GetConfigValue("SmtpHost"),
                    int.Parse(ConfigReader.GetConfigValue("SmtpPort"), CultureInfo.InvariantCulture),
                    ConfigReader.GetConfigValue("SmtpUser"),
                    ConfigReader.GetConfigValue("SmtpPassword"),
                    ConfigReader.GetConfigValue("SmtpSenderName"),
                    ConfigReader.GetConfigValue("SmtpSenderAddress")))
        {
        }

        public InvitationService(BlobShareDataStoreEntities context, IPermissionService permissionService, INotificationService notificationService)
        {
            this.context = context;
            this.permissionService = permissionService;
            this.notificationService = notificationService;
        }

        public Invitation InviteUser(User user, string invitationLink, DateTime expiration, string personalMessage, bool signEmail)
        {
            var invitation = this.context.Invitations.FirstOrDefault(i => i.User.UserId == user.UserId && i.ActivationDateTime == null);

            if (invitation == null)
            {
               invitation = new Invitation
               {
                   InvitationId = Guid.NewGuid(),
                   User = user,
                   Email = user.Email,
                   ExpirationDateTime = expiration,
                   CreationDateTime = DateTime.UtcNow,
                   SentDateTime = DateTime.UtcNow
               };

               this.context.Invitations.AddObject(invitation);
            }

            invitation.ExpirationDateTime = expiration;

            if (user.Email != invitation.Email)
            {
                invitation.Email = user.Email;
            }
            
            this.context.SaveChanges();

            // Send invitation link
            invitationLink += "/" + invitation.InvitationId;
            this.SendNotification(invitation, invitationLink, personalMessage, signEmail);

            return invitation;
        }

        public void ActivateUserInvitation(Invitation invitation, User user)
        {
            invitation.ActivationDateTime = DateTime.UtcNow;
            this.context.SaveChanges();
        }

        public Invitation RetrieveInvitation(Guid invitationId)
        {
            return this.context.Invitations.Where(i => i.InvitationId == invitationId).SingleOrDefault();
        }

        public IEnumerable<Invitation> GetInvitations()
        {
            return this.context.Invitations.OrderByDescending(i => i.CreationDateTime);
        }

        public IEnumerable<Invitation> GetPendingInvitations()
        {
            return this.context.Invitations.Where(i => i.ActivationDateTime == null).OrderByDescending(i => i.CreationDateTime);
        }

        public int GetPendingInvitationsCount()
        {
            return this.context.Invitations.Where(i => i.ActivationDateTime == null).OrderByDescending(i => i.CreationDateTime).Count();
        }

        private static string GetCurrentUserName()
        {
            var identity = Thread.CurrentPrincipal.Identity as IClaimsIdentity;
            if (identity != null && identity.Claims.Any(c => c.ClaimType == ClaimTypes.Email))
            {
                return identity.Claims.Single(c => c.ClaimType == ClaimTypes.Email).Value;
            }

            return Thread.CurrentPrincipal.Identity.Name;
        }

        private static string GetCurrentUserIdentityName()
        {
            return Thread.CurrentPrincipal.Identity.Name;
        }

        private void SendNotification(Invitation invitation, string invitationLink, string personalMessage, bool signEmail)
        {
            if (personalMessage == null)
            {
                personalMessage = string.Empty;
            }

            personalMessage = personalMessage.Replace("\n", "\n<br/>");

            var notificationSubject = string.Format(CultureInfo.InvariantCulture, "{0} wants to invite you to Blob Share", GetCurrentUserName());

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
            .Append("<p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'><br>Hi! You've been invited to Blob Share. </span></p>")
            .Append("<p></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>Blob Share is a Windows Azure based application that allows you to upload, view, download and share your blobs on the cloud.  Start using Blob Share now.</span></p><p></p>")
            .Append("<p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>In order to accept this invitation, please visit the following URL:&nbsp;<a style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#ff6600' href=\"{activationUri}\">{activationUri}</a></span></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>{personalMessage}</span></p><p><span style='font-size:11.0pt;font-family:\"Calibri\",\"sans-serif\";color:#7b8185'>{signature}</span></p><br>")
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

            var messageParameters = new Dictionary<string, string>
            {
                { "{signature}", signature },
                { "{activationUri}", invitationLink },
                { "{personalMessage}", personalMessage }
            };

            using (LinkedResource logo = new LinkedResource(HttpContext.Current.Server.MapPath("~/Content/Images/home_bg_mail.jpg"), MediaTypeNames.Image.Jpeg))
            {
                logo.ContentId = "logo";
                logo.TransferEncoding = TransferEncoding.Base64;

                this.notificationService.SendNotification(new string[] { invitation.Email }, notificationSubject, notificationTemplate.ToString(), logo, messageParameters);
            }
        }
    }
}