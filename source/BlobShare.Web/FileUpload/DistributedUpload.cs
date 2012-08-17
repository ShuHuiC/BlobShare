namespace Microsoft.Samples.DPE.BlobShare.Web.FileUpload
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class DistributedUpload
    {
        private CloudBlockBlob blob;

        public DistributedUpload(CloudStorageAccount account, string blobAddress)
        {
            var blobStorage = account.CreateCloudBlobClient();
            this.blob = blobStorage.GetBlockBlobReference(blobAddress);
        }

        public void Begin()
        {
            this.blob.Container.CreateIfNotExist();
            this.blob.DeleteIfExists();
        }

        public void UploadBlock(Stream input, long offset)
        {
            input.Seek(0, SeekOrigin.Begin);

            string blockId = this.EncodeOffset(offset);
            string contentMD5 = this.CalculateMD5(input);

            var options = new BlobRequestOptions 
            { 
                RetryPolicy = RetryPolicies.RetryExponential(
                RetryPolicies.DefaultClientRetryCount, 
                RetryPolicies.DefaultClientBackoff) 
            };
            this.blob.PutBlock(blockId, input, contentMD5, options);
        }

        public void Commit()
        {
            try
            {
                this.DoCommit();
            }
            catch (InvalidDataException)
            {
                System.Threading.Thread.Sleep(30000); // wait a few seconds to give enough time to all the blocks to start uploading
                this.DoCommit();
            }
        }

        private void DoCommit()
        {
            // download the list of blocks that were uploaded
            var blockList = this.blob.DownloadBlockList(BlockListingFilter.All);

            // sort the blocks by their offset
            var orderedBlockList = blockList.OrderBy(block =>
            {
                var currentOffset = this.DecodeOffset(block.Name);
                return currentOffset;
            });

            // ensure that there are no gaps in the sequence
            long offset = 0;
            foreach (var bli in orderedBlockList)
            {
                var currentOffset = this.DecodeOffset(bli.Name);

                if (currentOffset != offset)
                {
                    throw new InvalidDataException("Commit failed because the blob is incomplete. One or more blocks are missing.");
                }

                offset += bli.Size;
            }

            // commit the list of uploaded blocks
            this.blob.PutBlockList(orderedBlockList.Select(p => p.Name));
        }

        private string CalculateMD5(Stream input)
        {
            using (var cryptoService = MD5.Create())
            {
                var md5 = cryptoService.ComputeHash(input);
                input.Seek(0, SeekOrigin.Begin);
                return Convert.ToBase64String(md5);
            }
        }

        private string EncodeOffset(long offset)
        {
            return Convert.ToBase64String(System.BitConverter.GetBytes(offset));
        }

        private long DecodeOffset(string blockId)
        {
            return BitConverter.ToInt64(Convert.FromBase64String(blockId), 0);
        }
    }
}
