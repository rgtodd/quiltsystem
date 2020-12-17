//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Database.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.MicroEvent.Implementations
{
    internal class OrderEventMicroService : MicroEventMicroService, IOrderEventMicroService
    {
        public OrderEventMicroService(
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

        public async Task HandleOrderEventAsync(MOrder_OrderEvent eventData)
        {
            using var log = BeginFunction(nameof(OrderEventMicroService), nameof(HandleOrderEventAsync), eventData);
            try
            {
                switch (eventData.EventTypeCode)
                {
                    case OrderEventTypeCodes.FulfillmentUpdate:
                        await HandleFulfillmentRequiredEventAsync(eventData).ConfigureAwait(false);
                        break;

                    case OrderEventTypeCodes.FundingUpdate:
                        await HandleFundingRequiredEventAsync(eventData).ConfigureAwait(false);
                        break;

                    case OrderEventTypeCodes.Close:
                        // No action required
                        break;

                    default:
                        throw new ArgumentException($"Unknown event type code {eventData.EventTypeCode}.");
                }
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        private async Task HandleFundingRequiredEventAsync(MOrder_OrderEvent eventData)
        {
            var fundableReference = CreateFundableReference.FromOrderId(eventData.OrderId);
            var fundableId = await FundingMicroService.AllocateFundableAsync(fundableReference).ConfigureAwait(false);

            await FundingMicroService.SetFundsRequiredAsync(
                fundableId,
                eventData.FundsRequiredIncome,
                eventData.FundsRequiredSalesTax,
                eventData.FundsRequiredSalesTaxJurisdiction,
                eventData.UnitOfWork).ConfigureAwait(false);
        }

        private async Task HandleFulfillmentRequiredEventAsync(MOrder_OrderEvent eventData)
        {
            var fulfillableReference = CreateFulfillableReference.FromOrderId(eventData.OrderId);

            var existingFulfillableId = await FullfillmentMicroService.LookupFulfillableAsync(fulfillableReference);

            long fulfillableId;
            if (!existingFulfillableId.HasValue)
            {
                var allocateFulfillable = new MFulfillment_AllocateFulfillable()
                {
                    FulfillableReference = fulfillableReference,
                    Name = $"Order {eventData.OrderNumber}",
                    ShippingAddress = eventData.ShippingAddress,
                    FulfillableItems = new List<MFulfillment_AllocateFulfillableItem>()
                };
                foreach (var fulfillmentEventItem in eventData.OrderEventItems)
                {
                    var fulfillableItemReference = CreateFulfillableItemReference.FromOrderItemId(fulfillmentEventItem.OrderItemId);
                    var allocateFulfillableItem = new MFulfillment_AllocateFulfillableItem()
                    {
                        FulfillableItemReference = fulfillableItemReference,
                        Description = fulfillmentEventItem.Description,
                        ConsumableReference = fulfillmentEventItem.ConsumableReference,
                        FulfillableItemComponents = new List<MFulfillment_AllocateFulfillableItemComponent>()
                    };
                    foreach (var fulfillmentEventItemComponent in fulfillmentEventItem.OrderEventItemComponents)
                    {
                        var allocateFulfillableItemComponent = new MFulfillment_AllocateFulfillableItemComponent()
                        {
                            Description = fulfillmentEventItemComponent.Description,
                            ConsumableReference = fulfillmentEventItemComponent.ConsumableReference,
                            Quantity = fulfillmentEventItemComponent.Quantity
                        };
                        allocateFulfillableItem.FulfillableItemComponents.Add(allocateFulfillableItemComponent);
                    }
                    allocateFulfillable.FulfillableItems.Add(allocateFulfillableItem);
                }

                var allocateFulfillableResponse = await FullfillmentMicroService.AllocateFulfillableAsync(allocateFulfillable);

                fulfillableId = allocateFulfillableResponse.FulfillableId;
            }
            else
            {
                fulfillableId = existingFulfillableId.Value;
            }
            //LogMessage($"Fullfillable ID = {fulfillableId}.");

            foreach (var fulfillmentEventItem in eventData.OrderEventItems)
            {
                if (fulfillmentEventItem.RequiredQuantity != 0)
                {
                    var fulfillableItemReference = CreateFulfillableItemReference.FromOrderItemId(fulfillmentEventItem.OrderItemId);
                    var fulfillableItemId = await FullfillmentMicroService.LookupFulfillableItemAsync(fulfillableItemReference);
                    if (fulfillableItemId.HasValue)
                    {
                        await FullfillmentMicroService.SetFulfillmentRequestQuantity(fulfillableItemId.Value, fulfillmentEventItem.RequiredQuantity, eventData.UnitOfWork).ConfigureAwait(false);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Fulfillable item not found for reference {fulfillableItemReference}");
                    }
                }
            }
        }
    }
}
