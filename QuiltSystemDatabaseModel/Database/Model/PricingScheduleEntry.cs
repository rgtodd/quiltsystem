//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//

#nullable disable

namespace RichTodd.QuiltSystem.Database.Model
{
    public partial class PricingScheduleEntry
    {
        public long PricingScheduleEntryId { get; set; }
        public long PricingScheduleId { get; set; }
        public string UnitOfMeasureCode { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }

        public virtual PricingSchedule PricingSchedule { get; set; }
        public virtual UnitOfMeasure UnitOfMeasureCodeNavigation { get; set; }
    }
}
