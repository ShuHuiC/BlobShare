namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class RoleViewModel
    {
        public Role Role { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
    }
}