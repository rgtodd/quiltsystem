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
//    internal class ShipmentRequestCancelOperation : BusinessOperation
//    {

//        public ShipmentRequestCancelOperation(
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

//        public static bool CanCancel(ShipmentRequest dbShipmentRequest)
//        {
//            if (dbShipmentRequest.ShipmentRequestStatusTypeCode == ShipmentRequestStatusTypes.Open)
//            {
//                if (dbShipmentRequest.ShipmentShipmentRequests.All(r => r.Shipment.ShipmentStatusTypeCode != ShipmentStatusTypes.Complete))
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public async Task ExecuteAsync(long shipmentRequestId)
//        {
//            using var log = BeginFunction(nameof(ShipmentRequestCancelOperation), nameof(ExecuteAsync), shipmentRequestId);
//            try
//            {
//                using var ctx = QuiltContextFactory.Create();

//                var dbShipmentRequest = await ctx.ShipmentRequests.Where(r => r.ShipmentRequestId == shipmentRequestId).SingleAsync().ConfigureAwait(false);

//                if (!CanCancel(dbShipmentRequest))
//                {
//                    throw new BusinessOperationException("Cannot cancel shipment request.");
//                }

//                dbShipmentRequest.ShipmentRequestStatusTypeCodeNavigation = ctx.ShipmentRequestStatusType(ShipmentRequestStatusTypes.Cancelled);
//                dbShipmentRequest.StatusDateTimeUtc = Locale.GetUtcNow();

//                foreach (var dbShipmentShipmentRequest in dbShipmentRequest.ShipmentShipmentRequests)
//                {
//                    var dbShipment = dbShipmentShipmentRequest.Shipment;
//                    if (dbShipment.ShipmentStatusTypeCode.In(ShipmentStatusTypes.Open, ShipmentStatusTypes.Posted))
//                    {
//                        dbShipment.ShipmentStatusTypeCodeNavigation = ctx.ShipmentStatusType(ShipmentStatusTypes.Cancelled);
//                        dbShipment.StatusDateTimeUtc = Locale.GetUtcNow();
//                    }
//                }

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
//                    var dbShipmentRequest = await ctx.ShipmentRequests.Where(r => r.ShipmentRequestId == shipmentRequestId).SingleOrDefaultAsync().ConfigureAwait(false);
//                    if (dbShipmentRequest != null)
//                    {
//                        dbShipmentRequest.ShipmentRequestStatusTypeCodeNavigation = ctx.ShipmentRequestStatusType(ShipmentRequestStatusTypes.Exception);
//                        dbShipmentRequest.StatusDateTimeUtc = Locale.GetUtcNow();
//                    }

//                    var topicReference = CreateTopicReference.FromShipmentRequestId(shipmentRequestId);
//                    var topicId = await CommunicationMicroService.AllocateTopicAsync(topicReference, null);
//                    CreateOperationExceptionAlert(ctx, ex, topicId: topicId);

//                    _ = await ctx.SaveChangesAsync().ConfigureAwait(false);
//                }

//                throw;
//            }
//            //        }

//    }
//}