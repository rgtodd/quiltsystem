//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System.Collections.Generic;

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class UnitOfMeasure
    {
        public UnitOfMeasure()
        {
            InventoryItemStocks = new HashSet<InventoryItemStock>();
            InventoryItemUnits = new HashSet<InventoryItemUnit>();
            PricingScheduleEntries = new HashSet<PricingScheduleEntry>();
            ProjectSnapshotComponents = new HashSet<ProjectSnapshotComponent>();
        }

        public string UnitOfMeasureCode { get; set; }
        public string Name { get; set; }

        public virtual ICollection<InventoryItemStock> InventoryItemStocks { get; set; }
        public virtual ICollection<InventoryItemUnit> InventoryItemUnits { get; set; }
        public virtual ICollection<PricingScheduleEntry> PricingScheduleEntries { get; set; }
        public virtual ICollection<ProjectSnapshotComponent> ProjectSnapshotComponents { get; set; }
    }
}
