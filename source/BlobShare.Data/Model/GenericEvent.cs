namespace Microsoft.Samples.DPE.BlobShare.Data.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class GenericEvent
    {
        public DateTime EventDateTime { get; set; }

        public User User { get; set; }

        public BlobSet BlobSet { get; set; }

        public Blob Blob { get; set; }

        public int EventType { get; set; }

        public int UserEventType { get; set; }

        public bool IsUserEvent { get; set; }

        public string Url { get; set; }

        public string UserAgent { get; set; }

        public string RemoteMachine { get; set; }

        public string SessionId { get; set; }
    }
}
