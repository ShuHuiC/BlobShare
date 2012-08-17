namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;

    public class LogOnViewModel
    {
        public string AcsNamespace { get; set; }

        public string Realm { get; set; }

        public string ReplayTo { get; set; }

        public string Context { get; set; }
    }
}