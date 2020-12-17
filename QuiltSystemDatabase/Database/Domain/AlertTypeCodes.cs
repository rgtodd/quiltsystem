//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class AlertTypeCodes
    {
        public const string PayPalIpnSuccess = "PAYPAL-IPN-SUCCESS";
        public const string PayPalIpnFailure = "PAYPAL-IPN-FAILURE";
        public const string OrderReceipt = "ORDER-RECEIPT";
        public const string OrderReceiptMismatch = "ORDER-RECEIPT-MISMATCH";
        public const string OrderPaymentMismatch = "ORDER-PAYMENT-MISMATCH";
        public const string UnexpectedOrderPayment = "UNEXPECTED-ORDER-PAYMENT";
        public const string OrderReceiptFailure = "ORDER-RECEIPT-FAILURE";
        public const string OperationException = "OPERATION-EXCEPTION";
    }
}