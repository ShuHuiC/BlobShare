namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Helpers
{
    using System;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DateTimeHelperTest
    {
        [TestMethod]
        public void ConvertToLocalZone()
        {
            DateTime now = DateTime.UtcNow;

            DateTime local = DateTimeHelper.ToLocalTime(now);

            Assert.AreNotEqual(local, now);
            Assert.AreEqual(local, now.AddHours(-7));
        }

        [TestMethod]
        public void ConvertToUtcZone()
        {
            DateTime now = DateTime.Now;

            DateTime utc = DateTimeHelper.FromLocalTime(now);

            Assert.AreNotEqual(utc, now);
            Assert.AreEqual(now, utc.AddHours(-7));
        }
    }
}
