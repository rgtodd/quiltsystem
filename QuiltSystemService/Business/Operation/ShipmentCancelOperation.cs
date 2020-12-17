//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

//using Microsoft.EntityFrameworkCore;

//using RichTodd.QuiltSystem.Database.Domain;
//using RichTodd.QuiltSystem.Database.Model;
//using RichTodd.QuiltSystem.Database.Model.Extensions;
//using RichTodd.QuiltSystem.Extensions;
//using RichTodd.QuiltSystem.Service.Base;
//using RichTodd.QuiltSystem.Service.Core.Abstractions;
//using RichTodd.QuiltSystem.Service.Database.Abstractions;
//using RichTodd.QuiltSystem.Service.Micro.Abstractions;

//namespace RichTodd.QuiltSystem.Business.Operation
//{
//    internal class ShipmentCancelOperation : BusinessOperation
//    {

//        public ShipmentCancelOperation(
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

//        public static bool CanCancel(Shipment dbShipment)
//        {
//            return dbShipment.ShipmentStatusTypeCode.In(ShipmentStatusTypes.Open, ShipmentStatusTypes.Posted);
//        }

//        public async Task ExecuteAsync(long shipmentId)
//        {
//            using var log = BeginFunction(nameof(ShipmentCancelOperation), nameof(ExecuteAsync), shipmentId);
//            try
//            {
//                using var ctx = QuiltContextFactory.Create();

//                var dbShipment = await ctx.Shipments.Where(r => r.ShipmentId == shipmentId).SingleAsync().ConfigureAwait(false);

//                if (!CanCancel(dbShipment))
//                {
//                    throw new BusinessOperationException("Cannot cancel shipment.");
//                }

//                dbShipment.ShipmentStatusTypeCodeNavigation = ctx.ShipmentStatusType(ShipmentStatusTypes.Cancelled);
//                dbShipment.StatusDateTimeUtc = Locale.GetUtcNow();

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