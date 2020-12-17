//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_KitBuildItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Sku1 { get; set; }
        public string Sku2 { get; set; }
        public string WebColor1 { get; set; }
        public string WebColor2 { get; set; }
        public string PartId { get; set; }
        public MKit_KitBuildItemImage Image { get; set; }
    }
}