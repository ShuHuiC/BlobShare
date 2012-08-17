namespace Microsoft.Samples.DPE.BlobShare.Security
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Security.Permissions;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.Samples.DPE.BlobShare.Core.Exceptions;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class AccountAssociationClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        public const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        private const string ClaimIssuerName = "BlobShareCAM";

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public override IClaimsPrincipal Authenticate(string resourceName, IClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal.Identity.IsAuthenticated)
            {
                var identity = incomingPrincipal.Identity as IClaimsIdentity;
                var user = EnsureApplicationUser(identity);

                if (user != null)
                {
                    if (identity.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Name) == null)
                    {
                        identity.Claims.Add(new Claim(ClaimTypes.Name, user.Name, user.Name.GetType().Name, ClaimIssuerName));
                    }

                    if (identity.Claims.FirstOrDefault(c => c.ClaimType == ClaimTypes.Email) == null)
                    {
                        identity.Claims.Add(new Claim(ClaimTypes.Email, user.Email, user.Email.GetType().Name, ClaimIssuerName));
                    }

                    foreach (var role in user.Roles)
                    {
                        identity.Claims.Add(new Claim(ClaimTypes.Role, role.RoleName, role.RoleName.GetType().Name, ClaimIssuerName));
                    }
                }
            }

            return incomingPrincipal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "By design")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        private static User EnsureApplicationUser(IClaimsIdentity identity)
        {
            var invitationService = new InvitationService();
            var context = BlobShareDataStoreEntities.CreateInstance();
            var userService = new UserService(context);

            var invitationId = Guid.Empty;
            User user = null;

            // Get name identifier and identity provider
            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            // Administrator First Login
            if (userService.GetUsers().Count() == 0)
            {
                var emailClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                return new User()
                {
                    NameIdentifier = nameIdentifierClaim.Value,
                    IdentityProvider = identityProviderClaim.Value,
                    Email = emailClaim == null ? string.Empty : emailClaim.ToString(),
                    Name = emailClaim == null ? string.Empty : emailClaim.ToString(),
                };
            }

            if (IsInvitationRequest(out invitationId) && !string.IsNullOrWhiteSpace(nameIdentifierClaim.Value))
            {
                // TODO: Lock the activation process
                var invitation = invitationService.RetrieveInvitation(invitationId);

                if (invitation == null)
                {
                    throw new InvalidInvitationException("Invalid Invitation ID", "It seems that the provided invitation ID does not exist. Please contact your administrator.");
                }

                if (invitation.ActivationDateTime.HasValue)
                {
                    throw new InvalidInvitationException("Invitation Already Activated", "It seems that the invitation was already activated. Please contact your administrator.");
                }

                if (DateTime.UtcNow.CompareTo(invitation.ExpirationDateTime) > 0)
                {
                    throw new InvalidInvitationException("Invitation Expired", "It seems that the invitation you are trying to activate has already expired. Please contact your administrator.");
                }

                user = userService.RetrieveUserByNameIdentifier(nameIdentifierClaim.Value, identityProviderClaim.Value);

                if (user != null)
                {
                    if (!user.Email.Equals(invitation.Email))
                    {
                        throw new InvalidUserException("Account Already Linked", "It seems that you have already linked this account with another user. Please try again with a different one.");
                    }
                }
                else
                {
                    user = userService.RetrieveUserById(invitation.User.UserId);
                }

                if (user == null)
                {
                    user = new User()
                    {
                        Name = invitation.Email,
                        Email = invitation.Email,
                        NameIdentifier = nameIdentifierClaim.Value,
                        IdentityProvider = identityProviderClaim.Value
                    };

                    userService.CreateUser(user);
                }
                else
                {
                    if (user.NameIdentifier != nameIdentifierClaim.Value || user.IdentityProvider != identityProviderClaim.Value)
                    {
                        user.NameIdentifier = nameIdentifierClaim.Value;
                        user.IdentityProvider = identityProviderClaim.Value;

                        userService.UpdateUser(user);
                    }
                }

                invitationService.ActivateUserInvitation(invitation, user);
            }
            else
            {
                user = userService.RetrieveUserByNameIdentifier(nameIdentifierClaim.Value, identityProviderClaim.Value);

                if (user == null)
                {
                    throw new InvalidUserException("Invalid User", "It seems that no user is linked to this account, please try again with another or contact your administrator.");
                }
            }

            if (user.Inactive)
            {
                throw new InvalidUserException("Inactive User", "It seems that this user was deactivated, Please contact your administrator.");
            }

            var eventService = new EventService(context);
            eventService.CreateEventUserLogin(user);

            return user;
        }

        private static bool IsInvitationRequest(out Guid invitationNumber)
        {
            Uri requestBaseUrl = WSFederationMessage.GetBaseUrl(HttpContext.Current.Request.Url);
            WSFederationMessage message = WSFederationMessage.CreateFromNameValueCollection(requestBaseUrl, HttpContext.Current.Request.Form);
            invitationNumber = Guid.Empty;

            if (message != null)
            {
                invitationNumber = message.Context.ToUpperInvariant().Contains(ConfigurationManager.AppSettings["UserAccountInvitationAction"].ToUpperInvariant()) ?
                    new Guid(message.Context.Split('/').Last()) :
                    Guid.Empty;
            }
            else
            {
                invitationNumber = requestBaseUrl.AbsolutePath.StartsWith(ConfigurationManager.AppSettings["UserAccountInvitationAction"], StringComparison.OrdinalIgnoreCase) ?
                    new Guid(requestBaseUrl.AbsolutePath.Split('/').Last()) :
                    Guid.Empty;
            }

            return invitationNumber != Guid.Empty;
        }
    }
}