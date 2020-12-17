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
    internal class InventoryEventMicroService : MicroEventMicroService, IInventoryEventMicroService
    {
        public InventoryEventMicroService(
            IApplicationLocale locale,
            ILogger<OrderEventMicroService> logger,
            IQuiltContextFactory quiltContextFactory,
            IServiceProvider serviceProvider)
            : base(
                  locale,
                  logger,
                  quiltContextFactory,
                  serviceProvider)
        { }

        public Task OnConsumableQuantityUpdatedAsync(long consumableId, int quantity)
        {
            throw new NotImplementedException();
        }
    }
}
