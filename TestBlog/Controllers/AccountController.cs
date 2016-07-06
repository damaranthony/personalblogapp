using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BlogData.Data;
using BlogData.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestBlog.Models;

namespace TestBlog.Controllers
{
    [Authorize]
    public class AccountController : Controller
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
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        #endregion

        #region ACTION METHODS
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                    return View(model);
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, LockoutEnabled = false };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                //create author record
                _unitOfWork.AuthorRepository.Insert(new Author
                {
                    UserId = user.Id,
                    Name = model.Name,
                    IsDeleted = false
                });
                _unitOfWork.Save();

                return RedirectToAction("Users", "Manage");
            }
            AddErrors(result);

            return View(model);
        }

        public async Task<ActionResult> Suspend(string id)
        {
            //check user id 
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Users", "Manage");
            //check if user exist
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Users", "Manage");
            //lock user account
            user.LockoutEnabled = true;
            await UserManager.UpdateAsync(user);
            TempData["User"] = "User account has been suspended!";

            return RedirectToAction("Users", "Manage");
        }

        public async Task<ActionResult> Activate(string id)
        {
            //check user id
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Users", "Manage");
            //check if user exist
            var user = await UserManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Users", "Manage");
            //remove user account log
            user.LockoutEnabled = false;
            await UserManager.UpdateAsync(user);
            TempData["User"] = "User account has been activated!";

            return RedirectToAction("Users", "Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Blog");
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Blog");
        }
        #endregion

        #region PROTECTED METHODS
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
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
        #endregion
    }
}