namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public class UserInviteViewModel
    {
        public User User { get; set; }

        public string PersonalMessage { get; set; }

        public bool SignEmail { get; set; }
    }
}