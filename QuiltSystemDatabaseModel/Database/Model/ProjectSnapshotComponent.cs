//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class ProjectSnapshotComponent
    {
        public long ProjectSnapshotComponentId { get; set; }
        public long ProjectSnapshotId { get; set; }
        public int ProjectSnapshotComponentSequence { get; set; }
        public string ConsumableReference { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public int Quantity { get; set; }

        public virtual ProjectSnapshot ProjectSnapshot { get; set; }
        public virtual UnitOfMeasure UnitOfMeasureCodeNavigation { get; set; }
    }
}
