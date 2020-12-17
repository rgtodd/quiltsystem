//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using Microsoft.EntityFrameworkCore;

//using RichTodd.QuiltSystem.Database.Domain;
//using RichTodd.QuiltSystem.Database.Model;
//using RichTodd.QuiltSystem.Database.Model.Extensions;
//using RichTodd.QuiltSystem.Service.Base;
//using RichTodd.QuiltSystem.Service.Core.Abstractions;
//using RichTodd.QuiltSystem.Service.Database.Abstractions;
//using RichTodd.QuiltSystem.Service.Micro.Abstractions;

//namespace RichTodd.QuiltSystem.Business.Operation
//{
//    internal class ShipmentPostOperation : BusinessOperation
//    {

//        public ShipmentPostOperation(
//            IApplicationLogger applicationLogger,
//            IApplicationLocale applicationLocale,
//            IQuiltContextFactory quiltContextFactory,
//            ICommunicationMicroService communicationMicroService)
//            : base(
//                  applicationLogger,
//                  applicationLocale,
//                  quiltContextFactory,
//                  communicationMicroService)
//        { }

//        public static bool CanPost(Shipment dbShipment)
//        {
//            return dbShipment.ShipmentStatusTypeCode == ShipmentStatusTypes.Open;
//        }

//        public async Task ExecuteAsync(long shipmentId)
//        {
//            using var log = BeginFunction(nameof(ShipmentPostOperation), nameof(ExecuteAsync), shipmentId);
//            try
//            {
//                using var ctx = QuiltContextFactory.Create();

//                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

//                if (!CanPost(dbShipment))
//                {
//                    throw new BusinessOperationException("Shipment cannot be posted.");
//                }

//                dbShipment.ShipmentStatusTypeCodeNavigation = ctx.ShipmentStatusType(ShipmentStatusTypes.Posted);
//                dbShipment.StatusDateTimeUtc = Locale.GetUtcNow();

//                var orderId = ParseOrderId.FromFulfillableReference(dbShipment.ShipmentShipmentRequests.First().ShipmentRequest.Fulfillable.FulfillableReference);
//                var dbOrder = await ctx.Orders.Where(r => r.OrderId == orderId).SingleAsync().ConfigureAwait(false);
//                var ordererUserId = ParseUserId.FromOrdererReference(dbOrder.Orderer.OrdererReference);

//                await CreateNotificationAsync(ctx, ordererUserId, NotificationTypes.OrderShipped, orderId).ConfigureAwait(false);

//                _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
//            }
//            catch (BusinessOperationException ex)
//            {
//                log.LogException(ex);
//                throw;
//            }
//            catch (Exception ex)
//            {
//                log.LogException(ex);

//                using (var ctx = QuiltContextFactory.Create())
//                {
//                    var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleOrDefaultAsync().ConfigureAwait(false);
//                    if (dbShipment != null)
//                    {
//                        dbShipment.ShipmentStatusTypeCodeNavigation = ctx.ShipmentStatusType(ShipmentStatusTypes.Exception);
//                        dbShipment.StatusDateTimeUtc = Locale.GetUtcNow();
//                    }

//                    var topicReference = CreateTopicReference.FromShipmentId(shipmentId);
//                    var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, null);
//                    CreateOperationExceptionAlert(ctx, ex, topicId: topicId);

//                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
//                }

//                throw;
//            }
//            //        }

//    }
//}