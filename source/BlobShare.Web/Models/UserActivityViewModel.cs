namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class UserActivityViewModel
    {
        [Required(ErrorMessage = "You need to specify From Date")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "You need to specify To Date")]
        public DateTime ToDate { get; set; }

        public bool LoginEvent { get; set; }

        public bool ViewResourceEvent { get; set; }

        public bool ViewSetEvent { get; set; }

        public bool DownloadResourceEvent { get; set; }

        public bool CreateUserEvent { get; set; }

        public bool ActivateUserEvent { get; set; }

        public bool DeactivateUserEvent { get; set; }

        public bool HasUserEvent { get; set; }

        public bool HasBlob { get; set; }

        public bool HasBlobSet { get; set; }

        public string SelectedUserName { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public IEnumerable<GenericEvent> GenericEvents { get; set; }
    }
}