//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web.Models.Home;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class HomeController : ApplicationController<HomeModelFactory>
    {
        public HomeController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService)
            : base(
                  applicationLocale,
                  domainMicroService)
        { }

        [AllowAnonymous]
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
        public ActionResult Index()
        {
            return View();
        }

    }
}