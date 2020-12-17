//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Web;
using RichTodd.QuiltSystem.WebAdmin.Models.Color;

namespace RichTodd.QuiltSystem.WebAdmin.Controllers
{
    public class ColorController : ApplicationController<ColorModelFactory>
    {
        private IColorAjaxService ColorAjaxService { get; }

        public ColorController(
            IApplicationLocale applicationLocale,
            IDomainMicroService domainMicroService,
            IColorAjaxService colorAjaxService)
            : base(
                  applicationLocale,
                  domainMicroService)
        {
            ColorAjaxService = colorAjaxService ?? throw new ArgumentNullException(nameof(colorAjaxService));
        }

        //public ActionResult Compare()
        //{
        //    var model = ModelFactory.CreateColorPaletteCompareModel();

        //    return View(model);
        //}

        public async Task<ActionResult> Index(string webColor = null)
        {
            var svcColorPalette = await ColorAjaxService.CreateColorPaletteAsync(webColor);

            var model = ModelFactory.CreateColorPaletteModel(svcColorPalette);

            return View(model);
        }

    }
}