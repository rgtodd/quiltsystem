//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class Orderable
    {
        public Orderable()
        {
            OrderItems = new HashSet<OrderItem>();
            OrderableComponents = new HashSet<OrderableComponent>();
        }

        public long OrderableId { get; set; }
        public string OrderableReference { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public decimal Price { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<OrderableComponent> OrderableComponents { get; set; }
    }
}
