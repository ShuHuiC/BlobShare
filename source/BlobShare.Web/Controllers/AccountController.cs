namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Security;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.IdentityModel.Protocols.WSFederation;
    using Microsoft.IdentityModel.Web;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;

    [HandleError]    
    [ValidateInput(false)]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    public class AccountController : ControllerBase
    {
        public const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        private IUserService userService;
        private IRoleService roleService;

        public AccountController()
        {
            this.userService = new UserService(this.Context);
            this.roleService = new RoleService(this.Context);
        }

        public AccountController(IUserService userService, IRoleService roleService)
        {
            this.userService = userService;
            this.roleService = roleService;
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult LogOn(string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = "/";
            }

            return this.LogOnCommon(returnUrl);
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogOn()
        {
            return this.LogOnCommon(null);
        }

        [Authorize]
        public ActionResult Invitation(string ticketNumber)
        {
            if (this.Request.UrlReferrer == null)
            {
                // Request is not coming from ACS
                // Force the call to the ClaimsAuthenticationManager
                FederatedAuthentication.ServiceConfiguration.ClaimsAuthenticationManager.Authenticate(
                    this.Request.Url.AbsolutePath,
                    (IClaimsPrincipal)Thread.CurrentPrincipal);
            }

            Guid invitationId;

            try
            {
                var urlSplitted = this.Request.Url.AbsoluteUri.Split(new char[1] { '/' });
                invitationId = new Guid(urlSplitted[urlSplitted.Length - 1]);
            } 
            catch (Exception ex)
            {
                throw new InvalidOperationException("The invitation Id could not be retrieved from the url.", ex);
            }

            var user = this.userService.RetrieveUserByInvitationId(invitationId);

            if (user == null)
            {
                throw new InvalidOperationException("The user associated with the invitation could not be found.");
            }

            var name = User.Identity.Name;

            var model = new ProfileViewModel()
            {
                Id = user.UserId,
                Name = user.Name,
                OriginalName = user.Name
            };

            return this.View("Invitation", model);
        }

        [HttpGet]
        public new ActionResult Profile()
        {
            var name = User.Identity.Name;
            var user = this.userService.SearchUsers(name).FirstOrDefault();

            var model = new ProfileViewModel()
            {
                Id = user.UserId,
                Name = user.Name,
                OriginalName = user.Name
            };

            return this.View(model);
        }

        [HttpPost]
        public new ActionResult Profile(ProfileViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var claimsIdentity = (IClaimsIdentity)User.Identity;

            var emailClaim = claimsIdentity.Claims.SingleOrDefault(c => c.ClaimType.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase));

            var email = string.Empty;

            if (emailClaim == null)
            {
                throw new InvalidOperationException("The email address could not be retrieved from the claims identity object.");
            }
            else
            {
                email = emailClaim.Value;
            }

            var user = this.userService.RetrieveUserByEMail(email);

            if (user == null)
            {
                throw new InvalidOperationException(string.Format("No user could be found with the claims email address '{0}'.", email));
            }

            user.Name = model.Name;

            this.userService.UpdateUser(user);

            this.ExecuteLogOff();
            return this.RedirectToAction("Index", "Home");
        }

        public ActionResult LogOff()
        {
            this.ExecuteLogOff();

            return this.RedirectToAction("Index", "Home");
        }

        [AcceptVerbs(HttpVerbs.Get), Authorize]
        public ActionResult RegisterAdmin()
        {
            var identity = this.HttpContext.User.Identity as IClaimsIdentity;
            var emailIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            return View(
                new RegisterAdminViewModel
                {
                    BootstrapAdministratorSecret = string.Empty,
                    AdministratorEmail = emailIdentifierClaim == null ? string.Empty : emailIdentifierClaim.Value
                });
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        [AcceptVerbs(HttpVerbs.Post), Authorize]
        public ActionResult RegisterAdmin(RegisterAdminViewModel model)
        {
            if (model.BootstrapAdministratorSecret != ConfigReader.GetConfigValue("BootstrapAdministratorSecret"))
            {
                this.ModelState.AddModelError("BootstrapAdministratorSecret", "The provided Bootstrap Administrator Secret is invalid.");
            }

            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            if (this.userService.GetUsers().Count() > 0)
            {
                // If a user exists, then this action should not occur.
                return RedirectToAction("Index", "Home");
            }

            var identity = this.HttpContext.User.Identity as IClaimsIdentity;

            var nameIdentifierClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.NameIdentifier, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            var identityProviderClaim = identity.Claims.Where(c => c.ClaimType.Equals(IdentityProviderClaimType, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            var user = new User()
            {
                Name = model.AdministratorEmail,
                Email = model.AdministratorEmail,
                NameIdentifier = nameIdentifierClaim.Value,
                IdentityProvider = identityProviderClaim.Value,
            };

            this.userService.CreateUser(user);
            var role = this.roleService.GetRoleByName("Administrator");
            this.roleService.AddUserToRole(role, user);

            this.ExecuteLogOff();

            return RedirectToAction("RegistrationSuccess");
        }

        public ActionResult RegistrationSuccess()
        {
            return View();
        }

        /// <summary>
        /// This method extracts the WS-Federation passive context from the current HTTP request,
        /// if it is a valid protocol message.
        /// </summary>
        /// <returns>Context string if it exists; otherwise String.Empty</returns>
        private string GetContextFromRequest()
        {
            Uri requestBaseUrl = WSFederationMessage.GetBaseUrl(Request.Url);
            WSFederationMessage message = WSFederationMessage.CreateFromNameValueCollection(requestBaseUrl, Request.Form);
            return message != null ? message.Context : string.Empty;
        }

        private string GetRealm()
        {
            Uri reqUrl = this.Request.Url;
            var realm = new StringBuilder();
            realm.Append(reqUrl.Scheme);
            realm.Append("://");
            realm.Append(this.Request.Headers["Host"] ?? reqUrl.Authority);
            realm.Append(this.Request.ApplicationPath);
            if (!this.Request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                realm.Append("/");
            }

            return realm.ToString();
        }

        private string GetWReply()
        {
            Uri reqUrl = this.Request.Url;
            var realm = this.GetRealm();
            var wreply = new StringBuilder(realm);
            wreply.Append("Account/LogOn");

            return wreply.ToString();
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        private ActionResult LogOnCommon(string returnUrl)
        {
            // If the request is unauthenticated, Redirect to the LogOn page.
            if (!Request.IsAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    returnUrl = "/";
                }

                var model = new LogOnViewModel
                {
                    AcsNamespace = ConfigReader.GetConfigValue("AcsNamespace"),
                    Realm = Uri.EscapeDataString(this.GetRealm()),
                    ReplayTo = Uri.EscapeDataString(this.GetWReply()),
                    Context = Uri.EscapeDataString(returnUrl)
                };

                return View(model);
            }

            // If the request is authenticated and is not a postback from acs, return unauthroized
            if (Request.IsAuthenticated && !this.IsSignInResponse())
            {
                return new RedirectResult("~/403.htm");
            }

            // Request is already authenticated.
            // Redirect to the URL the user was trying to access before being authenticated.
            string effectiveReturnUrl = returnUrl;

            // If no return URL was specified, try to get it from the Request context.
            if (string.IsNullOrEmpty(effectiveReturnUrl))
            {
                effectiveReturnUrl = this.GetContextFromRequest();
            }

            // If there is a return URL, Redirect to it. Otherwise, Redirect to Home.
            if (!string.IsNullOrEmpty(effectiveReturnUrl))
            {
                return Redirect(effectiveReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private void ExecuteLogOff()
        {
            try
            {
                FormsAuthentication.SignOut();
            }
            finally
            {
                FederatedAuthentication.WSFederationAuthenticationModule.SignOut(true);
            }
        }

        private bool IsSignInResponse()
        {
            if (Request.HttpMethod != "POST")
            {
                return false;
            }

            if (Request.Form.AllKeys.Contains("wa"))
            {
                return true;
            }

            if (Request.Form.AllKeys.Contains("wresult"))
            {
                return true;
            }

            if (Request.Form.AllKeys.Contains("wctx"))
            {
                return true;
            }

            return false;
        }
    }
}