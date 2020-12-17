//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;
using RichTodd.QuiltSystem.Web.Locale;
using RichTodd.QuiltSystem.Web.Models.Profile;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class ProfileController : ApplicationController<ProfileModelFactory>
    {
        private IUserMicroService UserMicroService { get; }
        private IUserManagementMicroService UserManagementMicroService { get; }

        public ProfileController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IUserMicroService userMicroService,
            IUserManagementMicroService userManagementMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            UserMicroService = userMicroService ?? throw new ArgumentNullException(nameof(userMicroService));
            UserManagementMicroService = userManagementMicroService ?? throw new ArgumentNullException(nameof(userManagementMicroService));
        }

        public ActionResult ChangePassword()
        {
            var model = new ProfileChangePasswordModel();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ProfileChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await UserManagementMicroService.ChangePasswordAsync(GetUserId(), model.OldPassword, model.NewPassword);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Password changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> DeleteShippingAddress()
        {
            try
            {
                await UserMicroService.UpdateShippingAddressAsync(GetUserId(), null);

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditEmail()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());
            var model = new ProfileEditEmailModel()
            {
                Email = svcProfileDetailData.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditEmail(ProfileEditEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                await UserManagementMicroService.ChangeEmailAsync(GetUserId(), model.Email);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Email changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditName()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());
            var model = new ProfileEditNameModel()
            {
                FirstName = svcProfileDetailData.FirstName,
                LastName = svcProfileDetailData.LastName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditName(ProfileEditNameModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var svcNameData = new MUser_UpdateName()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                await UserMicroService.UpdateNameAsync(GetUserId(), svcNameData);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Name changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditNickname()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());
            var model = new ProfileEditNicknameModel()
            {
                Nickname = svcProfileDetailData.NickName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditNickname(ProfileEditNicknameModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var svcNicknameData = new MUser_UpdateNickname()
                {
                    NickName = model.Nickname
                };

                await UserMicroService.UpdateNicknameAsync(GetUserId(), svcNicknameData);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Nicknamed changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditShippingAddress()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());

            var model = ModelFactory.CreateProfileEditShippingAddressModel(svcProfileDetailData);

            model.StateCodes = GetStateCodes(string.IsNullOrEmpty(model.StateCode));

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditShippingAddress(ProfileEditShippingAddressModel model)
        {
            //if (!ControllerModelFactory.IsValidPostalCode(model.PostalCode))
            //{
            //    ModelState.AddModelError("PostalCode", "Invalid postal code.");
            //}
            if (!ModelState.IsValid)
            {
                if (model.StateCodes == null)
                {
                    model.StateCodes = GetStateCodes(string.IsNullOrEmpty(model.StateCode));
                }
                return View(model);
            }

            try
            {
                var svcShippingAddressData = new MUser_UpdateShippingAddress()
                {
                    AddressLine1 = Trim(model.AddressLine1),
                    AddressLine2 = Trim(model.AddressLine2),
                    City = Trim(model.City),
                    StateCode = Trim(model.StateCode),
                    PostalCode = ModelFactory.ParsePostalCode(model.PostalCode),
                    CountryCode = Trim(model.CountryCode) ?? "US"
                };

                await UserMicroService.UpdateShippingAddressAsync(GetUserId(), svcShippingAddressData);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Shipping address changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditTimeZone()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());

            var model = new ProfileEditTimeZoneModel()
            {
                TimeZoneId = svcProfileDetailData.TimeZoneId
            };

            model.TimeZones = GetTimeZones(string.IsNullOrEmpty(model.TimeZoneId));

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditTimeZone(ProfileEditTimeZoneModel model)
        {
            if (!ModelState.IsValid)
            {
                if (model.TimeZones == null)
                {
                    model.TimeZones = GetTimeZones(string.IsNullOrEmpty(model.TimeZoneId));
                }
                return View(model);
            }

            try
            {
                var svcTimeZoneData = new MUser_UpdateTimeZone()
                {
                    TimeZoneId = model.TimeZoneId
                };

                await UserMicroService.UpdateTimeZoneAsync(GetUserId(), svcTimeZoneData);

                UserLocale.RemoveFrom(HttpContext);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Timezone changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> EditWebsite()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());
            var model = new ProfileEditWebsiteModel()
            {
                WebsiteUrl = svcProfileDetailData.WebsiteUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditWebsite(ProfileEditWebsiteModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                var svcWebsiteData = new MUser_UpdateWebsite()
                {
                    WebsiteUrl = model.WebsiteUrl
                };

                await UserMicroService.UpdateWebsiteAsync(GetUserId(), svcWebsiteData);

                AddFeedbackMessage(Feedback.FeedbackMessageTypes.Informational, "Website changed.");

                return RedirectToAction("Index");
            }
            catch (ServiceException ex)
            {
                AddModelErrors(ex);
                return View();
            }
        }

        public async Task<ActionResult> Index()
        {
            var svcProfileDetailData = await UserMicroService.GetUserAsync(GetUserId());
            var model = ModelFactory.CreateProfileDetailModel(svcProfileDetailData);
            return View(model);
        }

        private string Trim(string value)
        {
            return !string.IsNullOrEmpty(value)
                ? value.Trim()
                : null;
        }

    }
}