//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class ShipmentRequestStatusCodes
    {
        public const string Pending = "PENDING";
        public const string Open = "OPEN";
        public const string Complete = "COMPLETE";
        public const string Cancelled = "CANCELLED";
        public const string Exception = "EXCEPTION";
    }
}