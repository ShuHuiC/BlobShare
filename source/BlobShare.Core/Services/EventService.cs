namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class EventService : IEventService
    {
        private BlobShareDataStoreEntities context;
        private IUserService userService;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public EventService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        public EventService(BlobShareDataStoreEntities context)
            : this(context, new UserService(context))
        {
        }

        public EventService(BlobShareDataStoreEntities context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public void CreateEventUserViewBlob(User user, Blob blob)
        {
            this.CreateEventUserViewBlob(user, blob, this.GetRequestData());
        }

        public void CreateEventUserViewBlob(User user, Blob blob, RequestData request)
        {
            BlobEvent @event = new BlobEvent() 
            {
                BlobEventId = Guid.NewGuid(),
                EventDateTime = DateTime.UtcNow,
                EventType = (int)EventType.View,
                User = user,
                Blob = blob
            };

            CompleteBlobEvent(@event, request);

            this.context.BlobEvents.AddObject(@event);
            this.context.SaveChanges();
        }

        public void CreateEventUserDownloadBlob(User user, Blob blob)
        {
            this.CreateEventUserDownloadBlob(user, blob, this.GetRequestData());
        }

        public void CreateEventUserDownloadBlob(User user, Blob blob, RequestData request)
        {
            BlobEvent @event = new BlobEvent()
            {
                BlobEventId = Guid.NewGuid(),
                EventDateTime = DateTime.UtcNow,
                EventType = (int)EventType.Download,
                User = user,
                Blob = blob
            };

            CompleteBlobEvent(@event, request);

            this.context.BlobEvents.AddObject(@event);
            this.context.SaveChanges();
        }

        public void CreateEventUserViewBlobSet(User user, BlobSet set)
        {
            this.CreateEventUserViewBlobSet(user, set, this.GetRequestData());
        }

        public void CreateEventUserViewBlobSet(User user, BlobSet set, RequestData request)
        {
            BlobSetEvent @event = new BlobSetEvent()
            {
                BlobSetEventId = Guid.NewGuid(),
                EventDateTime = DateTime.UtcNow,
                EventType = (int)EventType.View,
                User = user,
                BlobSet = set
            };

            CompleteBlobSetEvent(@event, request);

            this.context.BlobSetEvents.AddObject(@event);
            this.context.SaveChanges();
        }

        public void CreateEventUserViewBlobSet(string nameIdentifier, string identityProvider, BlobSet set)
        {
            this.CreateEventUserViewBlobSet(nameIdentifier, identityProvider, set, this.GetRequestData());
        }

        public void CreateEventUserViewBlobSet(string nameIdentifier, string identityProvider, BlobSet set, RequestData request)
        {
            User user = this.userService.RetrieveUserByNameIdentifier(nameIdentifier, identityProvider);
            this.CreateEventUserViewBlobSet(user, set, request);
        }

        public void CreateEventUserViewBlob(string nameIdentifier, string identityProvider, Blob blob)
        {
            this.CreateEventUserViewBlob(nameIdentifier, identityProvider, blob, this.GetRequestData());
        }

        public void CreateEventUserViewBlob(string nameIdentifier, string identityProvider, Blob blob, RequestData request)
        {
            User user = this.userService.RetrieveUserByNameIdentifier(nameIdentifier, identityProvider);
            this.CreateEventUserViewBlob(user, blob, request);
        }

        public void CreateEventUserDownloadBlob(string nameIdentifier, string identityProvider, Blob blob)
        {
            this.CreateEventUserDownloadBlob(nameIdentifier, identityProvider, blob, this.GetRequestData());
        }

        public void CreateEventUserDownloadBlob(string nameIdentifier, string identityProvider, Blob blob, RequestData request)
        {
            User user = this.userService.RetrieveUserByNameIdentifier(nameIdentifier, identityProvider);
            this.CreateEventUserDownloadBlob(user, blob, request);
        }

        public void CreateEventUserCreate(User user)
        {
            this.CreateEventUserCreate(user, this.GetRequestData());
        }

        public void CreateEventUserCreate(User user, RequestData request)
        {
            this.CreateUserEvent(user, UserEventType.Create, request);
        }

        public void CreateEventUserActivation(User user)
        {
            this.CreateEventUserActivation(user, this.GetRequestData());
        }

        public void CreateEventUserActivation(User user, RequestData request)
        {
            this.CreateUserEvent(user, UserEventType.Activation, request);
        }

        public void CreateEventUserDeactivation(User user)
        {
            this.CreateEventUserDeactivation(user, this.GetRequestData());
        }

        public void CreateEventUserDeactivation(User user, RequestData request)
        {
            this.CreateUserEvent(user, UserEventType.Deactivation, request);
        }

        public void CreateEventUserLogin(User user)
        {
            this.CreateEventUserLogin(user, this.GetRequestData());
        }

        public void CreateEventUserLogin(User user, RequestData request)
        {
            this.CreateUserEvent(user, UserEventType.Login, request);
        }

        public RequestData GetRequestData()
        {
            if (HttpContext.Current == null)
            {
                return null;
            }

            HttpRequest currentRequest = HttpContext.Current.Request;

            RequestData request = new RequestData()
            {
                Url = currentRequest.RawUrl,
                UserAgent = currentRequest.UserAgent,
                RemoteMachine = string.IsNullOrEmpty(currentRequest.UserHostName) ? currentRequest.UserHostAddress : currentRequest.UserHostAddress,
                SessionId = null
            };

            return request;
        }

        private static void CompleteUserEvent(UserEvent @event, RequestData request)
        {
            if (request != null)
            {
                @event.Url = request.Url;
                @event.UserAgent = request.UserAgent;
                @event.RemoteMachine = request.RemoteMachine;
                @event.SessionId = request.SessionId;
            }
        }

        private static void CompleteBlobEvent(BlobEvent @event, RequestData request)
        {
            if (request != null)
            {
                @event.Url = request.Url;
                @event.UserAgent = request.UserAgent;
                @event.RemoteMachine = request.RemoteMachine;
                @event.SessionId = request.SessionId;
            }
        }

        private static void CompleteBlobSetEvent(BlobSetEvent @event, RequestData request)
        {
            if (request != null)
            {
                @event.Url = request.Url;
                @event.UserAgent = request.UserAgent;
                @event.RemoteMachine = request.RemoteMachine;
                @event.SessionId = request.SessionId;
            }
        }

        private void CreateUserEvent(User user, UserEventType type, RequestData request)
        {
            UserEvent @event = new UserEvent()
            {
                UserEventId = Guid.NewGuid(),
                User = user,
                EventType = (int)type,
                EventDateTime = DateTime.UtcNow
            };

            CompleteUserEvent(@event, request);

            this.context.UserEvents.AddObject(@event);
            this.context.SaveChanges();
        }
    }
}