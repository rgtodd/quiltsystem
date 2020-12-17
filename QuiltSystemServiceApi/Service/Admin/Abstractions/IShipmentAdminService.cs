//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IShipmentAdminService
    {
        // Shipment Request Methods

        Task<IReadOnlyList<AShipment_ShipmentRequestSummary>> GetShipmentRequestSummariesAsync(MFulfillment_ShipmentRequestStatus shipmentRequestStatus, int? recordCount);
        Task<AShipment_ShipmentRequest> GetShipmentRequestAsync(long shipmentRequestId);
        Task<AShipment_ShipmentRequest> GetActiveShipmentRequestAsync(long orderId);

        Task CancelShipmentRequestAsync(long shipmentRequestId);

        // Shipment Methods

        Task<IReadOnlyList<AShipment_ShipmentSummary>> GetShipmentSummariesAsync(MFulfillment_ShipmentStatus shipmentStatus, int? recordCount);
        Task<AShipment_Shipment> GetShipmentAsync(long shipmentId);
        Task<AShipment_Shipment> GetActiveShipmentAsync(long shipmentRequestId);

        Task<long> CreateShipmentAsync(AShipment_CreateShipment createShipment);
        Task UpdateShipmentAsync(AShipment_UpdateShipment updateShipment);
        Task PostShipmentAsync(long shipmentId);
        Task ProcessShipmentAsync(long shipmentId);
        Task CancelShipmentAsync(long shipmentId);

        // Support Methods

        Task<IList<AShipment_ShippingVendor>> GetShippingVendorsAsync();
    }
}
