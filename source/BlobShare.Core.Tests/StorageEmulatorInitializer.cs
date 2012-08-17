namespace Microsoft.Samples.DPE.BlobShare.Core.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Diagnostics;

    [TestClass]
    public class StorageEmulatorInitializer
    {
        [AssemblyInitialize]
        public static void DevelopmentStorageInitialize(TestContext context)
        {
            var sdkDirectory = Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"), @"Windows Azure Emulator\emulator");
            var processStartInfo = new ProcessStartInfo()
            {
                FileName = Path.Combine(sdkDirectory, "csrun.exe"),
                Arguments = "/devstore",
            };

            using (var process = Process.Start(processStartInfo))
            {
                process.WaitForExit();
            }
        }
    }
}