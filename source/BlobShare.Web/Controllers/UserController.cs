namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;

    [CustomAuthorize(Roles = "Administrator")]
    public class UserController : ControllerBase
    {
        private IUserService userService;
        private IRoleService roleService;
        private IInvitationService invitationService;

        public UserController()
        {
            this.userService = new UserService(this.Context);
            this.roleService = new RoleService(this.Context);
            this.invitationService = new InvitationService(this.Context);
        }

        public UserController(IUserService userService, IRoleService roleService, IInvitationService invitationService)
        {
            this.userService = userService;
            this.roleService = roleService;
            this.invitationService = invitationService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult UserList()
        {
            var model = this.userService.GetUsers().Select(
                    u =>
                        new
                        {
                            Name = u.Name,
                            UserId = u.UserId,
                            Email = u.Email,
                            Status = u.Inactive ? "Inactive" : "Active",
                            Roles = u.Roles.OrderBy(r => r.RoleName).Select(o => o.RoleName),
                        });

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);
            var model = new UserViewModel()
            {
                User = user
            };

            List<Role> roles = this.roleService.GetRoles().ToList();
            roles.RemoveAll(r => model.User.Roles.FirstOrDefault(ur => ur.RoleId == r.RoleId) != null);

            var items = new List<SelectListItem>();
            foreach (var role in roles)
            {
                items.Add(new SelectListItem() { Text = role.RoleName, Value = role.RoleId.ToString() });
            }

            model.Roles = items;

            model.IsMe = this.userService.IsMe(user);

            return View(model);
        }

        public ActionResult Invite(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);

            var model = new UserInviteViewModel()
            {
                User = user
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Invite(Guid id, UserInviteViewModel model)
        {
            User user = this.userService.RetrieveUserById(id);

            var invitationLink = this.GetInvitationPage();
            var expiration = DateTime.UtcNow.AddDays(30);

            try
            {
                this.invitationService.InviteUser(user, invitationLink, expiration, model.PersonalMessage, false);
            }
            catch (System.Net.Mail.SmtpException e)
            {
                model.User = user;
                ModelState.AddModelError("SmtpError", e);

                return View("Invite", model);
            }

            return RedirectToAction("Details", new { id = user.UserId });
        }

        public ActionResult Create()
        {
            var model = new UserViewModel();
            model.RoleNames = this.roleService.GetRoles().OrderBy(r => r.RoleName).Select(r => r.RoleName);

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(UserViewModel model, FormCollection form)
        {
            if (!this.ModelState.IsValid)
            {
                model.RoleNames = this.roleService.GetRoles().OrderBy(r => r.RoleName).Select(r => r.RoleName);
                return View(model);
            }

            var roles = this.roleService.GetRoles().OrderBy(r => r.RoleName).ToList();
            User user = new User() { Name = model.Name, Email = model.Email };

            try
            {
                this.userService.CreateUser(user);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("CustomErrors", ex.Message);
                model.RoleNames = this.roleService.GetRoles().OrderBy(r => r.RoleName).Select(r => r.RoleName);
                return View(model);
            }

            int k = 1;

            foreach (var role in roles)
            {
                if (!string.IsNullOrEmpty(form["Role" + k]) && form["Role" + k].Contains("true"))
                {
                    this.roleService.AddUserToRole(role, user);
                }

                k++;
            }

            var invitationLink = this.GetInvitationPage();
            var expiration = DateTime.UtcNow.AddDays(30);

            this.invitationService.InviteUser(user, invitationLink, expiration, model.PersonalMessage, false);

            return RedirectToAction("Details", new { id = user.UserId });
        }

        public ActionResult BulkCreate()
        {
            var model = new BulkUserViewModel();
            model.RoleNames = this.roleService.GetRoles().OrderBy(r => r.RoleName).Select(r => r.RoleName);

            return View(model);
        }

        [HttpPost]
        public ActionResult BulkCreate(BulkUserViewModel model, FormCollection form)
        {
            if (!this.ModelState.IsValid)
            {
                model.RoleNames = this.roleService.GetRoles().OrderBy(r => r.RoleName).Select(r => r.RoleName);
                return View(model);
            }

            var roles = this.roleService.GetRoles().OrderBy(r => r.RoleName).ToList();

            var emails = model.Emails.Replace(';', ',').Split(',').Select(e => e.Trim()).Distinct();            

            foreach (var email in emails)
            {
                User user = this.userService.RetrieveUserByEMail(email);

                if (user == null)
                {
                    user = new User() { Name = this.GetNameFromEmail(email), Email = email };

                    this.userService.CreateUser(user);

                    int k = 1;

                    foreach (var role in roles)
                    {
                        if (!string.IsNullOrEmpty(form["Role" + k]) && form["Role" + k].Contains("true"))
                        {
                            this.roleService.AddUserToRole(role, user);
                        }

                        k++;
                    }
                }

                var invitationLink = this.GetInvitationPage();
                var expiration = DateTime.UtcNow.AddDays(30);
                this.invitationService.InviteUser(user, invitationLink, expiration, model.PersonalMessage, false);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);

            var model = new UserViewModel()
            {
                Id = user.UserId,
                Name = user.Name,
                OriginalName = user.Name,
                Email = user.Email,
                Status = user.Inactive ? 1 : 0
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, UserViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            User user = new User
            {
                UserId = id,
                Name = model.Name,
                Email = model.Email,
                Inactive = model.Status == 0 ? false : true
            };

            try
            {
                this.userService.UpdateUser(user);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("Email", ex.Message);
                return View(model);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddToRole(Guid id, Guid? selectedRole)
        {
            if (selectedRole != null && selectedRole.Value != Guid.Empty)
            {
                User user = this.userService.RetrieveUserById(id);
                Role role = this.roleService.GetRoleById(selectedRole.Value);

                this.roleService.AddUserToRole(role, user);
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult RemoveFromRole(Guid id, Guid? selectedRole)
        {
            if (selectedRole != null && selectedRole.Value != Guid.Empty)
            {
                User user = this.userService.RetrieveUserById(id);
                Role role = this.roleService.GetRoleById(selectedRole.Value);

                this.roleService.RemoveUserFromRole(role, user);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult Activate(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);
            this.userService.ActivateUser(user);

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult Deactivate(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);
            this.userService.DeactivateUser(user);

            return RedirectToAction("Details", new { id = id });
        }

        public JsonResult UserSearch(string term)
        {
            var data = this.userService.SearchUsers(term).Select(u => u.Name).ToArray();

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [HandleError]
        public ActionResult Delete(Guid id)
        {
            User user = this.userService.RetrieveUserById(id);
            this.userService.DeleteUser(user);

            return RedirectToAction("Index");
        }

        private string GetInvitationPage()
        {
            Uri reqUrl = this.Request.Url;
            var invitationPage = new StringBuilder();
            invitationPage.Append(reqUrl.Scheme);
            invitationPage.Append("://");
            invitationPage.Append(this.Request.Headers["Host"] ?? reqUrl.Authority);
            invitationPage.Append(ConfigurationManager.AppSettings["UserAccountInvitationAction"]);

            return invitationPage.ToString();
        }

        private string GetNameFromEmail(string email)
        {
            var pos = email.IndexOf("@");
            var name = email.Substring(0, pos);
            name = Regex.Replace(name, @"[_\-\.0-9]", " ");
            name = Regex.Replace(name, "([A-Z]{1,2})", " $1").TrimStart();
            name = Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name);

            return name;
        }
    }
}