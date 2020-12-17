//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
namespace RichTodd.QuiltSystem.Service.Micro.Abstractions.Data
{
    public class MKit_KitSpecificationUpdate
    {
        public string Size { get; set; }
        public string CustomWidth { get; set; }
        public string CustomHeight { get; set; }

        public string BorderWidth { get; set; }
        public string CustomBorderWidth { get; set; }
        public MCommon_FabricStyle BorderFabricStyle { get; set; }

        public string BindingWidth { get; set; }
        public string CustomBindingWidth { get; set; }
        public MCommon_FabricStyle BindingFabricStyle { get; set; }

        public bool? HasBacking { get; set; }
        public MCommon_FabricStyle BackingFabricStyle { get; set; }

        public bool? TrimTriangles { get; set; }
    }
}
