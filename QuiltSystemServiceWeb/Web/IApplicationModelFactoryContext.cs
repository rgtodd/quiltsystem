//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.AspNetCore.Http;

using RichTodd.QuiltSystem.Service.Core.Abstractions;

namespace RichTodd.QuiltSystem.Web
{
    public interface IApplicationModelFactoryContext
    {
        IApplicationLocale Locale { get; }

        HttpContext HttpContext { get; }
    }
}