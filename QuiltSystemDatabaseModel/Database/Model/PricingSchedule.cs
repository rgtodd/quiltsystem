//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class PricingSchedule
    {
        public PricingSchedule()
        {
            InventoryItems = new HashSet<InventoryItem>();
            PricingScheduleEntries = new HashSet<PricingScheduleEntry>();
        }

        public long PricingScheduleId { get; set; }
        public string Name { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual ICollection<InventoryItem> InventoryItems { get; set; }
        public virtual ICollection<PricingScheduleEntry> PricingScheduleEntries { get; set; }
    }
}
