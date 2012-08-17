namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Helpers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.WindowsAzure;

    [CustomAuthorize(Roles = "Administrator")]
    public class BlobController : ControllerBase
    {
        private IBlobService blobService;
        private IBlobSetService blobSetService;
        private IUserService userService;
        private IRoleService roleService;
        private IPermissionService permissionService;

        [System.Security.Permissions.EnvironmentPermission(System.Security.Permissions.SecurityAction.LinkDemand, Unrestricted = true)]
        public BlobController()
        {
            this.blobService = new BlobService(this.Context, CloudStorageAccount.Parse(ConfigReader.GetConfigValue("DataConnectionString")), ConfigReader.GetConfigValue("MainBlobContanier"));
            this.blobSetService = new BlobSetService(this.Context);
            this.roleService = new RoleService(this.Context);
            this.userService = new UserService(this.Context);
            this.permissionService = new PermissionService(this.Context);
        }

        public BlobController(IBlobService blobService, IBlobSetService blobSetService, IRoleService roleService, IUserService userService, IPermissionService permissionService)
        {
            this.blobService = blobService;
            this.blobSetService = blobSetService;
            this.roleService = roleService;
            this.userService = userService;
            this.permissionService = permissionService;
        }

        public ActionResult Index()
        {
            return this.View(this.blobService.GetBlobs().ToViewModel());
        }

        [HttpGet]
        public ActionResult Upload()
        {
            string message = null;

            if (Request.QueryString["FileTooLarge"] != null)
            {
                int maxRequestLength = 0;
                System.Web.Configuration.HttpRuntimeSection section =
                    (System.Web.Configuration.HttpRuntimeSection)
                    System.Web.Configuration.WebConfigurationManager.GetWebApplicationSection("system.web/httpRuntime");
                if (section != null)
                {
                    maxRequestLength = section.MaxRequestLength;
                }

                message = string.Format("File is too large. Use multiple file upload for files larger than {0}KB.", maxRequestLength);
            }

            var model = new BlobUploadModel
            {
                UploadHandler = this.GetUploadHandlerPage(),
                Message = message
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Upload(BlobUploadModel model)
        {
            model.ContentFile = Request.Files["ContentFile"];
            if (model.ContentFile == null || model.ContentFile.ContentLength == 0)
            {
                ModelState.AddModelError("ContentFile", "Please select the file to upload.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            string filename = Path.GetFileName(model.ContentFile.FileName);
            var blobName = string.IsNullOrEmpty(model.Name) ? Path.GetFileNameWithoutExtension(model.ContentFile.FileName) : model.Name;

            var blob = new Blob()
            {
                Name = blobName,
                Description = model.Description,
                OriginalFileName = filename,
                UploadDateTime = DateTime.UtcNow
            };

            Guid blobId = this.blobService.UploadBlob(blob, model.ContentFile.InputStream);

            return RedirectToAction("Details", new { id = blobId });
        }

        public ActionResult Details(Guid id)
        {
            var blob = this.blobService.GetBlobById(id);
            var model = blob.ToViewModel();

            model.BlobSets = blob.BlobSets.ToViewModel();
            model.Uri = this.blobService.GetBlob(blob).Uri.ToString();

            return this.View(model);
        }

        public ActionResult Permissions(Guid id)
        {
            var model = new BlobPermissionViewModel();
            model.Blob = this.blobService.GetBlobById(id);

            var currentUsers = model.Blob.Permissions.SelectMany(p => p.Users).Select(u => u.UserId);
            var currentRoles = model.Blob.Permissions.SelectMany(p => p.Roles).Select(r => r.RoleId);

            model.Users = this.userService.GetUsers()
                            .Where(u => !currentUsers.Any(cu => cu == u.UserId)).ToArray()
                            .Select(u => new SelectListItem() { Text = u.Name, Value = u.UserId.ToString() });

            model.Roles = this.roleService.GetRoles()
                            .Where(r => !currentRoles.Any(cr => cr == r.RoleId)).ToArray()
                            .Select(r => new SelectListItem() { Text = r.RoleName, Value = r.RoleId.ToString() });

            return this.View(model);
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var blob = this.blobService.GetBlobById(id);
            var model = new BlobUpdateDescriptionModel()
            {
                Id = blob.BlobId,
                Name = blob.Name,
                OriginalName = blob.Name,
                Description = blob.Description
            };

            return this.View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, BlobUpdateDescriptionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            this.blobService.UpdateDescription(id, model.Name, model.Description);
            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddBlobToSet(Guid id, string selectedblobsetname)
        {
            if (!string.IsNullOrEmpty(selectedblobsetname))
            {
                var blobSetNames = selectedblobsetname.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var blobSetName in blobSetNames)
                {
                    if (!string.IsNullOrWhiteSpace(blobSetName))
                    {
                        this.blobSetService.AddBlobToSet(blobSetName.Trim(), id);
                    }
                }
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddToSet(Guid id, Guid? selectedSet)
        {
            if (selectedSet != null && selectedSet.Value != Guid.Empty)
            {
                this.blobSetService.AddBlobToSet(selectedSet.Value, id);
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Delete(Guid id)
        {
            this.blobService.DeleteBlobById(id);

            return RedirectToAction("Index");
        }

        public ActionResult RemoveFromSet(Guid id, Guid selectedSet)
        {
            if (selectedSet != Guid.Empty)
            {
                this.blobSetService.RemoveBlobFromSet(selectedSet, id);
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult View(Guid id)
        {
            var blob = this.blobService.GetBlobById(id);

            if (blob == null)
            {
                return new RedirectResult("~/404.htm");
            }

            Response.ContentType = this.blobService.GetContentType(blob);
            var blob2 = this.blobService.GetBlob(blob);
            blob2.DownloadToStream(Response.OutputStream);

            return null;
        }

        public ActionResult Download(Guid id)
        {
            var blob = this.blobService.GetBlobById(id);

            if (blob == null)
            {
                return new RedirectResult("~/404.htm");
            }

            Response.ContentType = this.blobService.GetContentType(blob);
            Response.AddHeader("Content-disposition", "attachment; filename=" + blob.OriginalFileName);

            var blob2 = this.blobService.GetBlob(blob);
            blob2.DownloadToStream(Response.OutputStream);

            return null;
        }

        [HttpPost]
        public ActionResult GrantRolePermission(Guid id, Guid? selectedRole, int selectedExpiration)
        {
            if (selectedRole.HasValue && !selectedRole.Equals(Guid.Empty) && selectedExpiration > 0)
            {
                Microsoft.Samples.DPE.BlobShare.Data.Model.Role role = this.roleService.GetRoleById(selectedRole.Value);
                Blob blob = this.blobService.GetBlobById(id);
                DateTime expiration = this.GetExpiration(selectedExpiration);
                this.permissionService.GrantPermissionToRoleBlob(Privilege.Read, role, blob, expiration);
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
                Blob blob = this.blobService.GetBlobById(id);
                DateTime expiration = this.GetExpiration(selectedExpiration);
                this.permissionService.GrantPermissionToUserBlob(Privilege.Read, user, blob, expiration);
            }

            return RedirectToAction("Permissions", new { id = id });
        }

        public ActionResult RevokeUserPermission(Guid id, Guid selectedPermission)
        {
            this.permissionService.RevokePermission(selectedPermission);

            return RedirectToAction("Permissions", new { id = id });
        }

        private string GetUploadHandlerPage()
        {
            // Upload Handler Url
            Uri reqUrl = this.Request.Url;
            var uploadPage = new StringBuilder();
            uploadPage.Append(reqUrl.Scheme);
            uploadPage.Append("://");
            uploadPage.Append(this.Request.Headers["Host"] ?? reqUrl.Authority);
            uploadPage.Append(this.Request.ApplicationPath);
            if (!this.Request.ApplicationPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                uploadPage.Append("/");
            }

            uploadPage.Append("FileUploadHandler.ashx");

            return uploadPage.ToString();
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
    }
}