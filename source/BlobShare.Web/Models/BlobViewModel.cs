namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;

    public class BlobViewModel
    {
        public Guid BlobId { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

        public string OriginalFileName { get; set; }

        public string Description { get; set; }

        public IEnumerable<BlobSetViewModel> BlobSets { get; set; }
    }
}