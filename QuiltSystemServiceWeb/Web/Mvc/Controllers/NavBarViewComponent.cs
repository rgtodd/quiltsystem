//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Web.Mvc.Models;

namespace RichTodd.QuiltSystem.Web.Mvc.Controllers
{
    public class NavBarViewComponent : ApplicationViewComponent<NavBarVcModelFactory>
    {
        private readonly ISessionUserService m_sessionService;

        public NavBarViewComponent(
            IApplicationLocale applicationLocale,
            ISessionUserService sessionService)
            : base(applicationLocale)
        {
            m_sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Content(string.Empty);
            }

            var userId = HttpContext.GetUserId();
            var svcSession = await m_sessionService.GetSession(userId);

            var model = ModelFactory.CreateNavBarVcModel(svcSession);

            return View(model);
        }
    }
}
