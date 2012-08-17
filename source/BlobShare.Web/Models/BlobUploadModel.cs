namespace Microsoft.Samples.DPE.BlobShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Web;

    public class BlobUploadModel
    {        
        public string Name { get; set; }

        public string Description { get; set; }

        public string StorageFolder { get; set; }        

        public HttpPostedFileBase ContentFile { get; set; }

        public string UploadHandler { get; set; }

        public string Message { get; set; }
    }
}