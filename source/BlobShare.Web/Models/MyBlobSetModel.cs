namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class MyBlobSetViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public IEnumerable<BlobViewModel> Blobs { get; set; }
    }
}