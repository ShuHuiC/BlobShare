namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class UserViewModel
    {
        public Guid Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a name for the user")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a email for the user")]
        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                         + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessage = "The email entered has an invalid format")]
        public string Email { get; set; }

        public int Status { get; set; }     

        public string PersonalMessage { get; set; }

        // Edit Specific
        public string OriginalName { get; set; }

        // Details Specific
        public User User { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
     
        // Create Specific
        public IEnumerable<string> RoleNames { get; set; }

        public bool IsMe { get; set; }

        public string CustomErrors { get; set; }
    }
}