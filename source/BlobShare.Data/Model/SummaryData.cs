namespace Microsoft.Samples.DPE.BlobShare.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SummaryData
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public long Total { get; set; }

        public bool IsBlobSet { get; set; }
    }
}
