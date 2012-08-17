namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Web.Controllers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BlobControllerTest
    {
        [TestMethod]
        public void GetIndex()
        {
            BlobController controller = GetBlobController();

            ActionResult result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<BlobViewModel>));
        }

        [TestMethod]
        public void GetDetails()
        {
            MockBlobService service = new MockBlobService();
            BlobController controller = GetBlobController(service);
            var id = service.GetBlobs().First().BlobId;

            ActionResult result = controller.Details(id);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(BlobViewModel));
        }

        private static BlobController GetBlobController()
        {
            return GetBlobController(new MockBlobService());
        }

        private static BlobController GetBlobController(IBlobService service)
        {
            return new BlobController(service, new MockBlobSetService(), null, null, null);
        }
    }
}
