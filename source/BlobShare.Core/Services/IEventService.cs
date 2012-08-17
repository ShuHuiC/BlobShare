namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IEventService
    {
        void CreateEventUserViewBlob(User user, Blob blob);

        void CreateEventUserViewBlob(User user, Blob blob, RequestData request);

        void CreateEventUserDownloadBlob(User user, Blob blob);

        void CreateEventUserDownloadBlob(User user, Blob blob, RequestData request);

        void CreateEventUserViewBlobSet(User user, BlobSet set, RequestData request);

        void CreateEventUserViewBlob(string nameIdentifier, string identityProvider, Blob blob);

        void CreateEventUserViewBlob(string nameIdentifier, string identityProvider, Blob blob, RequestData request);

        void CreateEventUserDownloadBlob(string nameIdentifier, string identityProvider, Blob blob);

        void CreateEventUserDownloadBlob(string nameIdentifier, string identityProvider, Blob blob, RequestData request);

        void CreateEventUserViewBlobSet(string nameIdentifier, string identityProvider, BlobSet set);

        void CreateEventUserViewBlobSet(string nameIdentifier, string identityProvider, BlobSet set, RequestData request);

        void CreateEventUserCreate(User user);

        void CreateEventUserCreate(User user, RequestData request);

        void CreateEventUserActivation(User user);

        void CreateEventUserActivation(User user, RequestData request);

        void CreateEventUserDeactivation(User user);

        void CreateEventUserDeactivation(User user, RequestData request);

        void CreateEventUserLogin(User user);

        void CreateEventUserLogin(User user, RequestData request);

        RequestData GetRequestData();
    }
}