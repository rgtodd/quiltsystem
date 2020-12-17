//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Orderer
    {
        public Orderer()
        {
            Orders = new HashSet<Order>();
        }

        public long OrdererId { get; set; }
        public string OrdererReference { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual OrdererPendingOrder OrdererPendingOrder { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
