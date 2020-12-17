//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Admin.Abstractions.Data;
using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions
{
    public interface ISquareAdminService
    {
        Task<ASquare_Customer> GetCustomerAsync(long squareCustomerId);

        Task<ASquare_CustomerSummaryList> GetCustomerSummariesAsync(long? squareCustomerId, int? recordCount);

        Task<ASquare_Payment> GetPaymentAsync(long squarePaymentId);

        Task<ASquare_Payment> GetPaymentByRefundAsync(long squareRefundId);

        Task<ASquare_PaymentSummaryList> GetPaymentSummariesAsync(long? squareCustomerId, DateTime? paymentDateUtc, int? recordCount);
    }
}
