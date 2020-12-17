//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_KitPart
    {
        public string Id { get; set; }
        public string Sku { get; set; }
        public string WebColor { get; set; }
        public int Quantity { get; set; }
        public MKit_KitPartUnitOfMeasures UnitOfMeasure { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string Collection { get; set; }
    }
}
