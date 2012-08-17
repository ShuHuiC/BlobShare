namespace Microsoft.Samples.DPE.BlobShare.Security
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
                 
    public class BlobAuthorizationManager : ClaimsAuthorizationManager
    {
        public const string MyBlobsControllerName = "MyBlobs";
        private readonly IPermissionService permissionService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Is disposed in Dispose method")]
        public BlobAuthorizationManager()
            : this(new PermissionService())
        {
        }

        public BlobAuthorizationManager(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        public override bool CheckAccess(AuthorizationContext context)
        {
            var httpContext = HttpContext.Current;
            var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var controller = GetControllerName(routeData);

            if (!string.IsNullOrWhiteSpace(controller) && controller == MyBlobsControllerName)
            {
                if (!httpContext.Request.IsAuthenticated)
                {
                    return false;
                }
                else
                {
                    return this.CheckItemLevelAccess(routeData, (IClaimsIdentity)context.Principal.Identity);
                }
            }

            return true;
        }

        private static string GetControllerName(RouteData routeData)
        {
            object controllerName;

            if (routeData != null && routeData.Values.TryGetValue("controller", out controllerName))
            {
                return controllerName.ToString();
            }

            return null;
        }

        private bool CheckItemLevelAccess(RouteData routeData, IClaimsIdentity identity)
        {
            object id;
            var resourceId = Guid.Empty;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.SingleOrDefault(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase));
            var identityProviderClaim = identity.Claims.SingleOrDefault(c => c.ClaimType.Equals(AccountAssociationClaimsAuthenticationManager.IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase));

            if (routeData.Values.TryGetValue("BlobId", out id))
            {
                // trying to access a blob, verify access
                if (Guid.TryParse(id.ToString(), out resourceId))
                {
                    return this.permissionService.CheckPermissionToBlob(nameIdentifierClaim.Value, identityProviderClaim.Value, resourceId);
                }
            }
            else if (routeData.Values.TryGetValue("blobSetId", out id))
            {
                // trying to access a blob set, verify access
                if (Guid.TryParse(id.ToString(), out resourceId))
                {
                    return this.permissionService.CheckPermissionToBlobSet(nameIdentifierClaim.Value, identityProviderClaim.Value, resourceId);
                }
            }

            // not requesting for a resource
            return true;
        }
    }
}