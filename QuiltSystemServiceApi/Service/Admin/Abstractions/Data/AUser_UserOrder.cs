//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Admin.Abstractions.Data
{
    public class AUser_UserOrder
    {
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal Total { get; set; }
        public string OrderStatusType { get; set; }
        public DateTime StatusDateTimeUtc { get; set; }
        public DateTime OrderDateTimeUtc { get; set; }
    }
}
