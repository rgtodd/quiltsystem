//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    public class MockOrderEventMicroService : MockMicroEventMicroService, IOrderEventMicroService
    {
        public MockOrderEventMicroService(
            IApplicationLocale locale,
            ILogger<MockOrderEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        { }

        //public async Task HandleFundingEventAsync(MOrder_FundingEvent eventData)
        //{
        //    using var log = BeginFunction(nameof(OrderEventMicroService), nameof(HandleFundingEventAsync), eventData);
        //    try
        //    {
        //        await Task.CompletedTask;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogException(ex);
        //        throw;
        //    }
        //            //}

        public async Task HandleOrderEventAsync(MOrder_OrderEvent eventData)
        {
            using var log = BeginFunction(nameof(OrderEventMicroService), nameof(HandleOrderEventAsync), eventData);
            try
            {
                await Task.CompletedTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }
    }
}
