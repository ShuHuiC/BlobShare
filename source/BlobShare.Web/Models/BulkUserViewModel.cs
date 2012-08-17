namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class BulkUserViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify one or more comma separated E-Mails for the invited users")]
        [RegularExpression(@"^[a-zA-Z0-9\._%-]+@[a-zA-Z0-9\.-]+\.[a-zA-Z]{2,4}(?:\s*[,;]\s*[a-zA-Z0-9\._%-]+@[a-zA-Z0-9\.-]+\.[a-zA-Z]{2,4})*$", 
            ErrorMessage = "The email entered has an invalid format")]
        public string Emails { get; set; }

        public string PersonalMessage { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
        
        // Create Specific
        public IEnumerable<string> RoleNames { get; set; }
    }
}