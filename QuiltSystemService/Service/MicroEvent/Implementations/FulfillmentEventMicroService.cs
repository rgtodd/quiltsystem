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
    internal class FulfillmentEventMicroService : MicroEventMicroService, IFulfillmentEventMicroService
    {
        public FulfillmentEventMicroService(
            IApplicationLocale locale,
            ILogger<FulfillmentEventMicroService> logger,
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
                using var ctx = CreateQuiltContext();

                foreach (var fulfillmentEventItem in eventData.FulfillableEventItems)
                {
                    if (fulfillmentEventItem.FulfillmentCompleteQuantity != 0)
                    {
                        if (TryParseOrderItemId.FromFulfillableItemReference(fulfillmentEventItem.FulfillmentItemReference, out var orderItemId))
                        {
                            await OrderMicroService.SetFulfillmentCompleteAsync(orderItemId, fulfillmentEventItem.FulfillmentCompleteQuantity, eventData.UnitOfWork).ConfigureAwait(false);
                        }
                    }

                    if (fulfillmentEventItem.FulfillmentReturnQuantity != 0)
                    {
                        if (TryParseOrderItemId.FromFulfillableItemReference(fulfillmentEventItem.FulfillmentItemReference, out var orderItemId))
                        {
                            await OrderMicroService.SetFulfillmentReturnAsync(orderItemId, fulfillmentEventItem.FulfillmentReturnQuantity, eventData.UnitOfWork).ConfigureAwait(false);
                        }
                    }
                }
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
                using var ctx = CreateQuiltContext();

                switch (eventData.EventType)
                {
                    case MFulfillment_ShipmentEventTypes.Process:
                        {
                            var fulfillableIds = new HashSet<long>();
                            foreach (var shipmentRequestId in eventData.ShipmentRequestIds)
                            {
                                var mShipmentRequest = await FullfillmentMicroService.GetShipmentRequestAsync(shipmentRequestId).ConfigureAwait(false);
                                foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
                                {
                                    var mFulfillable = await FullfillmentMicroService.GetFulfillableByItemAsync(mShipmentRequestItem.FulfillableItemId).ConfigureAwait(false);
                                    var fulfillableId = mFulfillable.FulfillableId;
                                    if (fulfillableIds.Contains(fulfillableId))
                                    {
                                        _ = fulfillableIds.Add(fulfillableId);

                                        if (TryParseOrderId.FromFulfillableReference(mFulfillable.FulfillableReference, out var orderId))
                                        {
                                            var mOrder = await OrderMicroService.GetOrderAsync(orderId).ConfigureAwait(false);

                                            var userId = ParseUserId.FromOrdererReference(mOrder.OrdererReference);

                                            var participantReference = CreateParticipantReference.FromUserId(userId);
                                            var participantId = await CommunicationMicroService.AllocateParticipantAsync(participantReference).ConfigureAwait(false);

                                            var topicReference = CreateTopicReference.FromOrderId(orderId);
                                            var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, null).ConfigureAwait(false);

                                            await CommunicationMicroService.SendNotification(participantId, NotificationTypeCodes.OrderShipped, topicId).ConfigureAwait(false);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }

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
