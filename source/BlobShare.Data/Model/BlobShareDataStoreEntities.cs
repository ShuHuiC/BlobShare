namespace Microsoft.Samples.DPE.BlobShare.Data.Model
{
    using System;
    using System.Configuration;
    using System.Data.EntityClient;
    using System.Globalization;
    using System.Security.Permissions;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public partial class BlobShareDataStoreEntities
    {
        private const string EntityConnectionStringTemplate = "metadata=res://*/Model.BlobShareModel.csdl|res://*/Model.BlobShareModel.ssdl|res://*/Model.BlobShareModel.msl;provider=System.Data.SqlClient;provider connection string='{0};MultipleActiveResultSets=True'";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Disposed by callers")]
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static BlobShareDataStoreEntities CreateInstance()
        {
            var connectionString = GetSetting("BlobShareDataStoreEntities");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ConfigurationErrorsException("Missing BlobShareDataStoreEntities.");
            }

            var entityConnectionString = string.Format(CultureInfo.InvariantCulture, EntityConnectionStringTemplate, connectionString);
            var context = new BlobShareDataStoreEntities(new EntityConnection(entityConnectionString));
            
            return context;
        }

        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        private static string GetSetting(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}