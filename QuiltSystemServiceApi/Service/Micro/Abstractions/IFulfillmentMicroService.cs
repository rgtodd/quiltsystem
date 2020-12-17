//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IFulfillmentMicroService : IEventService
    {
        Task<MFulfillment_Dashboard> GetDashboardAsync();

        // Fulfillable Methods

        Task<MFulfillment_AllocateFulfillableResponse> AllocateFulfillableAsync(MFulfillment_AllocateFulfillable allocateFulfillable);

        Task<long?> LookupFulfillableAsync(string fulfillableReference);

        Task<long?> LookupFulfillableItemAsync(string fulfillableItemReference);

        Task<MFulfillment_Fulfillable> GetFulfillableAsync(long fulfillableId);

        Task<MFulfillment_Fulfillable> GetFulfillableByItemAsync(long fulfillableItemId);

        Task<MFulfillment_FulfillableSummaryList> GetFulfillableSummariesAsync(MFulfillment_FulfillableStatus fulfillableStatus, int? recordCount);

        Task SetFulfillableShippingAddress(long fulfillableId, MCommon_Address shippingAddress);

        Task SetFulfillmentRequestQuantity(long fulfillableItemId, int requestQuantity, string unitOfWork);

        Task<MFulfillment_FulfillableTransaction> GetFulfillableTransactionAsync(long fulfillableTransactionId);

        Task<MFulfillment_FulfillableTransactionSummaryList> GetFulfillableTransactionSummariesAsync(long? fulfillableId, string unitOfWork, string source);

        Task<MFulfillment_FulfillableEventLog> GetFulfillableEventLogAsync(long fulfillableEventId);

        Task<MFulfillment_FulfillableEventLogSummaryList> GetFulfillableEventLogSummariesAsync(long? fulfillableId, string unitOfWork, string source);

        // Shipment Request Methods

        Task<MFulfillment_ShipmentRequest> GetShipmentRequestAsync(long shipmentRequestId);

        Task<MFulfillment_ShipmentRequestSummaryList> GetShipmentRequestSummariesAsync(MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int? recordCount);

        Task<long?> GetPendingShipmentRequestAsync(long fulfillableId);

        Task OpenShipmentRequestAsync(long shipmentRequestId);

        Task CancelShipmentRequestAsync(long shipmentRequestId);

        Task<MFulfillment_ShipmentRequestTransaction> GetShipmentRequestTransactionAsync(long shipmentRequestTransactionId);

        Task<MFulfillment_ShipmentRequestTransactionSummaryList> GetShipmentRequestTransactionSummariesAsync(long? shipmentRequestId, string unitOfWork, string source);

        Task<MFulfillment_ShipmentRequestEventLog> GetShipmentRequestEventLogAsync(long shipmentRequestEventId);

        Task<MFulfillment_ShipmentRequestEventLogSummaryList> GetShipmentRequestEventLogSummariesAsync(long? shipmentRequestId, string unitOfWork, string source);

        // Shipment Methods

        Task<MFulfillment_Shipment> GetShipmentAsync(long shipmentId);

        Task<MFulfillment_ShipmentSummaryList> GetShipmentSummariesAsync(MFulfillment_ShipmentStatus shipmentStatus, int? recordCount);

        Task<long> CreateShipmentAsync(MFulfillment_CreateShipment shipment);

        Task UpdateShipmentAsync(MFulfillment_UpdateShipment shipment);

        Task PostShipmentAsync(long shipmentId);

        Task ProcessShipmentAsync(long shipmentId);

        Task CancelShipmentAsync(long shipmentId);

        Task<MFulfillment_ShipmentTransaction> GetShipmentTransactionAsync(long shipmentTransactionId);

        Task<MFulfillment_ShipmentTransactionSummaryList> GetShipmentTransactionSummariesAsync(long? shipmentId, string unitOfWork, string source);

        Task<MFulfillment_ShipmentEventLog> GetShipmentEventLogAsync(long shipmentEventId);

        Task<MFulfillment_ShipmentEventLogSummaryList> GetShipmentEventLogSummariesAsync(long? shipmentId, string unitOfWork, string source);

        // Return Request Methods

        Task<MFulfillment_ReturnRequest> GetReturnRequestAsync(long returnRequestId);

        Task<MFulfillment_ReturnRequestSummaryList> GetReturnRequestSummariesAsync(MFulfillment_ReturnRequestStatus returnRequestStatus, int? recordCount);

        Task<long> CreateReturnRequestAsync(MFulfillment_CreateReturnRequest returnRequest);

        Task UpdateReturnRequestAsync(MFulfillment_UpdateReturnRequest returnRequest);

        Task PostReturnRequestAsync(long returnRequestId);

        Task CancelReturnRequestAsync(long returnRequestId);

        Task<IReadOnlyList<MFulfillment_ReturnRequestReason>> GetReturnRequestReasonsAsync();

        Task<MFulfillment_ReturnRequestTransaction> GetReturnRequestTransactionAsync(long returnRequestTransactionId);

        Task<MFulfillment_ReturnRequestTransactionSummaryList> GetReturnRequestTransactionSummariesAsync(long? returnRequestId, string unitOfWork, string source);

        Task<MFulfillment_ReturnRequestEventLog> GetReturnRequestEventLogAsync(long returnRequestEventId);

        Task<MFulfillment_ReturnRequestEventLogSummaryList> GetReturnRequestEventLogSummariesAsync(long? returnRequestId, string unitOfWork, string source);

        // Return Methods

        Task<MFulfillment_Return> GetReturnAsync(long returnId);

        Task<MFulfillment_ReturnSummaryList> GetReturnSummariesAsync(MFulfillment_ReturnStatus returnStatus, int? recordCount);

        Task<long> CreateReturnAsync(MFulfillment_CreateReturn _return);

        Task UpdateReturnAsync(MFulfillment_UpdateReturn _return);

        Task PostReturnAsync(long returnId);

        Task ProcessReturnAsync(long returnId);

        Task CancelReturnAsync(long returnId);

        Task<MFulfillment_ReturnTransaction> GetReturnTransactionAsync(long returnTransactionId);

        Task<MFulfillment_ReturnTransactionSummaryList> GetReturnTransactionSummariesAsync(long? returnId, string unitOfWork, string source);

        Task<MFulfillment_ReturnEventLog> GetReturnEventLogAsync(long returnEventId);

        Task<MFulfillment_ReturnEventLogSummaryList> GetReturnEventLogSummariesAsync(long? returnId, string unitOfWork, string source);
    }
}
