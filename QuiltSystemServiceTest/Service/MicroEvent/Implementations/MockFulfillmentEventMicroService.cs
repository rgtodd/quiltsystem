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
    public class MockFulfillmentEventMicroService : MockMicroEventMicroService, IFulfillmentEventMicroService
    {
        public MockFulfillmentEventMicroService(
            IApplicationLocale locale,
            ILogger<MockFulfillmentEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        { }

        public async Task HandleFulfillmentEventAsync(MFulfillment_FulfillableEvent eventData)
        {
            using var log = BeginFunction(nameof(FulfillmentEventMicroService), nameof(HandleFulfillmentEventAsync), eventData);
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

        public async Task HandleShipmentRequestEventAsync(MFulfillment_ShipmentRequestEvent eventData)
        {
            using var log = BeginFunction(nameof(FulfillmentEventMicroService), nameof(HandleShipmentRequestEventAsync), eventData);
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

        public async Task HandleShipmentEventAsync(MFulfillment_ShipmentEvent eventData)
        {
            using var log = BeginFunction(nameof(FulfillmentEventMicroService), nameof(HandleShipmentEventAsync), eventData);
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

        public async Task HandleReturnRequestEventAsync(MFulfillment_ReturnRequestEvent eventData)
        {
            using var log = BeginFunction(nameof(FulfillmentEventMicroService), nameof(HandleReturnRequestEventAsync), eventData);
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

        public async Task HandleReturnEventAsync(MFulfillment_ReturnEvent eventData)
        {
            using var log = BeginFunction(nameof(FulfillmentEventMicroService), nameof(HandleReturnEventAsync), eventData);
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
