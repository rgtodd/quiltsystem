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
//    internal class ShipmentProcessOperation : BusinessOperation
//    {

//        public ShipmentProcessOperation(
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

//        public static bool CanProcess(Shipment dbShipment)
//        {
//            return dbShipment.ShipmentStatusTypeCode == ShipmentStatusTypes.Posted;
//        }

//        public async Task ExecuteAsync(long shipmentId)
//        {
//            using var log = BeginFunction(nameof(ShipmentProcessOperation), nameof(ExecuteAsync), shipmentId);
//            try
//            {
//                using var ctx = QuiltContextFactory.Create();

//                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

//                if (!CanProcess(dbShipment))
//                {
//                    throw new BusinessOperationException("Shipment cannot be processed.");
//                }

//                //OrderTransaction dbOrderTransaction;
//                //{
//                //    //var dbOrderTransactionBuilder = dbShipment.ShipmentRequest.Order.CreateOrderTransactionBuilder(Locale.GetUtcNow())
//                //    //    .Begin(OrderTransactionTypes.Ship, string.Format("Processing shipment {0}.", shipmentId));

//                //    // HACK: OrderStatus
//                //    /*
//                //                foreach (var dbShipmentItem in dbShipment.ShipmentItems)
//                //                {
//                //                    _ = dbOrderTransactionBuilder.AddItem(dbShipmentItem.Fulfillable.FulfillableOrderItem.OrderItem.OrderItemId, dbShipmentItem.Quantity, null);
//                //                }
//                //                */

//                //    //dbOrderTransaction = dbOrderTransactionBuilder.Create();
//                //}

//                // HACK: OrderStatus
//                /*
//                ctx.OnOrderTransactionCreated(dbOrderTransaction, Locale.GetUtcNow());

//                var builder = ctx.GetInventoryItemStockTransactionBuilder(Locale.GetUtcNow(), Locale.GetLocalNow()).Begin(dbOrderTransaction.OrderId);
//                foreach (var dbOrderTransactionItem in dbOrderTransaction.OrderTransactionItems)
//                {
//                    foreach (var dbOrderableComponent in dbOrderTransactionItem.OrderItem.Orderable.OrderableComponents)
//                    {
//                        var dbOrderableProjectComponent = dbOrderableComponent.OrderableProjectComponent;
//                        var dbInventoryItem = dbOrderableProjectComponent.ProjectSnapshotComponent.InventoryItem;

//                        var quantity = dbOrderTransactionItem.FulfillmentCompleteQuantity * dbOrderableProjectComponent.ProjectSnapshotComponent.Quantity;
//                        switch (dbOrderableProjectComponent.ProjectSnapshotComponent.UnitOfMeasureId)
//                        {
//                            case UnitsOfMeasures.FatQuarter: break;
//                            case UnitsOfMeasures.HalfYardage: quantity *= 2; break;
//                            case UnitsOfMeasures.Yardage: quantity *= 4; break;
//                            case UnitsOfMeasures.TwoYards: quantity *= 8; break;
//                            case UnitsOfMeasures.ThreeYards: quantity *= 12; break;
//                        }

//                        _ = builder.ConsumeInventoryItemStock(dbInventoryItem.InventoryItemId, UnitsOfMeasures.FatQuarter, quantity);
//                    }
//                }
//                _ = builder.Create();

//                dbShipment.ShipmentStatusType = ctx.ShipmentStatusType(ShipmentStatusTypes.Complete);
//                dbShipment.StatusDateTimeUtc = Locale.GetUtcNow();
//                dbShipment.OrderTransaction = dbOrderTransaction;
//                */

//                dbShipment.ShipmentShipmentRequests.First().ShipmentRequest.ShipmentRequestStatusTypeCodeNavigation = ctx.ShipmentRequestStatusType(ShipmentRequestStatusTypes.Complete);
//                dbShipment.ShipmentShipmentRequests.First().ShipmentRequest.StatusDateTimeUtc = Locale.GetUtcNow();

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
