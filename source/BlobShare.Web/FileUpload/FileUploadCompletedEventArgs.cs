namespace Microsoft.Samples.DPE.BlobShare.Web.FileUpload
{
    using System;

    public class FileUploadCompletedEventArgs : EventArgs
    {
        public FileUploadCompletedEventArgs() 
        { 
        }

        public FileUploadCompletedEventArgs(string fileName, DistributedUpload distributedUpload)
        {
            this.FileName = fileName;
            this.DistributedUpload = distributedUpload;
        }

        public DistributedUpload DistributedUpload { get; set; }

        public string FileName { get; set; }
    }
}