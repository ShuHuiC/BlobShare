namespace Microsoft.Samples.DPE.BlobShare.Web.FileUpload
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Web;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.WindowsAzure;

    public delegate void FileUploadCompletedEvent(object sender, FileUploadCompletedEventArgs e);

    public class FileUploadProcess
    {
        public event FileUploadCompletedEvent FileUploadCompleted;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public void ProcessRequest(HttpContext context)
        {
            string id = context.Request.QueryString["id"];
            string filename = context.Request.QueryString["filename"];
            bool complete = string.IsNullOrEmpty(context.Request.QueryString["Complete"]) ? true : bool.Parse(context.Request.QueryString["Complete"]);
            bool getBytes = string.IsNullOrEmpty(context.Request.QueryString["GetBytes"]) ? false : bool.Parse(context.Request.QueryString["GetBytes"]);
            long startByte = string.IsNullOrEmpty(context.Request.QueryString["StartByte"]) ? 0 : long.Parse(context.Request.QueryString["StartByte"]);

            var blobUri = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}/{1}{2}",
                    ConfigReader.GetConfigValue("MainBlobContanier"),
                    id,
                    Path.GetExtension(filename));

            var du = new DistributedUpload(
                CloudStorageAccount.FromConfigurationSetting("DataConnectionString"),
                blobUri);

            if (getBytes)
            {
                try
                {
                    du.Begin();
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    context.Response.StatusDescription = ex.Message;
                    return;
                }

                context.Response.Write("0");
                return;
            }
            else
            {
                try
                {
                    this.UploadChunkToBlobStorage(id, startByte, context.Request.InputStream, du);
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    context.Response.StatusDescription = ex.Message;
                    return;
                }

                if (complete)
                {
                    if (this.FileUploadCompleted != null)
                    {
                        var args = new FileUploadCompletedEventArgs(filename, du);
                        this.FileUploadCompleted(this, args);
                    }
                }
            }
        }

        private void UploadChunkToBlobStorage(string id, long startByte, Stream stream, DistributedUpload du)
        {
            du.UploadBlock(stream, startByte);
        }
    }
}