namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class BlobPermissionViewModel
    {
        public BlobPermissionViewModel()
        {
            this.Expirations = new ExpirationList();
        }

        public Blob Blob { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public IEnumerable<SelectListItem> Expirations { get; set; }
    }
}