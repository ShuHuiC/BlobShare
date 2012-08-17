namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BlobSetNotificationViewModel
    {
        public Guid BlobSetId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a subject for the notification")]
        public string Subject { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You need to specify a message for the notification")]
        public string Message { get; set; }

        public string Name { get; set; }
    }
}