namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class BlobUpdateDescriptionModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OriginalName { get; set; }
    }
}