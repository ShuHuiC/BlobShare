namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class BlobSetActivityViewModel
    {
        [Required(ErrorMessage = "You need to specify From Date")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "You need to specify To Date")]
        public DateTime ToDate { get; set; }

        public int EventType { get; set; }

        public int OrderType { get; set; }

        public string SelectedBlobSetName { get; set; }

        public IEnumerable<BlobSetEvent> BlobSetEvents { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}