namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class RegisterAdminViewModel
    {
        [Required(ErrorMessage = "Bootstrap Administrator Secret is required"), DisplayName("Bootstrap Administrator Secret")]
        public string BootstrapAdministratorSecret { get; set; }

        [DisplayName("Email Address")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required")]
        [RegularExpression(@"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
                         + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
                         + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?
                                    [0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
                         + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$", ErrorMessage = "The email entered has an invalid format")]
        public string AdministratorEmail { get; set; }
    }
}