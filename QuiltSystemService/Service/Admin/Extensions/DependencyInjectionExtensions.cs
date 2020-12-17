//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using Microsoft.Extensions.DependencyInjection;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Implementations;

namespace RichTodd.QuiltSystem.Service.Admin.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddAdminServices(this IServiceCollection services)
        {
            _ = services
                .AddSingleton<IAlertAdminService, AlertAdminService>()
                .AddSingleton<IDashboardAdminService, DashboardAdminService>()
                .AddSingleton<IDomainAdminService, DomainAdminService>()
                .AddSingleton<IEventAdminService, EventAdminService>()
                .AddSingleton<IFulfillableAdminService, FulfillableAdminService>()
                .AddSingleton<IFundingAdminService, FundingAdminService>()
                .AddSingleton<IInventoryAdminService, InventoryAdminService>()
                .AddSingleton<IJobAdminService, JobAdminService>()
                .AddSingleton<ILogAdminService, LogAdminService>()
                .AddSingleton<IMessageAdminService, MessageAdminService>()
                .AddSingleton<INotificationAdminService, NotificationAdminService>()
                .AddSingleton<IOrderAdminService, OrderAdminService>()
                .AddSingleton<IReportAdminService, ReportAdminService>()
                .AddSingleton<IReturnAdminService, ReturnAdminService>()
                .AddSingleton<ISalesTaxAdminService, SalesTaxAdminService>()
                .AddSingleton<ISquareAdminService, SquareAdminService>()
                .AddSingleton<IShipmentAdminService, ShipmentAdminService>()
                .AddSingleton<ITransactionAdminService, TransactionAdminService>()
                .AddScoped<IUserAdminService, UserAdminService>();

            return services;
        }
    }
}
