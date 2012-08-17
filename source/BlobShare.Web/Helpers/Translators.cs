namespace Microsoft.Samples.DPE.BlobShare.Web.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;

    public static class Translators
    {
        public static IEnumerable<BlobViewModel> ToViewModel(this IEnumerable<Blob> model)
        {
            return model.Select(i => i.ToViewModel());
        }

        public static BlobViewModel ToViewModel(this Blob model)
        {
            return new BlobViewModel
            {
                BlobId = model.BlobId,
                Description = model.Description,
                Name = model.Name,
                OriginalFileName = model.OriginalFileName,
            };
        }

        public static IEnumerable<BlobSetViewModel> ToViewModel(this IEnumerable<BlobSet> model)
        {
            return model.Select(i => i.ToViewModel());
        }

        public static BlobSetViewModel ToViewModel(this BlobSet model)
        {
            return new BlobSetViewModel
            {
                BlobSetId = model.BlobSetId,
                Description = model.Description,
                Name = model.Name,
            };
        }

        public static IEnumerable<PermissionViewModel> ToViewModel(this IEnumerable<Permission> model)
        {
            return model.Select(i => i.ToViewModel());
        }

        public static PermissionViewModel ToViewModel(this Permission model)
        {
            return new PermissionViewModel
            {
                IsBlob = model.Blob != null,
                BlobSetName = model.BlobSet == null ? string.Empty : model.BlobSet.Name,
                BlobSetId = model.BlobSet == null ? Guid.Empty : model.BlobSet.BlobSetId,
                BlobName = model.Blob == null ? string.Empty : model.Blob.Name,
                BlobId = model.Blob == null ? Guid.Empty : model.Blob.BlobId,
            };
        }
    }
}