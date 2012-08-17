namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class HomeViewModel
    {
        public IEnumerable<PermissionViewModel> Permissions { get; set; }

        public IEnumerable<SummaryData> MostVisited { get; set; }
    }
}