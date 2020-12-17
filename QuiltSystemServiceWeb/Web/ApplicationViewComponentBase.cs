//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.AspNetCore.Mvc;

using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Web
{
    public abstract class ApplicationViewComponentBase : ViewComponent, IApplicationViewComponent, IApplicationModelFactoryContext
    {
        public ApplicationViewComponentBase(
            IApplicationLocale applicationLocale)
        {
            Locale = applicationLocale ?? throw new ArgumentNullException(nameof(applicationLocale));
        }

        public IApplicationLocale Locale { get; }
    }
}