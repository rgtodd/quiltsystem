//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.WebAdmin.Models.Home;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class HomeController : ApplicationController<DashboardModelFactory>
    {
        private IDashboardAdminService DashboardAdminService { get; }

        public HomeController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IDashboardAdminService dashboardAdminService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            DashboardAdminService = dashboardAdminService ?? throw new ArgumentNullException(nameof(dashboardAdminService));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var svcDashboard = await DashboardAdminService.GetDashboardDataAsync();

                var model = ModelFactory.CreateDashboardModel(svcDashboard);

                return View("Dashboard", model);
            }
            else
            {
                return View("Index");
            }
        }

    }
}