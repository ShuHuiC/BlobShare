namespace Microsoft.Samples.DPE.BlobShare.Core.Helpers
{
    using System.Configuration;
    using System.Security.Permissions;

    public static class ConfigReader
    {
        [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        public static string GetConfigValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}