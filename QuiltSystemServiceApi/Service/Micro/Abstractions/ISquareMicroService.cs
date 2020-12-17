//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Threading.Tasks;

using RichTodd.QuiltSystem.Service.Micro.Abstractions.Data;

namespace RichTodd.QuiltSystem.Service.Micro.Abstractions
{
    public interface ISquareMicroService : IEventService
    {
        Task<MSquare_Dashboard> GetDashboardAsync();

        // Customer Methods

        Task<long> AllocateSquareCustomerAsync(string squareCustomerReference);

        Task<long?> LookupSquareCustomerIdAsync(string squareCustomerReference);

        Task<MSquare_Customer> GetSquareCustomerAsync(long squareCustomerId);

        Task<MSquare_CustomerSummaryList> GetSquareCustomerSummariesAsync(long? squareCustomerId, int? recordCount);

        // Payment Methods

        Task<long> AllocateSquarePaymentAsync(string squarePaymentReference, long squareCustomerId);

        Task<long?> LookupSquarePaymentIdAsync(string squarePaymentReference);

        Task<MSquare_Payment> GetSquarePaymentAsync(long squarePaymentId);

        Task<MSquare_Payment> GetSquarePaymentByRefundAsync(long squareRefundId);

        Task<MSquare_PaymentSummaryList> GetSquarePaymentSummariesAsync(long? squareCustomerId, DateTime? paymentDateUtc, int? recordCount);

        Task<MSquare_PaymentTransaction> GetPaymentTransactionAsync(long squarePaymentTransactionId);

        Task<MSquare_PaymentTransactionSummaryList> GetPaymentTransactionSummariesAsync(long? squarePaymentId, string unitOfWork, string source);

        Task<MSquare_PaymentEventLog> GetPaymentEventLogAsync(long paymentEventId);

        Task<MSquare_PaymentEventLogSummaryList> GetPaymentEventLogSummariesAsync(long? squarePaymentId, string unitOfWork, string source);

        // Refund Methods

        Task<MSquare_RefundTransaction> GetRefundTransactionAsync(long squareRefundTransactionId);

        Task<MSquare_RefundTransactionSummaryList> GetRefundTransactionSummariesAsync(long? squareRefundId, long? squarePaymentId, string unitOfWork, string source);

        Task<MSquare_RefundEventLog> GetRefundEventLogAsync(long RefundEventId);

        Task<MSquare_RefundEventLogSummaryList> GetRefundEventLogSummariesAsync(long? squareRefundId, long? squarePaymentId, string unitOfWork, string source);

        // Transaction Methods

        Task<long> CreateSquareWebPaymentRequestAsync(long squarePaymentId, decimal paymentAmount, string nonce);

        Task<MSquare_ProcessWebPaymentRequestResponse> ProcessWebPaymentRequestAsync(long squareWebPaymentRequestId);

        // Webhook Methods

        Task<long> CreateWebhookPayloadAsync(string payload);

        Task ProcessWebhookPayloadAsync(long squarePayloadId);
    }
}
