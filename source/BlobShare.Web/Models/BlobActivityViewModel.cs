namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class BlobActivityViewModel
    {
        [Required(ErrorMessage = "You need to specify From Date")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "You need to specify To Date")]
        public DateTime ToDate { get; set; }

        public int OrderType { get; set; }

        public string SelectedBlobName { get; set; }

        public IEnumerable<BlobEvent> BlobEvents { get; set; }

        public bool ViewEvents { get; set; }

        public bool DownloadEvents { get; set; }

        public int CurrentPage { get; set; }
        
        public int TotalPages { get; set; }
    }
}