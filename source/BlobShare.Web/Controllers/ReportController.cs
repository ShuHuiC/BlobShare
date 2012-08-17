namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.WindowsAzure;

    [CustomAuthorize(Roles = "Administrator")]
    public class ReportController : ControllerBase
    {
        private const int DefaultPageSize = 25;
        private IReportService reportService;
        private IUserService userService;
        private IBlobService blobService;
        private IBlobSetService blobSetService;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public ReportController()
        {
            this.reportService = new ReportService(this.Context);
            this.userService = new UserService(this.Context);
            this.blobService = new BlobService(this.Context, CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")), ConfigReader.GetConfigValue("MainBlobContanier"));
            this.blobSetService = new BlobSetService(this.Context);
        }

        public ReportController(IReportService reportService, IUserService userService, IBlobService blobService, IBlobSetService blobSetService)
        {
            this.reportService = reportService;
            this.userService = userService;
            this.blobService = blobService;
            this.blobSetService = blobSetService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserActivity()
        {
            var model = new UserActivityViewModel();

            DateTime now = DateTime.UtcNow;
            DateTime localnow = DateTimeHelper.ToLocalTime(now);

            model.FromDate = localnow.AddDays(-7);
            model.ToDate = localnow;

            model.LoginEvent = true;
            model.CreateUserEvent = true;
            model.ActivateUserEvent = true;
            model.DeactivateUserEvent = true;
            model.ViewSetEvent = true;
            model.ViewResourceEvent = true;
            model.DownloadResourceEvent = true;

            model.CurrentPage = 1;

            this.UserActivityGetEvents(model);

            return View(model);
        }

        [HttpPost]
        public ActionResult UserActivity(UserActivityViewModel model)
        {
            model.LoginEvent = Request.Form["LoginEvent"] != "false";
            model.CreateUserEvent = Request.Form["CreateUserEvent"] != "false";
            model.ActivateUserEvent = Request.Form["ActivateUserEvent"] != "false";
            model.DeactivateUserEvent = Request.Form["DeactivateUserEvent"] != "false";
            model.ViewSetEvent = Request.Form["ViewSetEvent"] != "false";
            model.ViewResourceEvent = Request.Form["ViewResourceEvent"] != "false";
            model.DownloadResourceEvent = Request.Form["DownloadResourceEvent"] != "false";

            this.UserActivityGetEvents(model);

            return View(model);
        }

        public ActionResult BlobActivity()
        {
            var model = new BlobActivityViewModel();

            DateTime now = DateTime.UtcNow;
            DateTime localnow = DateTimeHelper.ToLocalTime(now);

            model.FromDate = localnow.AddDays(-7);
            model.ToDate = localnow;

            model.ViewEvents = true;
            model.DownloadEvents = true;

            model.CurrentPage = 1;

            IList<int> eventtypes = new List<int>();
            eventtypes.Add((int)EventType.View);
            eventtypes.Add((int)EventType.Download);

            DateTime fromdate = DateTimeHelper.FromLocalTime(model.FromDate);
            DateTime todate = DateTimeHelper.FromLocalTime(NormalizeToDate(model.ToDate));

            var totalPages = 0;
            model.BlobEvents = this.reportService.GetBlobEvents(fromdate, todate, eventtypes, DefaultPageSize, model.CurrentPage, out totalPages);
            model.TotalPages = totalPages;

            return View(model);
        }

        [HttpPost]
        public ActionResult BlobActivity(BlobActivityViewModel model)
        {
            Blob blob = null;

            if (!string.IsNullOrEmpty(model.SelectedBlobName))
            {
                blob = this.blobService.GetBlobByName(model.SelectedBlobName);

                if (blob == null)
                {
                    this.ModelState.AddModelError("SelectedBlobName", "Unknown Blob");
                }
            }

            if (this.ModelState.IsValid)
            {
                DateTime fromdate = DateTimeHelper.FromLocalTime(model.FromDate);
                DateTime todate = DateTimeHelper.FromLocalTime(NormalizeToDate(model.ToDate));

                model.ViewEvents = Request.Form["ViewEvents"] != "false";
                model.DownloadEvents = Request.Form["DownloadEvents"] != "false";

                IList<int> eventtypes = new List<int>();

                if (model.ViewEvents)
                {
                    eventtypes.Add((int)EventType.View);
                }

                if (model.DownloadEvents)
                {
                    eventtypes.Add((int)EventType.Download);
                }

                var totalPages = 0;

                if (blob == null)
                {
                    model.BlobEvents = this.reportService.GetBlobEvents(fromdate, todate, eventtypes, DefaultPageSize, model.CurrentPage, out totalPages);
                }
                else
                {
                    model.BlobEvents = this.reportService.GetBlobEventsByBlob(fromdate, todate, eventtypes, blob.BlobId, DefaultPageSize, model.CurrentPage, out totalPages);
                }

                model.TotalPages = totalPages;
            }

            return View(model);
        }

        public ActionResult BlobSetActivity()
        {
            var model = new BlobSetActivityViewModel();

            DateTime now = DateTime.UtcNow;
            DateTime localnow = DateTimeHelper.ToLocalTime(now);

            model.FromDate = localnow.AddDays(-7);
            model.ToDate = localnow;

            model.CurrentPage = 1;

            DateTime fromdate = DateTimeHelper.FromLocalTime(model.FromDate);
            DateTime todate = DateTimeHelper.FromLocalTime(NormalizeToDate(model.ToDate));

            var totalPages = 0;
            model.BlobSetEvents = this.reportService.GetBlobSetEvents(fromdate, todate, EventType.View, DefaultPageSize, model.CurrentPage, out totalPages);
            model.TotalPages = totalPages;

            return View(model);
        }

        [HttpPost]
        public ActionResult BlobSetActivity(BlobSetActivityViewModel model)
        {
            DateTime fromdate = DateTimeHelper.FromLocalTime(model.FromDate);
            DateTime todate = DateTimeHelper.FromLocalTime(NormalizeToDate(model.ToDate));

            BlobSet set = null;

            if (!string.IsNullOrEmpty(model.SelectedBlobSetName))
            {
                set = this.blobSetService.GetBlobSetByName(model.SelectedBlobSetName);

                if (set == null)
                {
                    this.ModelState.AddModelError("SelectedBlobSetName", "Unknown Blob Set");
                }
            }

            if (this.ModelState.IsValid)
            {
                var totalPages = 0;              
                if (set == null)
                {
                    model.BlobSetEvents = this.reportService.GetBlobSetEvents(fromdate, todate, EventType.View, DefaultPageSize, model.CurrentPage, out totalPages);
                }
                else
                {
                    model.BlobSetEvents = this.reportService.GetBlobSetEventsByBlobSet(fromdate, todate, EventType.View, set.BlobSetId, DefaultPageSize, model.CurrentPage, out totalPages);
                }

                model.TotalPages = totalPages;
            }

            return View(model);
        }

        private static DateTime NormalizeToDate(DateTime datetime)
        {
            if (datetime.Hour == 0 && datetime.Minute == 0 && datetime.Second == 0)
            {
                return datetime.AddDays(1);
            }

            return datetime;
        }

        private void UserActivityGetEvents(UserActivityViewModel model)
        {
            IList<int> userEventTypes = new List<int>();

            if (model.LoginEvent)
            {
                userEventTypes.Add((int)UserEventType.Login);
            }

            if (model.CreateUserEvent)
            {
                userEventTypes.Add((int)UserEventType.Create);
            }

            if (model.ActivateUserEvent)
            {
                userEventTypes.Add((int)UserEventType.Activation);
            }

            if (model.DeactivateUserEvent)
            {
                userEventTypes.Add((int)UserEventType.Deactivation);
            }

            IList<int> setEventTypes = new List<int>();

            if (model.ViewSetEvent)
            {
                setEventTypes.Add((int)EventType.View);
            }

            IList<int> resourceEventTypes = new List<int>();

            if (model.ViewResourceEvent)
            {
                resourceEventTypes.Add((int)EventType.View);
            }

            if (model.DownloadResourceEvent)
            {
                resourceEventTypes.Add((int)EventType.Download);
            }

            User user = null;

            if (!string.IsNullOrEmpty(model.SelectedUserName))
            {
                user = this.userService.SearchUsers(model.SelectedUserName).FirstOrDefault();

                if (user == null)
                {
                    this.ModelState.AddModelError("SelectedUserName", "Unknown User");
                }
            }

            if (this.ModelState.IsValid)
            {
                DateTime fromdate = DateTimeHelper.FromLocalTime(model.FromDate);
                DateTime todate = DateTimeHelper.FromLocalTime(NormalizeToDate(model.ToDate));

                var totalPages = 0;
                if (user == null)
                {
                    model.GenericEvents = this.reportService.GetUserEvents(fromdate, todate, userEventTypes, setEventTypes, resourceEventTypes, DefaultPageSize, model.CurrentPage, out totalPages);
                }
                else
                {
                    model.GenericEvents = this.reportService.GetUserEventsByUser(fromdate, todate, userEventTypes, setEventTypes, resourceEventTypes, user.UserId, DefaultPageSize, model.CurrentPage, out totalPages);
                }

                model.TotalPages = totalPages;
            }
        }
    }
}