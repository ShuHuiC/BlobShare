namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Controllers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RoleControllerTest
    {
        [TestMethod]
        public void GetIndex()
        {
            RoleController controller = GetRoleController();

            ActionResult result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(IEnumerable<RoleViewModel>));
        }

        [TestMethod]
        public void GetCreate()
        {
            RoleController controller = GetRoleController();

            ActionResult result = controller.Create();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;

            Assert.IsNull(viewResult.ViewData.Model);
        }

        [TestMethod]
        public void PostCreate()
        {
            MockRoleService service = new MockRoleService();
            RoleController controller = GetRoleController(service);
            int nroles = service.Roles.Count;

            var model = new RoleEditViewModel()
            {
                Name = "New Role",
                Description = "New Role"
            };

            ActionResult result = controller.Create(model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);

            Assert.AreEqual(nroles + 1, service.Roles.Count);

            Assert.IsNotNull(service.NewRole);
            Assert.AreEqual("New Role", service.NewRole.RoleName);
        }

        [TestMethod]
        public void GetEdit()
        {
            MockRoleService service = new MockRoleService();
            RoleController controller = GetRoleController(service);
            Role role = service.Roles[0];

            ActionResult result = controller.Edit(role.RoleId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;

            Assert.IsNotNull(viewResult.ViewData.Model);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(RoleEditViewModel));

            var model = (RoleEditViewModel)viewResult.ViewData.Model;

            Assert.AreEqual(role.RoleName, model.Name);
            Assert.AreEqual(role.RoleId, model.Id);
        }

        [TestMethod]
        public void PostEdit()
        {
            MockRoleService service = new MockRoleService();
            RoleController controller = GetRoleController(service);

            Role role = service.Roles[0];

            RoleEditViewModel model = new RoleEditViewModel()
            {
                Id = role.RoleId,
                Name = "New " + role.RoleName
            };

            ActionResult result = controller.Edit(role.RoleId, model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(role.RoleId, redirectResult.RouteValues["id"]);

            Assert.AreEqual(role.RoleName, model.Name);
            Assert.AreEqual(role.RoleId, model.Id);
        }

        [TestMethod]
        public void GetDetails()
        {
            MockRoleService service = new MockRoleService();
            RoleController controller = GetRoleController(service);
            var id = service.Roles[0].RoleId;

            ActionResult result = controller.Details(id);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(RoleViewModel));
        }

        [TestMethod]
        public void AddToRole()
        {
            MockRoleService service = new MockRoleService();
            MockUserService userService = new MockUserService();
            RoleController controller = GetRoleController(service, userService);
            Role role = service.Roles[0];
            var id = role.RoleId;
            var userId = userService.Users[0].UserId;

            ActionResult result = controller.AddToRole(id, userId);

            Assert.AreEqual(1, role.Users.Count());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        [TestMethod]
        public void RemoveFromRole()
        {
            MockRoleService service = new MockRoleService();
            MockUserService userService = new MockUserService();
            RoleController controller = GetRoleController(service, userService);
            Role role = service.Roles[0];
            User user = userService.Users[0];

            service.AddUserToRole(role, user);
            var id = role.RoleId;
            var userId = userService.Users[0].UserId;

            ActionResult result = controller.RemoveFromRole(id, userId);

            Assert.AreEqual(0, role.Users.Count());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        private static RoleController GetRoleController()
        {
            return GetRoleController(new MockRoleService());
        }

        private static RoleController GetRoleController(IRoleService roleService)
        {
            return GetRoleController(roleService, new MockUserService());
        }

        private static RoleController GetRoleController(IRoleService roleService, IUserService userService)
        {
            return new RoleController(roleService, userService);
        }
    }
}
