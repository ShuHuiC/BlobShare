namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class BlobInvitationModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<string> PendingInvitations { get; set; }

        public IEnumerable<string> Users { get; set; }

        public IEnumerable<SelectListItem> Expirations { get; set; }
    }
}