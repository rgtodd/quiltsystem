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
        public static IServiceCollection AddMockMicroEventServices(this IServiceCollection services)
        {
            _ = services.AddTransient<ICommunicationEventMicroService, MockCommunicationEventMicroService>()
                .AddTransient<IFulfillmentEventMicroService, MockFulfillmentEventMicroService>()
                .AddTransient<IFundingEventMicroService, MockFundingEventMicroService>()
                .AddTransient<IInventoryEventMicroService, MockInventoryEventMicroService>()
                .AddTransient<IOrderEventMicroService, MockOrderEventMicroService>()
                .AddTransient<IProjectEventMicroService, MockProjectEventMicroService>()
                .AddTransient<ISquareEventMicroService, MockSquareEventMicroService>()
                .AddTransient<IUserEventMicroService, MockUserEventMicroService>();

            return services;
        }
    }
}
