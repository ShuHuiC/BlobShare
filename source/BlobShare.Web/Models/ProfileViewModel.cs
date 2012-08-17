namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ProfileViewModel
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a name for the user")]
        public string Name { get; set; }

        // Edit Specific
        public string OriginalName { get; set; }
    }
}