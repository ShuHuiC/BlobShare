namespace Microsoft.Samples.DPE.BlobShare.Web.Tests.Controllers
{
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.DPE.BlobShare.Core.Services;
    using Microsoft.Samples.DPE.BlobShare.Data.Model;
    using Microsoft.Samples.DPE.BlobShare.Web.Controllers;
    using Microsoft.Samples.DPE.BlobShare.Web.Models;
    using Microsoft.Samples.DPE.BlobShare.Web.Tests.Mocks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UserControllerTest
    {
        [TestMethod]
        public void GetIndex()
        {
            UserController controller = GetUserController();

            ActionResult result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsNull(viewResult.ViewData.Model);
        }

        [TestMethod]
        public void GetDetails()
        {
            MockUserService service = new MockUserService();
            UserController controller = GetUserController(service);
            var id = service.GetUsers().First().UserId;

            ActionResult result = controller.Details(id);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(UserViewModel));
        }

        [TestMethod]
        public void ActivateUser()
        {
            MockUserService service = new MockUserService();
            UserController controller = GetUserController(service);
            User user = service.Users[0];
            user.Inactive = true;
            var id = user.UserId;

            ActionResult result = controller.Activate(id);

            Assert.IsFalse(user.Inactive);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        [TestMethod]
        public void DeactivateUser()
        {
            MockUserService service = new MockUserService();
            UserController controller = GetUserController(service);
            User user = service.Users[0];
            user.Inactive = false;
            var id = user.UserId;

            ActionResult result = controller.Deactivate(id);

            Assert.IsTrue(user.Inactive);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        [TestMethod]
        public void AddToRole()
        {
            MockUserService service = new MockUserService();
            MockRoleService roleService = new MockRoleService();
            UserController controller = GetUserController(service, roleService);
            User user = service.Users[0];
            var id = user.UserId;          
            var roleId = roleService.GetRoles().First().RoleId;

            ActionResult result = controller.AddToRole(id, roleId);

            Assert.AreEqual(1, user.Roles.Count());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        [TestMethod]
        public void RemoveFromRole()
        {
            MockUserService service = new MockUserService();
            MockRoleService roleService = new MockRoleService();
            UserController controller = GetUserController(service, roleService);
            User user = service.Users[0];
            var id = user.UserId;
            var role = roleService.Roles[0];
            var roleId = role.RoleId;

            roleService.AddUserToRole(role, user);

            ActionResult result = controller.RemoveFromRole(id, roleId);

            Assert.AreEqual(0, user.Roles.Count());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(id, redirectResult.RouteValues["id"]);
        }

        [TestMethod]
        public void GetCreate()
        {
            UserController controller = GetUserController();

            ActionResult result = controller.Create();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;

            Assert.IsNotNull(viewResult.ViewData.Model);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(UserViewModel));
        }

        [TestMethod]
        public void GetBulkCreate()
        {
            UserController controller = GetUserController();

            ActionResult result = controller.BulkCreate();

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;

            Assert.IsNotNull(viewResult.ViewData.Model);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(BulkUserViewModel));
        }

        [TestMethod]
        public void GetEdit()
        {
            MockUserService service = new MockUserService();
            UserController controller = GetUserController(service);
            User user = service.Users[0];

            ActionResult result = controller.Edit(user.UserId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)result;

            Assert.IsNotNull(viewResult.ViewData.Model);
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(UserViewModel));

            var model = (UserViewModel)viewResult.ViewData.Model;

            Assert.AreEqual(user.Name, model.Name);
            Assert.AreEqual(user.Email, model.Email);
            Assert.AreEqual(user.UserId, model.Id);
        }

        [TestMethod]
        public void PostEdit()
        {
            MockUserService service = new MockUserService();
            UserController controller = GetUserController(service);
            User user = service.Users[0];
            var model = new UserViewModel()
            {
                Id = user.UserId,
                Name = "New " + user.Name,
                Email = "new" + user.Email
            };

            ActionResult result = controller.Edit(user.UserId, model);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            RedirectToRouteResult redirectResult = (RedirectToRouteResult)result;

            Assert.AreEqual("Details", redirectResult.RouteValues["action"]);
            Assert.AreEqual(user.UserId, redirectResult.RouteValues["id"]);

            Assert.IsNotNull(service.UpdatedUser);

            Assert.AreEqual(service.UpdatedUser.Name, model.Name);
            Assert.AreEqual(service.UpdatedUser.Email, model.Email);
        }

        private static UserController GetUserController()
        {
            return GetUserController(new MockUserService());
        }

        private static UserController GetUserController(IUserService userService)
        {
            return GetUserController(userService, new MockRoleService());
        }

        private static UserController GetUserController(IUserService userService, IRoleService roleService)
        {
            return new UserController(userService, roleService, null);
        }
    }
}
