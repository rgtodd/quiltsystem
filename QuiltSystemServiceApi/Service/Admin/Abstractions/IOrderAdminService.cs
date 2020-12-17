//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface IOrderAdminService
    {
        Task EnsureOrderAcceptedAsync(long orderId);

        Task<AOrder_Order> GetOrderAsync(long orderId);

        Task<IReadOnlyList<AOrder_OrderSummary>> GetOrderSummariesAsync(string orderNumber, DateTime? orderDateUtc, MOrder_OrderStatus orderStatus, string userName, int? recordCount);

        Task<long> PostOrderTransactionAsync(AOrder_PostOrderTransaction transaction);
    }
}