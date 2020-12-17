//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Security;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Core.Implementations;

namespace RichTodd.QuiltSystem.Service.Core.Extensions
{
    public static class CoreWebDependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationAuthorization(this IServiceCollection services)
        {
            _ = services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    ApplicationPolicies.IsAdministrator,
                    policy => policy.Requirements.Add(new RoleRequirement(ApplicationRoles.Administrator)));

                options.AddPolicy(
                    ApplicationPolicies.IsService,
                    policy => policy.Requirements.Add(new RoleRequirement(ApplicationRoles.Service)));

                options.AddPolicy(
                    ApplicationPolicies.IsEndUser,
                    policy => policy.Requirements.Add(new RoleRequirement(ApplicationRoles.User)));

                options.AddPolicy(
                    ApplicationPolicies.IsPriviledged,
                    policy => policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service })));

                options.AddPolicy(
                    ApplicationPolicies.AllowEditFinancial,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RoleRequirement(ApplicationRoles.FinancialEditor));
                    });

                options.AddPolicy(
                    ApplicationPolicies.AllowViewFinancial,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.FinancialEditor, ApplicationRoles.FinancialViewer }));
                    });

                options.AddPolicy(
                    ApplicationPolicies.AllowEditFulfillment,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RoleRequirement(ApplicationRoles.FulfillmentEditor));
                    });

                options.AddPolicy(
                    ApplicationPolicies.AllowViewFulfillment,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.FulfillmentEditor, ApplicationRoles.FulfillmentViewer }));
                    });

                options.AddPolicy(
                    ApplicationPolicies.AllowEditUser,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RoleRequirement(ApplicationRoles.UserEditor));
                    });

                options.AddPolicy(
                    ApplicationPolicies.AllowViewUser,
                    policy =>
                    {
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.Administrator, ApplicationRoles.Service }));
                        policy.Requirements.Add(new RolesRequirement(new List<string>() { ApplicationRoles.UserEditor, ApplicationRoles.UserViewer }));
                    });
            });

            // Register the requirement handlers required by the policies above.
            //
            _ = services.AddSingleton<IAuthorizationHandler, RoleRequirementHandler>();
            _ = services.AddSingleton<IAuthorizationHandler, RolesRequirementHandler>();

            return services;
        }

        public static IServiceCollection AddCoreWebServices(this IServiceCollection services, Action<CoreServiceOptionsBuilder> optionsAction = null)
        {
            var optionsBuilder = new CoreServiceOptionsBuilder();
            if (optionsAction != null)
            {
                optionsAction.Invoke(optionsBuilder);
            }
            var options = optionsBuilder.Options;

            if (options.UseHttpContext)
            {
                _ = services
                    .AddSingleton<IApplicationSecurityPolicy, ApplicationSecurityPolicyWeb>()
                    .AddSingleton<IApplicationLocale, ApplicationLocaleWeb>();
            }
            else
            {
                if (string.IsNullOrEmpty(options.StaticUserId))
                {
                    throw new ArgumentException("Static user ID not specified.");
                }

                _ = services
                    .AddSingleton<IApplicationSecurityPolicy, ApplicationSecurityPolicyStatic>(serviceProvider => new ApplicationSecurityPolicyStatic(options.StaticUserId))
                    .AddSingleton<IApplicationLocale, ApplicationLocaleStatic>();
            }

            _ = services
                .AddSingleton<IApplicationEmailSender, ApplicationEmailSender>()
                .AddSingleton<IApplicationRequestServices, ApplicationRequestServices>();

            return services;
        }
    }
}