//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class ReturnRequestReasonCodes
    {
        public const string ItemNotOrdered = "ITEM-NOT-ORDERED";
        public const string ItemDefective = "ITEM-DEFECTIVE";
        public const string ItemDamagedDuringShipping = "SHIPPING-DAMAGE";
        public const string NoLongerNeeded = "NO-LONGER-NEEDED";
        public const string ItemArrivedTooLate = "ARRIVED-TOO-LATE";
    }
}
