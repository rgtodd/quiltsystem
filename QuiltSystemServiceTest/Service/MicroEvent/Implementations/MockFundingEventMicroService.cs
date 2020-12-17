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
    public class MockFundingEventMicroService : MockMicroEventMicroService, IFundingEventMicroService
    {
        public MockFundingEventMicroService(
            IApplicationLocale locale,
            ILogger<MockFundingEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        {
        }

        public async Task HandleFundableEventAsync(MFunding_FundableEvent eventData)
        {
            using var log = BeginFunction(nameof(FundingEventMicroService), nameof(HandleFundableEventAsync), eventData);
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

        public async Task HandleFunderEventAsync(MFunding_FunderEvent eventData)
        {
            using var log = BeginFunction(nameof(FundingEventMicroService), nameof(HandleFunderEventAsync), eventData);
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
