//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.User.Abstractions;
using RichTodd.QuiltSystem.Service.User.Implementations;

namespace RichTodd.QuiltSystem.Service.User.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddUserServices(this IServiceCollection services)
        {
            _ = services
                .AddSingleton<ICartUserService, CartUserService>()
                .AddSingleton<IDesignUserService, DesignUserService>()
                .AddSingleton<IInventoryUserService, InventoryUserService>()
                .AddSingleton<IProjectUserService, ProjectUserService>()
                .AddSingleton<IOrderUserService, OrderUserService>()
                .AddSingleton<ISessionUserService, SessionUserService>();

            return services;
        }
    }
}
