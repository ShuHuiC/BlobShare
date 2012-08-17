namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class BlobSetPermissionViewModel
    {
        public BlobSetPermissionViewModel()
        {
            this.Expirations = new ExpirationList();
        }

        public BlobSet BlobSet { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public IEnumerable<SelectListItem> Expirations { get; set; }
    }
}