namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.WindowsAzure;

    [CustomAuthorize]
    public class MyBlobsController : ControllerBase
    {
        public const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        private IPermissionService permissionService;
        private IBlobService blobService;
        private IBlobSetService blobSetService;
        private IEventService eventService;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public MyBlobsController()
        {
            this.permissionService = new PermissionService(this.Context);
            this.blobService = new BlobService(this.Context, CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")), ConfigReader.GetConfigValue("MainBlobContanier"));
            this.blobSetService = new BlobSetService(this.Context);
            this.eventService = new EventService(this.Context);
        }

        public MyBlobsController(IPermissionService permissionService, IBlobService blobService, IBlobSetService blobSetService, IEventService eventService)
        {
            this.permissionService = permissionService;
            this.blobService = blobService;
            this.blobSetService = blobSetService;
            this.eventService = eventService;
        }

        public ActionResult Index()
        {
            return this.View();
        }

        public JsonResult MyBlobs()
        {
            IClaimsIdentity identity = (IClaimsIdentity)User.Identity;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            // Query Blob Sets
            var sets = this.permissionService
                        .GetBlobSetsByUser(nameIdentifierClaim.Value, identityProviderClaim.Value)
                        .OrderBy(rl => rl.Name)
                        .Select(
                            r =>
                                new
                                {
                                    Name = r.Name,
                                    Description = r.Description,
                                    BlobSetId = r.BlobSetId,
                                    BlobId = Guid.Empty,
                                    IsBlob = false
                                });

            var items = sets.ToList();

            // Query Blobs
            var blobs = this.permissionService
                        .GetBlobsByUser(nameIdentifierClaim.Value, identityProviderClaim.Value)
                        .OrderBy(br => br.Name)
                        .Select(
                            r =>
                                new
                                {
                                    Name = r.Name,
                                    Description = r.Description,
                                    BlobSetId = Guid.Empty,
                                    BlobId = r.BlobId,
                                    IsBlob = true
                                });

            items.AddRange(blobs);

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewBlobSet(Guid blobSetId)
        {
            var blobSet = this.blobSetService.GetBlobSetById(blobSetId);

            IClaimsIdentity identity = (IClaimsIdentity)User.Identity;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            this.eventService.CreateEventUserViewBlobSet(nameIdentifierClaim.Value, identityProviderClaim.Value, blobSet);

            var model = new MyBlobSetViewModel
            {
                Name = blobSet.Name,
                Description = blobSet.Description,
                Blobs = blobSet.Blobs.ToViewModel().OrderBy(br => br.Name),
            };

            return this.View(model);
        }

        public ActionResult ViewBlob(Guid blobId)
        {
            IClaimsIdentity identity = (IClaimsIdentity)User.Identity;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            var blob = this.blobService.GetBlobById(blobId);

            this.eventService.CreateEventUserViewBlob(nameIdentifierClaim.Value, identityProviderClaim.Value, blob);
            if (blob == null)
            {
                return new RedirectResult("~/404.htm");
            }

            Response.ContentType = this.blobService.GetContentType(blob);
            Response.AddHeader("Content-disposition", "inline; filename=" + blob.OriginalFileName);

            var blob2 = this.blobService.GetBlob(blob);
            blob2.DownloadToStream(Response.OutputStream);
            
            return null;
        }

        public ActionResult DownloadBlob(Guid blobId)
        {
            IClaimsIdentity identity = (IClaimsIdentity)User.Identity;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            var blob = this.blobService.GetBlobById(blobId);

            this.eventService.CreateEventUserDownloadBlob(nameIdentifierClaim.Value, identityProviderClaim.Value, blob);
            if (blob == null)
            {
                return new RedirectResult("~/404.htm");
            }

            Response.ContentType = this.blobService.GetContentType(blob);
            Response.AddHeader("Content-disposition", "attachment; filename=" + blob.OriginalFileName);

            var blob2 = this.blobService.GetBlob(blob);
            blob2.DownloadToStream(Response.OutputStream);
            
            return null;
        }
    }
}