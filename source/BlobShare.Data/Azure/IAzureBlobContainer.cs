namespace Microsoft.Samples.DPE.BlobShare.Data.AzureStorage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.WindowsAzure.StorageClient;

    public interface IAzureBlobContainer<T>
    {
        void EnsureExist();

        void EnsureExist(bool publicContainer);

        void Save(string objId, T obj);

        void SaveAsXml(string objId, T obj);

        string SaveByteArray(string objId, byte[] content, string contentType);

        string SaveByteArray(string objId, byte[] content, string contentType, TimeSpan timeOut);

        string SaveFile(string objId, string filePath, string contentType);

        string SaveFile(string objId, string filePath, string contentType, TimeSpan timeOut);

        string SaveStream(string objId, Stream stream, IDictionary<string, string> metadata);

        CloudBlob GetBlob(string objId);

        T Get(string objId);

        T GetFromXml(string objId);

        Stream GetFile(string objId);

        void Delete(string objId);
    }
}