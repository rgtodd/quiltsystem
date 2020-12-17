//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Locale;
using RichTodd.QuiltSystem.Web.Models.Login;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    //
    // Control flows:
    //
    // * Login
    // * Login (Post)
    //
    // * Register / Register
    // * Register (Post) / RegisterConfirmation
    // * ConfirmEmail
    //
    // * ForgotPassword / ForgotPassword
    // * ForgotPassword (Post) / Redirect
    // * ForgotPasswordConfirmation
    // * ResetPassword / ResetPassword
    // * ResetPassword (Post) / Redirect
    // * ResetPasswordConfirmation
    //
    // * Logoff / Redirect
    //
    public class LoginController : ApplicationController<LoginModelFactory>
    {
        private SignInManager<IdentityUser> SignInManager { get; }
        private IUserMicroService UserMicroService { get; }
        private IUserManagementMicroService UserManagementMicroService { get; }

        public LoginController(
            SignInManager<IdentityUser> signInManager,
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IUserMicroService userMicroService,
            IUserManagementMicroService userManagementMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            SignInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
            UserManagementMicroService = userManagementMicroService ?? throw new ArgumentNullException(nameof(userManagementMicroService));
        }

        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                await UserManagementMicroService.ConfirmEmailAsync(userId, code);

                ViewBag.Message = "Thank you for confirming your email.";

                return View();
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(LoginForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await UserMicroService.GetUserByNameAsync(model.Email);
                if (user == null)
                {
                    throw new ServiceException("Invalid user ID or password.");
                }

                var userId = user.UserId;
                var isConfirmed = await UserMicroService.GetEmailConfirmedAsync(userId);
                if (isConfirmed)
                {
                    await SendEmailResetAsync(userId);
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Login");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var user = await UserMicroService.GetUserByNameAsync(model.Email);
                if (user == null)
                {
                    throw new ServiceException("Invalid user ID or password.");
                }

                var userId = user.UserId;
                var confirmed = await UserMicroService.GetEmailConfirmedAsync(userId);
                if (!confirmed)
                {
                    await SendConfirmationEmailAsync(userId);

                    AddModelError("Your email address has not been confirmed.  A confirmation email has been sent.");
                    return View();
                }

                var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.IsPersistent, lockoutOnFailure: true);
                if (result.IsLockedOut)
                {
                    AddModelError("This account has been locked out.  Please reset your password to recover.");
                    return View();
                }
                else if (result.RequiresTwoFactor)
                {
                    // Used by two-factor authentication.
                    return View();
                }
                else if (!result.Succeeded)
                {
                    AddModelError("Invalid user ID or password.");
                    return View();
                }

                UserLocale.RemoveFrom(HttpContext);

                return RedirectToLocal(returnUrl);
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            await SignInManager.SignOutAsync();

            UserLocale.RemoveFrom(HttpContext);

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(LoginRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var request = new MUser_CreateUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Password = model.Password
                };

                var userId = await UserManagementMicroService.CreateNewUserAsync(request);

                await SendConfirmationEmailAsync(userId);

                return View("RegisterConfirmation");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string userId, string code)
        {
            try
            {
                var email = await UserMicroService.GetUserEmailAsync(userId);

                var model = new LoginResetPasswordViewModel()
                {
                    UserId = userId,
                    Code = code,
                    Email = email
                };

                return View(model);
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(LoginResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await UserManagementMicroService.ResetPasswordAsync(model.UserId, model.Password, model.Code);

                return RedirectToAction("ResetPasswordConfirmation", "Login");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : (ActionResult)RedirectToAction("Index", "Home");
        }

        private async Task SendConfirmationEmailAsync(string userId)
        {
            var code = await UserManagementMicroService.GenerateEmailConfirmationTokenAsync(userId);

            var callbackUrl = Url.Action("ConfirmEmail", "Login", values: new { userId, code }, protocol: Request.Scheme);

            await UserMicroService.SendConfirmationEmailAsync(userId, callbackUrl);
        }

        private async Task SendEmailResetAsync(string userId)
        {
            var code = await UserManagementMicroService.GeneratePasswordResetTokenAsync(userId);

            var callbackUrl = Url.Action("ResetPassword", "Login", values: new { userId, code }, protocol: Request.Scheme);

            await UserMicroService.SendResetEmailAsync(userId, callbackUrl);
        }

    }
}