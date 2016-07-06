using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BlogData.Data;
using BlogData.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestBlog.Models;

namespace TestBlog.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        #region PRIVATE VARIABLES
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        #endregion

        #region PUBLIC PROPERTIES
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        #region INIT
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region ACTION METHODS
        public ActionResult Users()
        {
            return View(UserManager.Users);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles()
        {
            var context = new ApplicationDbContext();
            var roles = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            return View(roles.Roles);
        }

        [Route("role/{roleid}")]
        public ActionResult Permissions(string roleid)
        {
            //check if role id is provided
            if (string.IsNullOrWhiteSpace(roleid)) return View();

            var context = new ApplicationDbContext();
            var roleObj = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)).FindByIdAsync(roleid);
            //get users in current role
            ViewBag.UsersInRole = UserManager.Users.ToList().Where(u => u.Roles.Select(ur => ur.RoleId).Contains(roleObj.Result.Id));
            //get users not in the role
            ViewBag.UserList = UserManager.Users.ToList().Where(u => !u.Roles.Select(ur => ur.RoleId).Contains(roleObj.Result.Id));
            ViewBag.RoleId = roleid;
            //get all permission by role id
            return string.IsNullOrEmpty(roleid) ? View(new List<ContentStateToRole>()) : View(_unitOfWork.ContentStateToRoleRepository.GetContentStateRolesByRoleId(roleid));
        }

        [Route("id/{id}/role/{roleid}")]
        public ActionResult AssignRole(string id, string roleid)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(roleid)) return RedirectToAction("Permissions", new { roleid });
            //check if user exist
            var uObj = UserManager.Users.FirstOrDefault(u => u.Id == id);
            if (uObj == null) return RedirectToAction("Permissions");
            //get role object
            var context = new ApplicationDbContext();
            var roleObj = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)).FindByIdAsync(roleid);

            if (roleObj.Result.Users.All(r => r.UserId == id)) return RedirectToAction("Permissions", new { roleid });
            //assign user to role
            UserManager.AddToRole(id, roleObj.Result.Name);
            TempData["UserRoleAssign"] = "User has been successfully assigned to role!";

            return RedirectToAction("Permissions", new { roleid });
        }

        [Route("uid/{uid}/role/{roleid}")]
        public ActionResult RemoveFromRole(string uid, string roleid)
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(roleid)) return RedirectToAction("Permissions", new { roleid });
            //check if user exist
            var uObj = UserManager.Users.FirstOrDefault(u => u.Id == uid);
            if (uObj == null) return RedirectToAction("Permissions");
            //get role object
            var context = new ApplicationDbContext();
            var roleObj = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context)).FindByIdAsync(roleid);

            if (roleObj.Result.Users.All(r => r.UserId == uid)) return RedirectToAction("Permissions", new { roleid });
            //remove user to role
            UserManager.RemoveFromRole(uid, roleObj.Result.Name);
            TempData["UserRoleAssign"] = "User has been removed from the role!";

            return RedirectToAction("Permissions", new { roleid });
        }
        #endregion
        
        #region PROTECTED METHODS
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }
        #endregion

        #region HELPERS
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }
        #endregion
    }
}