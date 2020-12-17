//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class OrdererPendingOrder
    {
        public long OrdererId { get; set; }
        public long OrderId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }

        public virtual Order Order { get; set; }
        public virtual Orderer Orderer { get; set; }
    }
}
