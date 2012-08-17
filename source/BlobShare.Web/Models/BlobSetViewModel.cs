namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class BlobSetViewModel
    {
        public Guid BlobSetId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a name for the blob set")]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Private { get; set; }

        public IEnumerable<string> PendingInvitations { get; set; }

        public IEnumerable<string> Users { get; set; }

        public IEnumerable<SelectListItem> Expirations { get; set; }

        // Details Specific
        public IEnumerable<BlobViewModel> Blobs { get; set; }

        // Edit Specific
        public string OriginalName { get; set; }

        // Create Specific
        public bool AddPermissions { get; set; }
    }
}