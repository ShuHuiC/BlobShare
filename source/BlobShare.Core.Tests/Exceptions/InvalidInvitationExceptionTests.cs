namespace Microsoft.Samples.DPE.BlobShare.Core.Tests.Services
{
    using System.Runtime.Serialization;
    using Microsoft.Samples.DPE.BlobShare.Core.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InvalidInvitationExceptionTests
    {
        [TestMethod]
        public void ToStringGetsDescription()
        {
            var description = "test description";
            var message = "test message";
            var ex = new InvalidInvitationException(message, description);

            Assert.AreEqual(description, ex.ToString());
        }

        [TestMethod]
        public void GetObjectData()
        {
            var description = "test description";
            var message = "test message";
            var ex = new InvalidInvitationException(message, description);

            var info = new SerializationInfo(typeof(InvalidInvitationException), new FormatterConverter());
            var context = new StreamingContext();

            ex.GetObjectData(info, context);

            Assert.AreEqual(message, info.GetString("Message"));
        }
    }
}
