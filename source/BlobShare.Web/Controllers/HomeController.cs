namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;

    [HandleError]
    public class HomeController : ControllerBase
    {
        private IPermissionService permissionService;
        private IUserService userService;
        private IReportService reportService;

        public HomeController()
        {
            this.permissionService = new PermissionService(this.Context);
            this.userService = new UserService(this.Context);
            this.reportService = new ReportService(this.Context);
        }

        public HomeController(IPermissionService permissionService, IUserService userService, IReportService reportService)
        {
            this.permissionService = permissionService;
            this.userService = userService;
            this.reportService = reportService;
        }

        public ActionResult Index()
        {
            var model = new HomeViewModel();

            if (Request.IsAuthenticated)
            {
                IClaimsIdentity identity = (IClaimsIdentity)User.Identity;

                // Get name identifier and identity provider
                var nameIdentifierClaim = identity.Claims.SingleOrDefault(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase));
                var identityProviderClaim = identity.Claims.SingleOrDefault(c => c.ClaimType.Equals(Microsoft.Samples.DPE.BlobShare.Security.AccountAssociationClaimsAuthenticationManager.IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase));

                User user = this.userService.RetrieveUserByNameIdentifier(nameIdentifierClaim.Value, identityProviderClaim.Value);
                model.Permissions = this.permissionService.GetNewPermissionsByUser(user, 5).ToViewModel();
                
                var visited = this.reportService.GetTopItemsByUser(user, 100);

                var selected = new List<SummaryData>();

                foreach (var v in visited) 
                {
                    if ((v.IsBlobSet && this.permissionService.CheckPermissionToBlobSet(nameIdentifierClaim.Value, identityProviderClaim.Value, v.Id))
                        || (!v.IsBlobSet && this.permissionService.CheckPermissionToBlob(nameIdentifierClaim.Value, identityProviderClaim.Value, v.Id)))
                    {
                        selected.Add(v);
                    }

                    if (selected.Count >= 5)
                    {
                        break;
                    }
                }

                model.MostVisited = selected;
            }

            return View(model);
        }
    }
}