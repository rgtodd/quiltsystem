//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Database.Domain
{
    public static class OrderStatusCodes
    {
        public const string Pending = "PENDING";
        public const string Submitted = "SUBMITTED";
        public const string Fulfilling = "FULFILLING";
        public const string Closed = "CLOSED";
    }
}