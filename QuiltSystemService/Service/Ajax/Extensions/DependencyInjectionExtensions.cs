//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Ajax.Abstractions;
using RichTodd.QuiltSystem.Service.Ajax.Implementations;

namespace RichTodd.QuiltSystem.Service.Ajax.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAjaxServices(this IServiceCollection services)
        {
            _ = services
                .AddSingleton<IColorAjaxService, ColorAjaxService>()
                .AddSingleton<IDesignAjaxService, DesignAjaxService>();

            return services;
        }
    }
}
