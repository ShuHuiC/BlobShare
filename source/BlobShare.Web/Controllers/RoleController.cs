namespace Microsoft.Samples.DPE.BlobShare.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;

    [CustomAuthorize(Roles = "Administrator")]
    public class RoleController : ControllerBase
    {
        private IRoleService roleService;
        private IUserService userService;

        public RoleController()
        {
            this.roleService = new RoleService(this.Context);
            this.userService = new UserService(this.Context);
        }

        public RoleController(IRoleService roleService, IUserService userService)
        {
            this.roleService = roleService;
            this.userService = userService;
        }

        public ActionResult Index()
        {
            var model = this.roleService.GetRoles().Select(r => new RoleViewModel { Role = r });
            return View(model);
        }

        public ActionResult Details(Guid id)
        {
            Role role = this.roleService.GetRoleById(id);
            var model = new RoleViewModel() { Role = role };

            List<User> users = this.userService.GetUsers().ToList();
            users.RemoveAll(u => model.Role.Users.Where(ru => ru.UserId == u.UserId).FirstOrDefault() != null);

            var items = new List<SelectListItem>();
            foreach (var user in users)
            {
                items.Add(new SelectListItem() { Text = user.Name, Value = user.UserId.ToString() });
            }

            model.Users = items;

            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(RoleEditViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var role = new Role()
            {
                RoleName = model.Name,
                Description = model.Description
            };

            try
            {
                this.roleService.CreateRole(role);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }

            return RedirectToAction("Details", new { id = role.RoleId });
        }

        [HttpGet]
        public ActionResult Edit(Guid id)
        {
            var role = this.roleService.GetRoleById(id);
            var model = new RoleEditViewModel()
            {
                Id = role.RoleId,
                Name = role.RoleName,
                OriginalName = role.RoleName,
                Description = role.Description
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(Guid id, RoleEditViewModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var role = this.roleService.GetRoleById(id);
            role.RoleName = model.Name;
            role.Description = model.Description;

            try
            {
                this.roleService.UpdateRole(role);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError("Name", ex.Message);
                return View(model);
            }

            return RedirectToAction("Details", new { id = id });
        }

        [HttpPost]
        public ActionResult AddToRole(Guid id, Guid? selectedUser)
        {
            if (selectedUser != null && selectedUser.Value != Guid.Empty)
            {
                User user = this.userService.RetrieveUserById(selectedUser.Value);
                Role role = this.roleService.GetRoleById(id);

                this.roleService.AddUserToRole(role, user);
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult RemoveFromRole(Guid id, Guid? selectedUser)
        {
            if (selectedUser != null && selectedUser.Value != Guid.Empty)
            {
                User user = this.userService.RetrieveUserById(selectedUser.Value);
                Role role = this.roleService.GetRoleById(id);

                this.roleService.RemoveUserFromRole(role, user);
            }

            return RedirectToAction("Details", new { id = id });
        }
    }
}