namespace Microsoft.Samples.DPE.BlobShare.Data.Tests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Text;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlobShareModelTests
    {
        [TestMethod]
        public void RetrieveBlobs()
        {
            BlobShareDataStoreEntities context = BlobShareDataStoreEntities.CreateInstance();
            foreach (var item in context.Blobs)
            {
                Assert.IsNotNull(item);
            }
        }
    }
}
