namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.WindowsAzure;

    [CustomAuthorize(Roles = "Administrator")]
    public class BlobSetController : ControllerBase
    {
        private IBlobSetService blobSetService;
        private IBlobService blobService;
        private IUserService userService;
        private IRoleService roleService;
        private IPermissionService permissionService;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public BlobSetController()
        {
            this.blobSetService = new BlobSetService(this.Context);
            this.blobService = new BlobService(this.Context, CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")), ConfigReader.GetConfigValue("MainBlobContanier"));
            this.roleService = new RoleService(this.Context);
            this.userService = new UserService(this.Context);
            this.permissionService = new PermissionService(this.Context);
        }

        public BlobSetController(IBlobSetService blobSetService, IBlobService blobService, IRoleService roleService, IUserService userService, IPermissionService permissionService)
        {
            this.blobSetService = blobSetService;
            this.blobService = blobService;
            this.roleService = roleService;
            this.userService = userService;
            this.permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return View(this.blobSetService.GetBlobSets().ToViewModel());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(BlobSetViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var blobSet = new BlobSet
            {
                Name = model.Name,
                Description = model.Description
            };

            this.blobSetService.CreateBlobSet(blobSet);

            if (model.AddPermissions)
            {
                return RedirectToAction("Permissions", new { id = blobSet.BlobSetId });
            }
            else
            {
                return RedirectToAction("Details", new { id = blobSet.BlobSetId });
            }
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var blobSet = this.blobSetService.GetBlobSetById(id);

            var model = blobSet.ToViewModel();
            model.OriginalName = blobSet.Name;

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, BlobSetViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var blob = this.blobSetService.GetBlobSetById(id);
            blob.Name = model.Name;
            blob.Description = model.Description;
            this.blobSetService.UpdateBlobSet(blob);

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        public ActionResult Notifications(Guid id)
        {
            var blobSet = this.blobSetService.GetBlobSetById(id);

            var model = new BlobSetNotificationViewModel();
            model.BlobSetId = blobSet.BlobSetId;
            model.Name = blobSet.Name;
            model.Subject = string.Format("Blob Share - Notifications - {0} blob set has changed", blobSet.Name);
            model.Message = string.Empty;

            return View(model);
        }

        [HttpPost]
        public ActionResult Notifications(Guid id, BlobSetNotificationViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var blobsetUri = this.GetBlobsetUri();
            var changesNotificationService = new ChangesNotificationService();
            changesNotificationService.NotifyBlobsetUsers(id, blobsetUri, model.Subject, model.Message, false);

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Delete(Guid id)
        {
            this.blobSetService.DeleteBlobSet(id);

            return RedirectToAction("Index");
        }

        public ActionResult Details(Guid id)
        {
            var blobSet = this.blobSetService.GetBlobSetById(id);

            var model = blobSet.ToViewModel();
            model.Blobs = blobSet.Blobs.ToViewModel();

            return View(model);
        }

        public ActionResult Permissions(Guid id)
        {
            var model = new BlobSetPermissionViewModel();
            model.BlobSet = this.blobSetService.GetBlobSetById(id);

            var currentUsers = model.BlobSet.Permissions.SelectMany(p => p.Users).Select(u => u.UserId);
            var currentRoles = model.BlobSet.Permissions.SelectMany(p => p.Roles).Select(p => p.RoleId);

            model.Users = this.userService.GetUsers()
                            .Where(u => !currentUsers.Any(cu => cu == u.UserId)).ToArray()
                            .Select(u => new SelectListItem() { Text = u.Name, Value = u.UserId.ToString() });

            model.Roles = this.roleService.GetRoles()
                            .Where(r => !currentRoles.Any(cr => cr == r.RoleId)).ToArray()
                            .Select(r => new SelectListItem() { Text = r.RoleName, Value = r.RoleId.ToString() });

            return View(model);
        }

        [HttpPost]
        public ActionResult AddToSet(Guid id, Guid? selectedBlob)
        {
            if (selectedBlob != null && selectedBlob.Value != Guid.Empty)
            {
                this.blobSetService.AddBlobToSet(id, selectedBlob.Value);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddBlobToSet(Guid id, string selectedBlobname)
        {
            if (!string.IsNullOrEmpty(selectedBlobname))
            {
                var blobNames = selectedBlobname.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var blobName in blobNames)
                {
                    if (!string.IsNullOrWhiteSpace(blobName))
                    {
                        this.blobSetService.AddBlobToSet(id, blobName.Trim());
                    }
                }
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult RemoveFromSet(Guid id, Guid selectedBlob)
        {
            if (selectedBlob != null && selectedBlob != Guid.Empty)
            {
                this.blobSetService.RemoveBlobFromSet(id, selectedBlob);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult GrantRolePermission(Guid id, Guid? selectedRole, int selectedExpiration)
        {
            if (selectedRole.HasValue && !selectedRole.Equals(Guid.Empty) && selectedExpiration > 0)
            {
                Microsoft.Samples.DPE.BlobShare.Data.Model.Role role = this.roleService.GetRoleById(selectedRole.Value);
                BlobSet set = this.blobSetService.GetBlobSetById(id);
                DateTime expiration = this.GetExpiration(selectedExpiration);
                
                this.permissionService.GrantPermissionToRoleBlobSet(Privilege.Read, role, set, expiration);
            }

            return RedirectToAction("Permissions", new { id = id });
        }

        public ActionResult RevokeRolePermission(Guid id, Guid selectedPermission)
        {
            this.permissionService.RevokePermission(selectedPermission);

            return RedirectToAction("Permissions", new { id = id });
        }

        [HttpPost]
        public ActionResult GrantUserPermission(Guid id, Guid? selectedUser, int selectedExpiration)
        {
            if (selectedUser.HasValue && !selectedUser.Equals(Guid.Empty) && selectedExpiration > 0)
            {
                User user = this.userService.RetrieveUserById(selectedUser.Value);
                BlobSet set = this.blobSetService.GetBlobSetById(id);
                DateTime expiration = this.GetExpiration(selectedExpiration);
                
                this.permissionService.GrantPermissionToUserBlobSet(Privilege.Read, user, set, expiration);
            }

            return RedirectToAction("Permissions", new { id = id });
        }

        public ActionResult RevokeUserPermission(Guid id, Guid selectedPermission)
        {
            this.permissionService.RevokePermission(selectedPermission);

            return RedirectToAction("Permissions", new { id = id });
        }

        public ActionResult BlobSearch(string term, int maxResults)
        {
            var data = this.blobService.GetBlobsByPartialName(term).Select(br => br.Name).ToArray();
            var result = new JsonResult();
            result.Data = data.Take(maxResults);
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        public ActionResult ResourceSearch(string term, int maxResults)
        {
            var data = this.blobSetService.GetBlobSetsByPartialName(term).Select(br => br.Name).ToArray();
            var result = new JsonResult();
            result.Data = data.Take(maxResults); 
            result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return result;
        }

        private DateTime GetExpiration(int selectedExpiration)
        {
            var expiration = DateTime.UtcNow;

            if ((selectedExpiration % 30) == 0)
            {
                expiration = expiration.AddMonths(selectedExpiration / 30);
            }
            else
            {
                expiration = expiration.AddDays(selectedExpiration);
            }

            return expiration;
        }

        private string GetBlobsetUri()
        {
            string url = this.Request.UrlReferrer.AbsoluteUri;
            string newurl = url.Replace("BlobSet", "MyBlobs");
            newurl = newurl.Replace("Notifications", "ViewBlobSet");
            return newurl;
        }
    }
}