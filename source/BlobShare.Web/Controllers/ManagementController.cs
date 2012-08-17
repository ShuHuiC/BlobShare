namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Configuration;
    using System.Text;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.WindowsAzure;

    [CustomAuthorize(Roles = "Administrator")]
    public class ManagementController : ControllerBase
    {
        private readonly IBlobService blobService;
        private readonly IBlobSetService blobSetService;
        private readonly IInvitationService invitationService;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public ManagementController()
        {
            this.blobService = new BlobService(this.Context, CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")), ConfigReader.GetConfigValue("MainBlobContanier"));
            this.blobSetService = new BlobSetService(this.Context);
            this.invitationService = new InvitationService(this.Context);
        }

        public ManagementController(IBlobService blobService, IBlobSetService blobSetService, IInvitationService invitationService)
        {
            this.blobService = blobService;
            this.blobSetService = blobSetService;
            this.invitationService = invitationService;
        }

        private string GetInvitationPage()
        {
            Uri reqUrl = this.Request.Url;
            var invitationPage = new StringBuilder();
            invitationPage.Append(reqUrl.Scheme);
            invitationPage.Append("://");
            invitationPage.Append(this.Request.Headers["Host"] ?? reqUrl.Authority);
            invitationPage.Append(ConfigurationManager.AppSettings["UserAccountInvitationAction"]);

            return invitationPage.ToString();
        }
    }
}