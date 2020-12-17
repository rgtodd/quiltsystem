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

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    public class MockInventoryEventMicroService : MockMicroEventMicroService, IInventoryEventMicroService
    {
        public MockInventoryEventMicroService(
            IApplicationLocale locale,
            ILogger<MockInventoryEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        { }

        public async Task OnConsumableQuantityUpdatedAsync(long consumableId, int quantity)
        {
            using var log = BeginFunction(nameof(MockInventoryEventMicroService), nameof(OnConsumableQuantityUpdatedAsync), consumableId, quantity);
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
