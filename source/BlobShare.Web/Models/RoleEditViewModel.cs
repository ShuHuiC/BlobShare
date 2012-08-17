namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class RoleEditViewModel
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a name for the role")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string OriginalName { get; set; }
    }
}