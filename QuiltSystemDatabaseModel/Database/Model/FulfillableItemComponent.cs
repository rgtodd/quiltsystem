//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class FulfillableItemComponent
    {
        public long FulfillableItemComponentId { get; set; }
        public long FulfillableItemId { get; set; }
        public string Description { get; set; }
        public string ConsumableReference { get; set; }
        public int Quantity { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual FulfillableItem FulfillableItem { get; set; }
    }
}
