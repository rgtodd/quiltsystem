//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Implementations;

namespace RichTodd.QuiltSystem.Service.Database.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
        {
            _ = services.AddTransient<IQuiltContextFactory, QuiltContextFactory>();

            return services;
        }
    }
}
