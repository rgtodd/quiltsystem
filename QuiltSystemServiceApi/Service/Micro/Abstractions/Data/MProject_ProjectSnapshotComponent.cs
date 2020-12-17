//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MProject_ProjectSnapshotComponent
    {
        public long ProjectSnapshotComponentId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ConsumableReference { get; set; }
        public string Sku { get; set; }
        public MCommon_UnitsOfMeasure UnitOfMeasure { get; set; }
        public string UnitOfMeasureName { get; set; }
        public string Category { get; set; }
        public string Collection { get; set; }
        public string Manufacturer { get; set; }
    }
}
