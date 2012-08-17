namespace Microsoft.Samples.DPE.BlobShare.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class RequestData
    {
        public string Url { get; set; }

        public string UserAgent { get; set; }

        public string RemoteMachine { get; set; }

        public string SessionId { get; set; }
    }
}
