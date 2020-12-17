//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class InventoryItemTag
    {
        public long InventoryItemId { get; set; }
        public int TagId { get; set; }
        public DateTime CreateDateTimeUtc { get; set; }

        public virtual InventoryItem InventoryItem { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
