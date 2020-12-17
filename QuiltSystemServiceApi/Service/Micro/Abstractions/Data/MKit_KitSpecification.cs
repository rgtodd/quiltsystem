//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_KitSpecification
    {
        public string Width { get; set; }
        public string Height { get; set; }

        public string TotalWidth { get; set; }
        public string TotalHeight { get; set; }

        public string BorderWidth { get; set; }
        public MCommon_FabricStyle BorderFabricStyle { get; set; }

        public string BindingWidth { get; set; }
        public MCommon_FabricStyle BindingFabricStyle { get; set; }

        public bool HasBacking { get; set; }
        public MCommon_FabricStyle BackingFabricStyle { get; set; }

        public bool TrimTriangles { get; set; }
    }
}
