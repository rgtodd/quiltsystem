//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Mvc;

namespace RichTodd.QuiltSystem.Web.Controllers
{
    public class HelpController : Controller
    {

        // GET: Help
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

    }
}