//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.User.Abstractions;

namespace RichTodd.QuiltSystem.Web.View
{
    public sealed class ViewOptionsActionFilterAttribute : ActionFilterAttribute, IFilterFactory
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ViewOptionsActionFilterAttribute(
                serviceProvider.GetService<ISessionUserService>());
        }

        private readonly ISessionUserService m_sessionService;

        public ViewOptionsActionFilterAttribute()
        { }

        public ViewOptionsActionFilterAttribute(ISessionUserService sessionService)
        {
            m_sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            var userId = filterContext.HttpContext.GetUserId();

            if (!string.IsNullOrEmpty(userId))
            {
                var svcSession = await m_sessionService.GetViewOptions(userId).ConfigureAwait(false);

                ViewOptions viewOptions = new ViewOptions(svcSession.EcommerceEnabled, svcSession.PublicRegistrationEnabled);
                viewOptions.AddTo(filterContext.HttpContext);
            }
            else
            {
                var svcSession = await m_sessionService.GetViewOptions(null).ConfigureAwait(false);

                ViewOptions viewOptions = new ViewOptions(svcSession.EcommerceEnabled, svcSession.PublicRegistrationEnabled);
                viewOptions.AddTo(filterContext.HttpContext);
            }

            await next().ConfigureAwait(false);
        }
    }
}