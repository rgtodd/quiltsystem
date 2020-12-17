//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Implementations;

namespace RichTodd.QuiltSystem.Service.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services, Action<CoreServiceOptionsBuilder> optionsAction = null)
        {
            var optionsBuilder = new CoreServiceOptionsBuilder();
            if (optionsAction != null)
            {
                optionsAction.Invoke(optionsBuilder);
            }
            var options = optionsBuilder.Options;

            if (options.UseHttpContext)
            {
                throw new ArgumentException("HttpContext not supported by core services.");
            }

            if (string.IsNullOrEmpty(options.StaticUserId))
            {
                throw new ArgumentException("Static user ID not specified.");
            }

            _ = services
                .AddSingleton<IApplicationSecurityPolicy, ApplicationSecurityPolicyStatic>(serviceProvider => new ApplicationSecurityPolicyStatic(options.StaticUserId))
                .AddSingleton<IApplicationLocale, ApplicationLocaleStatic>()
                .AddSingleton<IApplicationEmailSender, ApplicationEmailSender>()
                .AddSingleton<IApplicationRequestServices, ApplicationRequestServices>();

            return services;
        }
    }
}
