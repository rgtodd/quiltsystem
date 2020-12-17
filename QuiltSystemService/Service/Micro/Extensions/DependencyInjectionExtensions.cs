//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Implementations;

namespace RichTodd.QuiltSystem.Service.Micro.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMicroServices(this IServiceCollection services, Action<MicroServiceOptionsBuilder> optionsAction = null)
        {
            var optionsBuilder = new MicroServiceOptionsBuilder();
            if (optionsAction != null)
            {
                optionsAction.Invoke(optionsBuilder);
            }
            var options = optionsBuilder.Options;

            _ = services
                .AddSingleton<ICacheMicroService, CacheMicroService>()
                .AddSingleton<ICommunicationMicroService, CommunicationMicroService>()
                .AddSingleton<IDesignMicroService, DesignMicroService>()
                .AddSingleton<IDomainMicroService, DomainMicroService>()
                .AddSingleton<IEventProcessorMicroService, EventProcessorMicroService>()
                .AddSingleton<IFulfillmentMicroService, FulfillmentMicroService>()
                .AddSingleton<IFundingMicroService, FundingMicroService>()
                .AddSingleton<IInventoryMicroService, InventoryMicroService>()
                .AddSingleton<IKitMicroService, KitMicroService>()
                .AddSingleton<ILedgerMicroService, LedgerMicroService>()
                .AddSingleton<IOrderMicroService, OrderMicroService>()
                .AddSingleton<IProjectMicroService, ProjectMicroService>()
                .AddSingleton<ISquareMicroService, SquareMicroService>()
                .AddSingleton<IUserMicroService, UserMicroService>();

            if (!options.ExcludeUserManagement)
            {
                _ = services.AddScoped<IUserManagementMicroService, UserManagementMicroService>();
            }

            return services;
        }
    }
}
