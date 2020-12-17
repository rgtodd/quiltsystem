//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface IOrderMicroService : IEventService
    {
        // Entity Allocation Methods

        Task<long> AllocateOrdererAsync(string ordererReference);

        Task<long?> LookupOrdererAsync(string ordererReference);

        Task<MOrder_AllocateOrderableResponse> AllocateOrderableAsync(MOrder_AllocateOrderable mAllocateOrderable);

        Task<MOrder_Dashboard> GetDashboardAsync();

        // Core Methods

        Task<MOrder_Order> GetCartOrderAsync(long ordererId);

        // Cart Methods

        Task<bool> AddCartItemAsync(long ordererId, long orderableId, int quantity);

        Task<bool> UpdateCartItemQuantityAsync(long ordererId, long orderItemId, int quantity);

        Task<bool> DeleteCartItemAsync(long ordererId, int orderItemId);

        Task<bool> UpdateShippingAddressAsync(long ordererId, MCommon_Address shippingAddress);

        Task<long> SubmitCartAsync(long ordererId);

        // Order Methods

        Task<MOrder_Order> GetOrderAsync(long orderId);

        Task<MOrder_OrderItem> GetOrderItemAsync(long orderItemId);

        Task<MOrder_OrderList> GetOrdersAsync(long ordererId);

        Task<MOrder_OrderSummaryList> GetOrderSummariesAsync(string orderNumber, DateTime? orderDateUtc, MOrder_OrderStatus orderStatus, long? ordererId, int? recordCount);

        Task<bool> SetFundsReceivedAsync(long orderId, decimal fundsReceived, string unitOfWork);

        Task SetFulfillmentCompleteAsync(long orderItemId, int quantity, string unitOfWork);

        Task SetFulfillmentReturnAsync(long orderItemId, int quantity, string unitOfWork);

        Task<MOrder_OrderTransaction> GetOrderTransactionAsync(long orderTransactionId);

        Task<MOrder_OrderTransactionSummaryList> GetOrderTransactionSummariesAsync(long? orderId, string unitOfWork, string source);

        Task<MOrder_OrderEventLog> GetOrderEventLogAsync(long shipmentRequestEventId);

        Task<MOrder_OrderEventLogSummaryList> GetOrderEventLogSummariesAsync(long? orderId, string unitOfWork, string source);
    }
}
