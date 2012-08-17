namespace Microsoft.Samples.DPE.BlobShare.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public interface IReportService
    {
        IEnumerable<BlobEvent> GetBlobEvents(DateTime fromdate, DateTime todate, IList<int> types, int pageSize, int currentPage, out int totalPages);

        IEnumerable<BlobEvent> GetBlobEventsByBlob(DateTime fromdate, DateTime todate, IList<int> types, Guid blobId, int pageSize, int currentPage, out int totalPages);
        
        IEnumerable<BlobSetEvent> GetBlobSetEventsByBlobSet(DateTime fromdate, DateTime todate, EventType type, Guid blobSetId, int pageSize, int currentPage, out int totalPages);
        
        IEnumerable<BlobSetEvent> GetBlobSetEvents(DateTime fromdate, DateTime todate, EventType type, int pageSize, int currentPage, out int totalPages);

        IEnumerable<SummaryData> GetTopItemsByUser(User user, int count);

        IEnumerable<GenericEvent> GetUserEvents(DateTime fromdate, DateTime todate, IList<int> userEventTypes, IList<int> setEventTypes, IList<int> resourceEventTypes, int pageSize, int currentPage, out int totalPages);

        IEnumerable<GenericEvent> GetUserEventsByUser(DateTime fromdate, DateTime todate, IList<int> userEventTypes, IList<int> setEventTypes, IList<int> resourceEventTypes, Guid userId, int pageSize, int currentPage, out int totalPages);
    }
}
