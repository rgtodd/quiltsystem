//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class OrderTransactionTypeCodes
    {
        public const string Submit = "SUBMIT";
        public const string FundsRequired = "FUNDS-REQUIRED";
        public const string FundsReceived = "FUNDS-RECEIVED";
        public const string FulfillmentRequired = "FULFILLMENT-REQUIRED";
        public const string FulfillmentComplete = "FULFILLMENT-COMPLETE";
        public const string FulfillmentReturn = "FULFILLMENT-RETURN";
        public const string Close = "CLOSE";
    }
}