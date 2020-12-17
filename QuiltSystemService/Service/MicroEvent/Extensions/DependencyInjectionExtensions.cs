//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.MicroEvent.Implementations;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddMicroEventServices(this IServiceCollection services)
        {
            _ = services
                .AddSingleton<ICommunicationEventMicroService, CommunicationEventMicroService>()
                .AddSingleton<IFulfillmentEventMicroService, FulfillmentEventMicroService>()
                .AddSingleton<IFundingEventMicroService, FundingEventMicroService>()
                .AddSingleton<IInventoryEventMicroService, InventoryEventMicroService>()
                .AddSingleton<IOrderEventMicroService, OrderEventMicroService>()
                .AddSingleton<IProjectEventMicroService, ProjectEventMicroService>()
                .AddSingleton<ISquareEventMicroService, SquareEventMicroService>()
                .AddSingleton<IUserEventMicroService, UserEventMicroService>();

            return services;
        }
    }
}
