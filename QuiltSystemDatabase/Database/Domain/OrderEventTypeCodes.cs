//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class OrderEventTypeCodes
    {
        public const string FundingUpdate = "FUNDING";
        public const string FulfillmentUpdate = "FULFILLMENT";
        public const string Close = "CLOSE";
    }
}
