//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RichTodd.QuiltSystem.Service.Admin.Abstractions;
using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Base;
using RichTodd.QuiltSystem.Service.Core.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Implementations
{
    internal class ShipmentAdminService : BaseService, IShipmentAdminService
    {
        private ICommunicationMicroService CommunicationMicroService { get; }
        private IDomainMicroService DomainMicroService { get; }
        private IEventProcessorMicroService EventProcessorMicroService { get; }
        private IFulfillmentMicroService FulfillmentMicroService { get; }
        private IKitMicroService KitMicroService { get; }
        private IOrderMicroService OrderMicroService { get; }
        private IProjectMicroService ProjectMicroService { get; }

        public ShipmentAdminService(
            IApplicationRequestServices requestServices,
            ILogger<ShipmentAdminService> logger,
            ICommunicationMicroService communicationMicroService,
            IDomainMicroService domainMicroService,
            IEventProcessorMicroService eventProcessorMicroService,
            IFulfillmentMicroService fulfillmentMicroService,
            IKitMicroService kitMicroService,
            IOrderMicroService orderMicroService,
            IProjectMicroService projectMicroService)
            : base(requestServices, logger)
        {
            CommunicationMicroService = communicationMicroService ?? throw new ArgumentNullException(nameof(communicationMicroService));
            DomainMicroService = domainMicroService ?? throw new ArgumentNullException(nameof(domainMicroService));
            EventProcessorMicroService = eventProcessorMicroService ?? throw new ArgumentNullException(nameof(eventProcessorMicroService));
            FulfillmentMicroService = fulfillmentMicroService ?? throw new ArgumentNullException(nameof(fulfillmentMicroService));
            KitMicroService = kitMicroService ?? throw new ArgumentNullException(nameof(kitMicroService));
            OrderMicroService = orderMicroService ?? throw new ArgumentNullException(nameof(orderMicroService));
            ProjectMicroService = projectMicroService ?? throw new ArgumentNullException(nameof(projectMicroService));
        }

        #region Shipment Request

        public async Task<IReadOnlyList<AShipment_ShipmentRequestSummary>> GetShipmentRequestSummariesAsync(MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetShipmentRequestSummariesAsync), shipmentRequestStatus, recordCount);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                var mShipmentRequestSummaryList = await FulfillmentMicroService.GetShipmentRequestSummariesAsync(shipmentRequestStatus, recordCount).ConfigureAwait(false);

                var summaries = Create.AShipment_ShipmentRequestSummaries(mShipmentRequestSummaryList.Summaries);

                var result = summaries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AShipment_ShipmentRequest> GetShipmentRequestAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetShipmentRequestAsync), shipmentRequestId);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                var mShipmentRequest = await FulfillmentMicroService.GetShipmentRequestAsync(shipmentRequestId);
                var mTransactions = await FulfillmentMicroService.GetShipmentRequestTransactionSummariesAsync(shipmentRequestId, null, null);
                var mEvents = await FulfillmentMicroService.GetShipmentRequestEventLogSummariesAsync(shipmentRequestId, null, null);

                var mFulfillables = new List<MFulfillment_Fulfillable>();
                foreach (var mShipmentRequestItem in mShipmentRequest.ShipmentRequestItems)
                {
                    // Retrieve the associated fulfillable if we haven't already.
                    //
                    var fulfillableId = mShipmentRequestItem.FulfillableId;
                    if (!mFulfillables.Any(r => r.FulfillableId == fulfillableId))
                    {
                        var mFulfillable = await FulfillmentMicroService.GetFulfillableAsync(fulfillableId).ConfigureAwait(false);
                        mFulfillables.Add(mFulfillable);
                    }
                }

                var allowEdit = await SecurityPolicy.AllowEditFulfillment();

                var shipmentRequest = Create.AShipment_ShipmentRequest(mShipmentRequest, mTransactions, mEvents, mFulfillables, allowEdit);

                var result = shipmentRequest;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AShipment_ShipmentRequest> GetActiveShipmentRequestAsync(long orderId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetActiveShipmentRequestAsync), orderId);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelShipmentRequestAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(CancelShipmentRequestAsync), shipmentRequestId);
            try
            {
                await Assert(SecurityPolicy.IsPrivileged).ConfigureAwait(false);

                await FulfillmentMicroService.CancelShipmentRequestAsync(shipmentRequestId);

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Shipment

        public async Task<IReadOnlyList<AShipment_ShipmentSummary>> GetShipmentSummariesAsync(MFulfillment_ShipmentStatus shipmentStatus, int? recordCount)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetShipmentSummariesAsync), shipmentStatus, recordCount);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                var mShipmentSummaryList = await FulfillmentMicroService.GetShipmentSummariesAsync(shipmentStatus, recordCount);

                var summaries = Create.AShipment_ShipmentSummaries(mShipmentSummaryList.Summaries);

                var result = summaries;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AShipment_Shipment> GetShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetShipmentAsync), shipmentId);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment).ConfigureAwait(false);

                var mShipment = await FulfillmentMicroService.GetShipmentAsync(shipmentId);
                var mTransactions = await FulfillmentMicroService.GetShipmentTransactionSummariesAsync(shipmentId, null, null);
                var mEvents = await FulfillmentMicroService.GetShipmentEventLogSummariesAsync(shipmentId, null, null);

                var mFulfillables = new List<MFulfillment_Fulfillable>();
                foreach (var mShipmentItem in mShipment.ShipmentItems)
                {
                    // Retrieve the associated fulfillable if we haven't already.
                    //
                    var fulfillableId = mShipmentItem.FulfillableId;
                    if (!mFulfillables.Any(r => r.FulfillableId == fulfillableId))
                    {
                        var mFulfillable = await FulfillmentMicroService.GetFulfillableAsync(fulfillableId).ConfigureAwait(false);
                        mFulfillables.Add(mFulfillable);
                    }
                }

                var allowEdit = await SecurityPolicy.AllowEditFulfillment();

                var result = Create.AShipment_Shipment(mShipment, mTransactions, mEvents, mFulfillables, allowEdit);

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<AShipment_Shipment> GetActiveShipmentAsync(long shipmentRequestId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetActiveShipmentAsync), shipmentRequestId);
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task<long> CreateShipmentAsync(AShipment_CreateShipment createShipment)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(CreateShipmentAsync), createShipment);
            try
            {
                await Assert(SecurityPolicy.AllowEditFulfillment);

                var shipmentId = await FulfillmentMicroService.CreateShipmentAsync(createShipment.MCreateShipment);

                var result = shipmentId;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task UpdateShipmentAsync(AShipment_UpdateShipment updateShipment)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(UpdateShipmentAsync), updateShipment);
            try
            {
                await Assert(SecurityPolicy.AllowEditFulfillment);

                await FulfillmentMicroService.UpdateShipmentAsync(updateShipment.MUpdateShipment);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task PostShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(PostShipmentAsync), shipmentId);
            try
            {
                await Assert(SecurityPolicy.AllowEditFulfillment);

                await FulfillmentMicroService.PostShipmentAsync(shipmentId);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task ProcessShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(ProcessShipmentAsync), shipmentId);
            try
            {
                await Assert(SecurityPolicy.AllowEditFulfillment);

                await FulfillmentMicroService.ProcessShipmentAsync(shipmentId).ConfigureAwait(false);

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        public async Task CancelShipmentAsync(long shipmentId)
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(CancelShipmentAsync), shipmentId);
            try
            {
                await Assert(SecurityPolicy.AllowEditFulfillment);

                await FulfillmentMicroService.CancelShipmentAsync(shipmentId);

                _ = await EventProcessorMicroService.ProcessPendingEvents().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        #region Support Methods

        public async Task<IList<AShipment_ShippingVendor>> GetShippingVendorsAsync()
        {
            using var log = BeginFunction(nameof(ShipmentAdminService), nameof(GetShippingVendorsAsync));
            try
            {
                await Assert(SecurityPolicy.AllowViewFulfillment);

                var shippingVendors = new List<AShipment_ShippingVendor>();

                var values = DomainMicroService.GetDomainValues(DomainMicroService.ShippingVendorDomain);
                foreach (var value in values)
                {
                    var shippingVendor = new AShipment_ShippingVendor()
                    {
                        ShippingVendorId = value.Id,
                        Name = value.Value
                    };
                    shippingVendors.Add(shippingVendor);
                }

                var result = shippingVendors;

                log.Result(result);

                return result;
            }
            catch (Exception ex)
            {
                log.Exception(ex);
                throw;
            }
        }

        #endregion

        private static class Create
        {
            public static AShipment_Shipment AShipment_Shipment(
                MFulfillment_Shipment mShipment,
                MFulfillment_ShipmentTransactionSummaryList mTransactions,
                MFulfillment_ShipmentEventLogSummaryList mEvents,
                IList<MFulfillment_Fulfillable> mFulfillables,
                bool allowEdit)
            {
                var shipment = new AShipment_Shipment()
                {
                    MShipment = mShipment,
                    MTransactions = mTransactions,
                    MEvents = mEvents,
                    MFulfillables = mFulfillables,
                    AllowEdit = allowEdit
                };

                return shipment;
            }

            public static AShipment_ShipmentSummary AShipment_ShipmentSummary(MFulfillment_ShipmentSummary mShipment)
            {
                var shipment = new AShipment_ShipmentSummary()
                {
                    ShipmentId = mShipment.ShipmentId,
                    ShipmentNumber = mShipment.ShipmentNumber,
                    FulfillableId = mShipment.FulfillableId,
                    FulfillableName = mShipment.FulfillableName,
                    FulfillableReference = mShipment.FulfillableReference,
                    ShipmentStatusName = mShipment.ShipmentStatus.ToString(),
                    ShippingVendorId = mShipment.ShippingVendorId,
                    TrackingCode = mShipment.TrackingCode,
                    CreateDateTimeUtc = mShipment.CreateDateTimeUtc,
                    StatusDateTimeUtc = mShipment.StatusDateTimeUtc
                };

                return shipment;
            }

            public static AShipment_ShipmentRequest AShipment_ShipmentRequest(
                MFulfillment_ShipmentRequest mShipmentRequest,
                MFulfillment_ShipmentRequestTransactionSummaryList mTransactions,
                MFulfillment_ShipmentRequestEventLogSummaryList mEvents,
                IList<MFulfillment_Fulfillable> mFulfillables,
                bool allowEdit)
            {
                var shipmentRequest = new AShipment_ShipmentRequest()
                {
                    MShipmentRequest = mShipmentRequest,
                    MTransactions = mTransactions,
                    MEvents = mEvents,
                    MFulfillables = mFulfillables,
                    AllowEdit = allowEdit
                };
                return shipmentRequest;
            }

            public static AShipment_ShipmentRequestSummary AShipment_ShipmentRequestSummary(MFulfillment_ShipmentRequestSummary mShipmentRequestSummary)
            {
                return new AShipment_ShipmentRequestSummary()
                {
                    ShipmentRequestId = mShipmentRequestSummary.ShipmentRequestId,
                    ShipmentRequestNumber = mShipmentRequestSummary.ShipmentRequestNumber,
                    FulfillableId = mShipmentRequestSummary.FulfillableId,
                    FulfillableName = mShipmentRequestSummary.FulfillableName,
                    FulfillableReference = mShipmentRequestSummary.FulfillableReference,
                    ShipmentRequestStatus = mShipmentRequestSummary.ShipmentRequestStatus.ToString(),
                    CreateDateTimeUtc = mShipmentRequestSummary.CreateDateTimeUtc,
                    StatusDateTimeUtc = mShipmentRequestSummary.StatusDateTimeUtc
                };
            }

            public static IReadOnlyList<AShipment_ShipmentRequestSummary> AShipment_ShipmentRequestSummaries(IList<MFulfillment_ShipmentRequestSummary> mShipmentRequestSummaries)
            {
                var summaries = new List<AShipment_ShipmentRequestSummary>();

                foreach (var mShipmentRequestSummary in mShipmentRequestSummaries)
                {
                    summaries.Add(AShipment_ShipmentRequestSummary(mShipmentRequestSummary));
                }

                return summaries;
            }

            public static IReadOnlyList<AShipment_ShipmentSummary> AShipment_ShipmentSummaries(IList<MFulfillment_ShipmentSummary> mSummaries)
            {
                var result = new List<AShipment_ShipmentSummary>();

                foreach (var mSummary in mSummaries)
                {
                    var shipment = AShipment_ShipmentSummary(mSummary);
                    result.Add(shipment);
                }

                return result;
            }
        }
    }
}
