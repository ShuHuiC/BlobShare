namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;

    [CustomAuthorize(Roles = "Administrator")]
    public class InvitationController : ControllerBase
    {
        private IInvitationService invitationService;

        public InvitationController()
        {
            this.invitationService = new InvitationService(this.Context);
        }

        public InvitationController(IInvitationService invitationService)
        {
            this.invitationService = invitationService;
        }

        public ActionResult Index()
        {
            ViewData["PendingCount"] = this.invitationService.GetPendingInvitationsCount();
            return View();
        }

        public JsonResult InvitationList()
        {            
            var invitations = this.invitationService.GetInvitations().ToArray();

            var model = invitations.Select(
                    i =>
                        new
                        {
                            Created = DateTimeHelper.ToLocalTime(i.CreationDateTime).ToShortDateString(),
                            UserName = i.User.Name,
                            UserId = i.User.UserId,
                            Email = i.Email,
                            Activated = i.ActivationDateTime.HasValue ? DateTimeHelper.ToLocalTime(i.ActivationDateTime.Value).ToShortDateString() : "Pending",
                            Expiration = DateTimeHelper.ToLocalTime(i.ExpirationDateTime).ToShortDateString(),
                        });

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pending()
        {
            return View();
        }

        public JsonResult PendingInvitationList()
        {
            var invitations = this.invitationService.GetPendingInvitations().ToArray();

            var model = invitations.Select(
                    i =>
                        new
                        {
                            Created = DateTimeHelper.ToLocalTime(i.CreationDateTime).ToShortDateString(),
                            UserName = i.User.Name,
                            UserId = i.User.UserId,
                            Email = i.Email,
                            Activated = i.ActivationDateTime.HasValue ? DateTimeHelper.ToLocalTime(i.ActivationDateTime.Value).ToShortDateString() : "Pending",
                            Expiration = DateTimeHelper.ToLocalTime(i.ExpirationDateTime).ToShortDateString(),
                        });

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
