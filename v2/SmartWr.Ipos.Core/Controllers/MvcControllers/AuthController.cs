using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Nop.Core.Caching;
using SmartWr.Ipos.Core.Messaging;
using SmartWr.Ipos.Core.Settings;
using SmartWr.Ipos.Core.ViewModels;
using SmartWr.WebFramework.Library.Infrastructure.Identity.Manager;
using SmartWr.WebFramework.Library.Infrastructure.IoCs;
using SmartWr.WebFramework.Library.MiddleServices.Interfaces.Auth;
using SmartWr.WebFramework.Library.MiddleServices.Models.Auth;

namespace SmartWr.Ipos.Core.Controllers.MvcControllers
{
    public class AuthController : BaseMvcController
    {
        private readonly ApplicationUserManager _userMgr;
        private readonly IApplicationRoleManager _roleMgr;
        private readonly string rolesKey = "ipos_roles";
        private ICacheManager _cacheManager;

        public AuthController(IApplicationUserManager userMgr, IApplicationRoleManager roleMgr)
        {
            _userMgr = (ApplicationUserManager)userMgr;
            _userMgr.UserManager.EmailService = new EmailService();
            _roleMgr = roleMgr;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult RegisterUser()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> RegisterUser(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    Email = vm.Email,
                    UserName = vm.Username
                };
                var register = _userMgr.Create(user);
                var addPass = await _userMgr.AddPasswordAsync(user.Id, vm.Password).ConfigureAwait(false);
                if (addPass.Succeeded)
                {
                    var result = await _userMgr.PasswordSignInAsync(vm.Username, vm.Password, true, false);
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {

            return View();

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(LoginViewModel Vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userMgr.FindByNameAsync(Vm.Username);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email was not found.");
                    return Redirect("RegisterUser");
                }
                else
                {

                    string code = await _userMgr.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Auth", new { Email = user.UserName, code = code }, protocol: Request.Url.Scheme);

                    await _userMgr.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");


                    return RedirectToAction("ForgotPasswordConfirmation", "Auth");
                }

            }
            ViewBag.ModelIsValid = false;
            return View();
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public ActionResult SignOut()
        {
            _userMgr.SignOut(new String[] { DefaultAuthenticationTypes.ApplicationCookie });
            return Redirect("~/auth/login");
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([Required]String userName, [Required]String password)
        {
            //var status = await _userMgr.PasswordSignInAsync(userName, password, true, true);


            //if (status == SignInStatus.Success)
            //{
            //    ClaimsIdentity claims = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            //    var claimPrincipal = User.Identity as ClaimsPrincipal;
            //    claims.AddClaims(claimPrincipal.Claims);
            //    return RedirectToAction("Index", "Home");
            //}

            //var appUser = await _userMgr.FindAsync(userName, password);

            //if (appUser != null)
            //{
            //    ClaimsIdentity claims = new ClaimsIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            //    claims.AddClaim(new Claim(ClaimTypes.Name, appUser.UserName));
            //    claims.AddClaim(new Claim(ClaimTypes.Email, appUser.Email));

            //    var roles = _userMgr.GetRoles(appUser.Id);
            //    claims.AddClaim(new Claim(ClaimTypes.Role, String.Join(",", roles)));
            //    return Redirect("~/");
            //}

            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        private async Task<string> SendEmailConfirmationTokenAsync(Int32 userID, string subject)
        {
            string code = await _userMgr.GenerateEmailConfirmationTokenAsync(userID);
            var callbackUrl = Url.Action("ResetPassword", "Auth",
               new { userId = userID, code = code }, protocol: Request.Url.Scheme);
            await _userMgr.SendEmailAsync(userID, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

            return callbackUrl;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string code, string Email)
        {

            if (code == null)
            {
                ModelState.AddModelError("", "");
            }

            ViewBag.code = code;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(string Email, string code, string newPassword)
        {

            if (ModelState.IsValid)
            {

                var user = await _userMgr.FindByNameAsync(Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "The user could not be found");
                }


                var result = await _userMgr.ResetPasswordAsync(user.Id, code, newPassword);

                if (result.Succeeded)
                {

                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                }

            }

            return View();
        }

        [HttpGet]
        public ActionResult Main()
        {
            return View("~/views/auth/usermanagementshell.cshtml");
        }

        public ActionResult UserList()
        {
            if (Request.IsAjaxRequest())
                return View();
            return HttpNotFound();
        }

        public ActionResult UserAccountEdit()
        {
            if (Request.IsAjaxRequest())
            {
                _cacheManager = EngineContext.Current.Resolve<ICacheManager>();

                var roles = _cacheManager.Get<IEnumerable<ApplicationRole>>(rolesKey);

                if (roles == null)
                {
                    roles = _roleMgr.GetRoles();
                    _cacheManager.Set(rolesKey, roles, AppKeys.DefaultCacheTime);
                }

                ViewBag.roles = new SelectList(roles, "Id", "Name");
                return View();
            }
            return HttpNotFound();
        }

        public ActionResult UserAccountView() 
        {
            if (Request.IsAjaxRequest())
            {
                _cacheManager = EngineContext.Current.Resolve<ICacheManager>();

                var roles = _cacheManager.Get<IEnumerable<ApplicationRole>>(rolesKey);

                if (roles == null)
                {
                    roles = _roleMgr.GetRoles();
                    _cacheManager.Set(rolesKey, roles, AppKeys.DefaultCacheTime);
                }

                ViewBag.roles = new SelectList(roles, "Id", "Name");
                return View();
            }
            return HttpNotFound();
        
        }

        public ActionResult CreateUserAccount()
        {
            if (!Request.IsAjaxRequest()) return HttpNotFound();
            _cacheManager = EngineContext.Current.Resolve<ICacheManager>();

            var roles = _cacheManager.Get<IEnumerable<ApplicationRole>>(rolesKey);

            if (roles == null)
            {
                roles = _roleMgr.GetRoles();
                _cacheManager.Set(rolesKey, roles, AppKeys.DefaultCacheTime);
            }

            ViewBag.roles = new SelectList(roles, "Id", "Name");
            return View();
        }


        public ActionResult UserPasswordChange() 
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> CheckExistingUserName(string username)
        {
            return Json(await _userMgr.FindByNameAsync(username) == null, JsonRequestBehavior.AllowGet);
        }
    }
}