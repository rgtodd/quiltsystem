//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IFulfillmentEventMicroService
    {
        Task HandleFulfillmentEventAsync(MFulfillment_FulfillableEvent eventData);
        Task HandleShipmentRequestEventAsync(MFulfillment_ShipmentRequestEvent eventData);
        Task HandleShipmentEventAsync(MFulfillment_ShipmentEvent eventData);
        Task HandleReturnRequestEventAsync(MFulfillment_ReturnRequestEvent eventData);
        Task HandleReturnEventAsync(MFulfillment_ReturnEvent eventData);
    }
}
