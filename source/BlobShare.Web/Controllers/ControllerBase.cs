namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;

    public abstract class ControllerBase : Controller
    {
        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.Demand, Unrestricted = true)]
        public ControllerBase()
        {
            this.Context = BlobShareDataStoreEntities.CreateInstance();
        }

        protected BlobShareDataStoreEntities Context { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}