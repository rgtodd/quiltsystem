//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web.Feedback;
using RichTodd.QuiltSystem.Web.Locale;
using RichTodd.QuiltSystem.Web.Paging;
using RichTodd.QuiltSystem.Web.View;

namespace RichTodd.QuiltSystem.Web
{
    [Authorize]
    //[ValidateInput(false)]
    [ApplicationRequireHttps]
    [CapturePagingStateActionFilter]
    [FeedbackActionFilter]
    //[LoggingActionFilter]
    [UserLocaleActionFilter]
    [ViewOptionsActionFilter]
    public abstract class ApplicationControllerBase : Controller, IApplicationController, IApplicationModelFactoryContext
    {
        public ApplicationControllerBase(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService)
        {
            Locale = applicationLocale ?? throw new ArgumentNullException(nameof(applicationLocale));
            DomainMicroService = domainMicroService ?? throw new ArgumentNullException(nameof(domainMicroService));
        }

        public IApplicationLocale Locale { get; }

        public IDomainMicroService DomainMicroService { get; }

        public string GetUserId()
        {
            var userId = HttpContext.GetUserId();
            return userId;
        }

        protected void AddFeedbackMessage(FeedbackMessageTypes messageType, string message)
        {
            FeedbackPool.Singleton.AddFeedbackMessage(GetFeedbackId(), messageType, message);
        }

        protected void AddModelError(string message)
        {
            ModelState.AddModelError("", message);
        }

        protected void AddModelErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                // HACK: IdentityResult error.
                ModelState.AddModelError("", error.Description);
            }
        }

        protected void AddModelErrors(ServiceException ex)
        {
            var detailsExist = false;
            foreach (var error in ex.Details)
            {
                ModelState.AddModelError("", error);
                detailsExist = true;
            }

            if (!detailsExist)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected RedirectToActionResult FallbackRedirectToAction(string actionName)
        {
            return RedirectToAction(actionName);
        }

        protected ViewResult FallbackView(string viewName, object model)
        {
            return View(viewName, model);
        }

        protected IReadOnlyList<FeedbackMessage> GetFeedbackMessages()
        {
            return FeedbackPool.Singleton.GetFeedbackMessages(GetFeedbackId());
        }

        protected IList<SelectListItem> GetStateCodes(bool includeSelect)
        {
            var result = new List<SelectListItem>();

            if (includeSelect)
            {
                result.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "(Select)"
                });
            }

            foreach (var svcValueData in DomainMicroService.GetDomainValues(DomainMicroService.StateDomain))
            {
                result.Add(new SelectListItem()
                {
                    Value = svcValueData.Id,
                    Text = svcValueData.Value
                });
            }

            return result;
        }

        protected IList<SelectListItem> GetTimeZones(bool includeSelect)
        {
            var result = new List<SelectListItem>();

            if (includeSelect)
            {
                result.Add(new SelectListItem()
                {
                    Value = "",
                    Text = "(Select)"
                });
            }

            foreach (var svcTimeZoneInfoData in DomainMicroService.GetTimeZoneInfoList())
            {
                result.Add(new SelectListItem()
                {
                    Value = svcTimeZoneInfoData.TimeZoneId,
                    Text = svcTimeZoneInfoData.Name
                });
            }

            return result;
        }

        private Guid GetFeedbackId()
        {
            // See if an ID has been defined in the HttpContext.
            //
            var contextId = HttpContext.Items[FeedbackActionFilterAttribute.Key];
            if (contextId != null)
            {
                return (Guid)contextId;
            }

            // See if an ID has been specified in the query string.
            //
            var queryStringId = Request.Query[FeedbackActionFilterAttribute.Key];
            if (queryStringId.Count == 1)
            {
                return Guid.Parse(queryStringId[0]);
            }

            // Create a new ID and add it to the HttpContext.
            //
            var id = Guid.NewGuid();
            HttpContext.Items[FeedbackActionFilterAttribute.Key] = id;
            return id;
        }
    }
}