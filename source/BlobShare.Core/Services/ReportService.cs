namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Permissions;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public sealed class ReportService : IReportService
    {
        private BlobShareDataStoreEntities context;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed in Dispose method")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public ReportService()
            : this(BlobShareDataStoreEntities.CreateInstance())
        {
        }

        public ReportService(BlobShareDataStoreEntities context)
        {
            this.context = context;
        }

        public IEnumerable<BlobEvent> GetBlobEvents(DateTime fromdate, DateTime todate, IList<int> types, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var count = this.context.BlobEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && types.Contains(ev.EventType)).Count();
            totalPages = (int)Math.Ceiling((decimal)count / pageSize);

            return this.context.BlobEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && types.Contains(ev.EventType)).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<BlobEvent> GetBlobEventsByBlob(DateTime fromdate, DateTime todate, IList<int> types, Guid blobId, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var count = this.context.BlobEvents.Where(ev => ev.Blob.BlobId == blobId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && types.Contains(ev.EventType)).Count();
            totalPages = (int)Math.Ceiling((decimal)count / pageSize);

            return this.context.BlobEvents.Where(ev => ev.Blob.BlobId == blobId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && types.Contains(ev.EventType)).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<BlobSetEvent> GetBlobSetEventsByBlobSet(DateTime fromdate, DateTime todate, EventType type, Guid blobSetId, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var count = this.context.BlobSetEvents.Where(ev => ev.BlobSet.BlobSetId == blobSetId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && ev.EventType == (int)type).Count();
            totalPages = (int)Math.Ceiling((decimal)count / pageSize);

            return this.context.BlobSetEvents.Where(ev => ev.BlobSet.BlobSetId == blobSetId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && ev.EventType == (int)type).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<BlobSetEvent> GetBlobSetEvents(DateTime fromdate, DateTime todate, EventType type, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var count = this.context.BlobSetEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && ev.EventType == (int)type).Count();
            totalPages = (int)Math.Ceiling((decimal)count / pageSize);

            return this.context.BlobSetEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && ev.EventType == (int)type).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize).AsEnumerable();
        }

        public IEnumerable<SummaryData> GetTopItemsByUser(User user, int count)
        {
            var blobs = this.context.BlobEvents.Where(ev => ev.User.UserId == user.UserId).GroupBy(ev => ev.Blob).Select(r => new SummaryData() { Id = r.Key.BlobId, Name = r.Key.Name, Total = r.Count(), IsBlobSet = false }).OrderByDescending(r => r.Total).Take(count);
            var sets = this.context.BlobSetEvents.Where(ev => ev.User.UserId == user.UserId).GroupBy(ev => ev.BlobSet).Select(r => new SummaryData() { Id = r.Key.BlobSetId, Name = r.Key.Name, Total = r.Count(), IsBlobSet = true }).OrderByDescending(r => r.Total).Take(count);

            return blobs.Union(sets).OrderByDescending(s => s.Total).Take(count);
        }

        public IEnumerable<GenericEvent> GetUserEvents(DateTime fromdate, DateTime todate, IList<int> userEventTypes, IList<int> setEventTypes, IList<int> resourceEventTypes, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var userEvents = this.context.UserEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && userEventTypes.Contains(ev.EventType));
            var setEvents = this.context.BlobSetEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && setEventTypes.Contains(ev.EventType));
            var resourceEvents = this.context.BlobEvents.Where(ev => ev.EventDateTime >= fromdate && ev.EventDateTime < todate && resourceEventTypes.Contains(ev.EventType));

            var genericUserEvents = userEvents.Select(ev => new GenericEvent()
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = true,
                BlobSet = null,
                Blob = null,
                User = ev.User,
                UserEventType = ev.EventType,
                EventType = (int)EventType.None,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            var genericSetEvents = setEvents.Select(ev => new GenericEvent
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = false,
                BlobSet = ev.BlobSet,
                Blob = null,
                User = ev.User,
                UserEventType = (int)UserEventType.None,
                EventType = ev.EventType,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            var genericResourceEvents = resourceEvents.Select(ev => new GenericEvent
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = false,
                BlobSet = null,
                Blob = ev.Blob,
                User = ev.User,
                UserEventType = (int)UserEventType.None,
                EventType = ev.EventType,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            totalPages = (int)Math.Ceiling((decimal)genericUserEvents.Count() / pageSize);
            return genericUserEvents.Union(genericSetEvents).Union(genericResourceEvents).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize);
        }

        public IEnumerable<GenericEvent> GetUserEventsByUser(DateTime fromdate, DateTime todate, IList<int> userEventTypes, IList<int> setEventTypes, IList<int> resourceEventTypes, Guid userId, int pageSize, int currentPage, out int totalPages)
        {
            fromdate = NormalizeFromDate(fromdate);
            todate = NormalizeToDate(todate);

            var userEvents = this.context.UserEvents.Where(ev => ev.User.UserId == userId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && userEventTypes.Contains(ev.EventType));
            var setEvents = this.context.BlobSetEvents.Where(ev => ev.User.UserId == userId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && setEventTypes.Contains(ev.EventType));
            var resourceEvents = this.context.BlobEvents.Where(ev => ev.User.UserId == userId && ev.EventDateTime >= fromdate && ev.EventDateTime < todate && resourceEventTypes.Contains(ev.EventType));

            var genericUserEvents = userEvents.Select(ev => new GenericEvent()
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = true,
                BlobSet = null,
                Blob = null,
                User = ev.User,
                UserEventType = ev.EventType,
                EventType = (int)EventType.None,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            var genericSetEvents = setEvents.Select(ev => new GenericEvent()
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = false,
                BlobSet = ev.BlobSet,
                Blob = null,
                User = ev.User,
                UserEventType = (int)UserEventType.None,
                EventType = ev.EventType,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            var genericResourceEvents = resourceEvents.Select(ev => new GenericEvent()
            {
                EventDateTime = ev.EventDateTime,
                IsUserEvent = false,
                BlobSet = null,
                Blob = ev.Blob,
                User = ev.User,
                UserEventType = (int)UserEventType.None,
                EventType = ev.EventType,
                Url = ev.Url,
                UserAgent = ev.UserAgent,
                RemoteMachine = ev.RemoteMachine,
                SessionId = ev.SessionId
            });

            totalPages = (int)Math.Ceiling((decimal)genericUserEvents.Count() / pageSize);
            return genericUserEvents.Union(genericSetEvents).Union(genericResourceEvents).OrderByDescending(ev => ev.EventDateTime).Skip(pageSize * (currentPage - 1)).Take(pageSize);
        }

        private static DateTime NormalizeFromDate(DateTime datetime)
        {
            return datetime;
        }

        private static DateTime NormalizeToDate(DateTime datetime)
        {
            return datetime;
        }
    }
}
