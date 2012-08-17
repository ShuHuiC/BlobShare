namespace Microsoft.Samples.DPE.BlobShare.Web
{
    using System;
    using System.IO;
    using System.Web;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.FileUpload;
    using Microsoft.WindowsAzure;

    public class FileUploadHandler : IHttpHandler
    {
        private HttpContext ctx;

        public FileUploadHandler()
        {
        }

        public bool IsReusable
        {
            get { return false; }
        }

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public void ProcessRequest(HttpContext context)
        {
            this.ctx = context;

            var fileUpload = new FileUploadProcess();

            fileUpload.FileUploadCompleted += new FileUploadCompletedEvent(this.FileUploadFileUploadCompleted);
            fileUpload.ProcessRequest(context);
        }

        private void FileUploadFileUploadCompleted(object sender, FileUploadCompletedEventArgs e)
        {
            var dataStore = BlobShareDataStoreEntities.CreateInstance();

            try
            {
                var blobService = new BlobService(
                    dataStore,
                    CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")),
                    ConfigReader.GetConfigValue("MainBlobContanier"));

                try
                {
                    if (!string.IsNullOrEmpty(e.FileName))
                    {
                        e.DistributedUpload.Commit();

                        var resource = new Blob();
                        resource.BlobId = Guid.Parse(this.ctx.Request.QueryString["id"]);
                        resource.Name = Path.GetFileNameWithoutExtension(e.FileName);
                        resource.Description = string.Empty;
                        resource.OriginalFileName = e.FileName;
                        resource.UploadDateTime = DateTime.UtcNow;

                        blobService.CreateBlob(resource);
                    }
                }
                catch (Exception ex)
                {
                    this.ctx.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    this.ctx.Response.StatusDescription = ex.Message;
                    return;
                }
            }
            finally
            {
                dataStore.Dispose();
            }
        }
    }
}