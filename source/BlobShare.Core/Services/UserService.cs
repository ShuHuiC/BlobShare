namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Linq;
    using System.Security.Permissions;
    using System.Threading;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class UserService : IUserService
    {
        private const string IdentityProviderClaimType = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        private readonly BlobShareDataStoreEntities context;
        private readonly IEventService eventService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public UserService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        public UserService(BlobShareDataStoreEntities context)
        {
            this.context = context;
            this.eventService = new EventService(context, this);
        }

        public UserService(BlobShareDataStoreEntities context, IEventService eventService)
        {
            this.context = context;
            this.eventService = eventService;
        }

        public User RetrieveUserByNameIdentifier(string nameIdentifier, string identityProvider)
        {
            return this.context.Users.Where(
                u =>
                    u.NameIdentifier.Equals(nameIdentifier, StringComparison.OrdinalIgnoreCase) &&
                    u.IdentityProvider.Equals(identityProvider, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault();
        }

        public User RetrieveUserByInvitationId(Guid invitationId)
        {
            return this.context.Users.Where(
                u => u.Invitations.FirstOrDefault(i => i.InvitationId == invitationId) != null).SingleOrDefault();
        }

        public void CreateUser(User user)
        {
            if (this.context.Users.SingleOrDefault(u => u.UserId == user.UserId || (u.NameIdentifier == user.NameIdentifier && u.IdentityProvider == user.IdentityProvider) || u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)) != null)
            {
                throw new InvalidOperationException("User already exists.");
            }

            if (user.UserId == Guid.Empty)
            {
                user.UserId = Guid.NewGuid();
            }
            
            this.context.Users.AddObject(user);
            this.context.SaveChanges();
            this.eventService.CreateEventUserCreate(user);
        }

        public void UpdateUser(User user)
        {
            var currentUser = this.context.Users.SingleOrDefault(u => u.UserId == user.UserId);

            if (currentUser != null)
            {
                if (!currentUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var findUser = this.context.Users.SingleOrDefault(u => u.Email == user.Email);

                    if (findUser != null)
                    {
                        throw new InvalidOperationException("The email address is currently assigned to another user.");
                    }
                }

                if (currentUser.Inactive != user.Inactive)
                {
                    if (currentUser.Inactive)
                    {
                        this.eventService.CreateEventUserActivation(currentUser);
                    }
                    else
                    {
                        this.eventService.CreateEventUserDeactivation(currentUser);
                    }
                }

                currentUser.Name = user.Name;
                currentUser.Email = user.Email;
                
                if (!string.IsNullOrWhiteSpace(user.NameIdentifier))
                {
                    currentUser.NameIdentifier = user.NameIdentifier;
                }

                if (!string.IsNullOrWhiteSpace(user.IdentityProvider))
                {
                    currentUser.IdentityProvider = user.IdentityProvider;
                }

                currentUser.Inactive = user.Inactive;

                this.context.SaveChanges();
            }
            else
            {
                throw new InvalidOperationException("User does not exist.");
            }
        }

        public IQueryable<User> GetUsers()
        {
            return this.context.Users.OrderBy(u => u.Name);
        }

        public User RetrieveUserById(Guid id)
        {
            return this.context.Users.Where(u => u.UserId == id).SingleOrDefault();
        }

        public User RetrieveUserByEMail(string email)
        {
            return this.context.Users.Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
        }

        public void ActivateUser(User user)
        {
            user.Inactive = false;
            this.context.SaveChanges();
            this.eventService.CreateEventUserActivation(user);
        }

        public void DeactivateUser(User user)
        {
            user.Inactive = true;
            this.context.SaveChanges();
            this.eventService.CreateEventUserDeactivation(user);
        }

        public IQueryable<User> SearchUsers(string partialName)
        {
            return this.context.Users.Where(u => u.Name.Contains(partialName)).OrderBy(u => u.Name);
        }

        public void DeleteUser(User user)
        {
            if (this.IsMe(user))
            {
                throw new InvalidOperationException("You cannot delete yourself.");
            }

            int count = this.context.Users.Count();

            if (count == 1)
            {
                throw new InvalidOperationException("The last user cannot be deleted.");
            }

            this.context.Users.DeleteObject(user);
            this.context.SaveChanges();
        }

        public bool IsMe(User user)
        {
            var identity = ((IClaimsPrincipal)Thread.CurrentPrincipal).Identity as ClaimsIdentity;
            var emailClaim = identity.Claims.Where(c => c.ClaimType.Equals(ClaimTypes.Email, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (string.Compare(emailClaim.Value, user.Email, true) == 0)
            {
                return true;
            }

            return false;
        }
    }
}