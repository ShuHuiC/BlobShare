namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;

    public class PermissionViewModel
    {
        public bool IsBlob { get; set; }
        
        public string BlobSetName { get; set; }
        
        public Guid BlobSetId { get; set; }
        
        public string BlobName { get; set; }

        public Guid BlobId { get; set; }
    }
}