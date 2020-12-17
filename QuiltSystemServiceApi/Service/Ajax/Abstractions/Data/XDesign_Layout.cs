//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;

namespace RichTodd.QuiltSystem.Service.Ajax.Abstractions.Data
{
    // Describes a layout.  Referenced by quilt.layout-list.js.
    //
    public class XDesign_Layout
    {
#pragma warning disable IDE1006 // Naming Styles
        public string className
        {
            get { return nameof(XDesign_Layout); }
            set { if (value != nameof(XDesign_Layout)) throw new ArgumentException("Value does not match class name."); }
        }

        public string id { get; set; }
        public string layoutCategory { get; set; }
        public string layoutName { get; set; }
        public int blockCount { get; set; }
        public int rowCount { get; set; }
        public int columnCount { get; set; }
        public XDesign_FabricStyle[] fabricStyles { get; set; }
        public XDesign_LayoutPreview preview { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
